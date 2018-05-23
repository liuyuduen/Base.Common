using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.Utility.WebService
{
    public class WebServiceInvoker
    {
        public static string PREFIX_BASE64STR = "B_64STR:";
        public static string PREFIX_BYTESSTR = "/9j/4AA";
        public static string PREFIX_XMLSTR = "<?xml ";
        private static JsonSerializer DictionaryKeyValuePairCompatibleSerializer;
        private static Dictionary<string, Assembly> cacheAssembly = new Dictionary<string, Assembly>();
        private const string LOGSOURCE = "WebServiceInvoker";

        static WebServiceInvoker()
        {
            ServicePointManager.DefaultConnectionLimit = 1024;
            ServicePointManager.MaxServicePointIdleTime = 60000;

            var settings = Helper.CreateJsonSerializerSettings(new List<JsonConverter> { new DictionaryKeyValuePairCompatibleConverter<object, object>() });
            DictionaryKeyValuePairCompatibleSerializer = JsonSerializer.Create(settings);
        }

        /// <summary>
        /// 获取缓存中的WebService列表
        /// </summary>
        /// <returns>已缓存的WebService列表</returns>
        public static List<string> GetCachedWS()
        {
            return cacheAssembly.Keys.ToList();
        }
        /// <summary>
        /// 删除WebService的缓存
        /// </summary>
        /// <param name="url">WebServiceURL</param>
        public static void DelCachedWS(string url)
        {
            lock (cacheAssembly)
            {
                cacheAssembly.Remove(url);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionUrl">用于执行 WebService 的请求地址</param>
        /// <param name="metaUrl">用于解析 WebService 的 WSDL 地址（一般来说应该与 <paramref name="actionUrl"/> 一致，但也有特殊情况，比如 WCF 中指定了与服务接口不一致的 Meta(WSDL) 地址。）</param>
        /// <param name="methodname"></param>
        /// <param name="args"></param>
        /// <param name="returnType"></param>
        /// <param name="headers"></param>
        /// <param name="agent"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static object InvokeWebService(string actionUrl, string methodname, object[] args,
            out Type returnType,
            string metaUrl = null,
            Dictionary<string, string> headers = null,
            string agent = null,
            int timeout = RequestHelper.DEFAULT_HTTP_TIMEOUT)
        {
            // 当直接使用 InvokeWebService 访问其他服务时，arg 可能传入的直接是本地的类，而不是来自消费者、然后序列化为 JObject/JArray 的对象
            // 此时需要序列化再反序列化一次，从而使得该对象能够正常传入服务WSDL的客户端代码的方法中。
            if (args != null)
            {
                args = (object[])JArray.FromObject(args).ToObject(typeof(object[]));
            }

            if (string.IsNullOrWhiteSpace(metaUrl))
                metaUrl = actionUrl;

            Assembly assembly = GetAssembly(metaUrl, true);
            Debug.Assert(assembly != null);

            //调用方法             
            object res = null;
            bool bFound = false;
            bool bRefReturnType = false;
            foreach (Type t in assembly.GetExportedTypes())
            {
                MethodInfo mi = t.GetMethod(methodname);
                if (mi != null)
                {
                    bFound = true;
                    SoapHttpClientProtocol obj = (SoapHttpClientProtocol)Activator.CreateInstance(t);
                    obj.Timeout = timeout;
                    obj.ConnectionGroupName = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
                    obj.AllowAutoRedirect = false;
                    if (obj is ContextedSoapHttpClientProtocol)
                    {
                        if (headers != null && headers.Any())
                        {
                            ((ContextedSoapHttpClientProtocol)obj).SetHeaders(headers);
                        }
                    }

                    if (!string.IsNullOrEmpty(agent))
                    {
                        if (!string.IsNullOrEmpty(obj.UserAgent))
                        { obj.UserAgent += " "; }
                        obj.UserAgent += agent;
                    }

                    // Url 属性中应该有该方法应该调用的Endpoint地址。
                    // 当Meta地址和服务本身的Endpoint调用地址不一致时，不能直接用Meta地址赋值到该属性。
                    if (string.IsNullOrWhiteSpace(obj.Url))
                    {
                        obj.Url = string.IsNullOrWhiteSpace(actionUrl) ? metaUrl : actionUrl;
                    }

                    try
                    {
                        var pis = mi.GetParameters();
                        List<object> args_need = new List<object>();
                        args_need.AddRange(args);
                        if (pis.Length != args_need.Count)
                        {
                            if ((pis.Length - args_need.Count) == 2 && pis[pis.Length - 2].ParameterType.IsByRef && mi.ReturnType == typeof(void))
                            {
                                bRefReturnType = true;
                                args_need.Add(res);
                                args_need.Add(true);

                                returnType = pis[pis.Length - 2].ParameterType;
                                if ((returnType.IsByRef || returnType.FullName.EndsWith("&")) && returnType.HasElementType)
                                {
                                    returnType = returnType.GetElementType();
                                }
                            }
                            else
                            {
                                var paramInfos = new string[pis.Length];
                                for (int i = 0; i < pis.Length; i++)
                                {
                                    paramInfos[i] = string.Format("[{0}] {2} {1}", i, pis[i].Name, pis[i].ParameterType.FullName);
                                }
                                var paramInfoStr = string.Join("\r\n", paramInfos);
                                throw new BizException("参数个数不正确！需要的参数列为：\r\n" + paramInfoStr + "\r\n注意WCF的WebService的参数中，int,long,DateTime等值类型和结构需要跟一个bool型的 true。");
                            }
                        }
                        args = args_need.ToArray();
                        object[] trueargs = new object[pis.Length];
                        var refOutParams = new List<Tuple<int, string>>();
                        for (int i = 0; i < pis.Length; i++)
                        {
                            if (pis[i].ParameterType.IsByRef || pis[i].IsOut)
                            {
                                if (pis[i].Name == "result")
                                    throw new Exception("ref/out 参数名不能为 “result”。");

                                refOutParams.Add(new Tuple<int, string>(i, pis[i].Name));
                            }

                            if (args[i] != null && args[i] is string)
                            {
                                try
                                {
                                    string text = (string)args[i];
                                    if (text.StartsWith(PREFIX_XMLSTR))
                                    {
                                        trueargs[i] = Utils.GetObjByXml(pis[i].ParameterType, text);
                                    }
                                    else if (text.StartsWith(PREFIX_BASE64STR))
                                    {
                                        trueargs[i] = Utils.Base64StrToObj(text.Substring(PREFIX_BASE64STR.Length));
                                    }
                                    else if (text.StartsWith(PREFIX_BYTESSTR) && pis[i].ParameterType == typeof(byte[]))
                                    {
                                        // 字符串前缀是"/9j/4AA"有多种可能：
                                        //  1. 本身就传字符串；（业务服务的参数签名为 string，此时业务服务自己进行base64处理传入，然后收到字符串再自己处理base64。）
                                        //  2. 可能是文件上传的 base64 byte[]；（业务服务的签名为 byte[]）
                                        // 因此此处还需要对类型进行约束。
                                        trueargs[i] = Convert.FromBase64String(text);
                                    }
                                    else if (pis[i].ParameterType == typeof(byte[]))
                                    {
                                        // 另外，所需参数为 byte[] 时，text 也可能是一串字符串，此时需要 json 反序列化。
                                        trueargs[i] = Convert.FromBase64String(text);
                                    }
                                    else
                                    {
                                        trueargs[i] = ConvertArg(args[i], pis[i].ParameterType);
                                    }
                                }
                                catch (Exception err)
                                {
                                    throw new BizException("参数转换失败，参数名：" + pis[i].Name + "，参数值：" + (string)args[i] + "，Type：" + pis[i].ParameterType.ToString()
                                        + "\n异常:" + err.GetInnerMessage());
                                }
                            }
                            else
                            {
                                if (pis[i].IsOut)
                                {
                                    trueargs[i] = args[i];
                                }
                                else
                                {
                                    trueargs[i] = ConvertArg(args[i], pis[i].ParameterType);
                                }
                            }

                            #region [- 保证 DataSet、DataTable 类型的参数可以正常序列化 -]
                            // 传入参数可能有 DataSet、DataTable
                            // Test case: 测试名称:	ImportLargeTable
                            //            测试全名: Tencent.OA.ASF.UnitTest.Major.LargeParameterTest.ImportLargeTable
                            //            测试源:	D:\works\FMPlatform_proj\trunk\EPO\VS_Solution\ASF.UnitTest\Tencent.OA.ASF.UnitTest.Major\BigTableTest.cs:第 16 行
                            if (trueargs[i] != null)
                            {
                                if (pis[i].ParameterType == typeof(DataTable))
                                {
                                    if (string.IsNullOrWhiteSpace(((DataTable)trueargs[i]).TableName))
                                    {
                                        ((DataTable)trueargs[i]).TableName = "Table_" + i;
                                    }
                                }
                                else if (pis[i].ParameterType == typeof(DataSet))
                                {
                                    var ds = ((DataSet)trueargs[i]);
                                    if (ds.Tables != null)
                                    {
                                        for (int j = 0; j < ds.Tables.Count; j++)
                                        {
                                            if (string.IsNullOrEmpty(ds.Tables[j].TableName))
                                            {
                                                ds.Tables[j].TableName = "Table_" + i + "_" + j;
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion
                        }

                        try
                        {
                            res = mi.Invoke(obj, trueargs);
                        }
                        catch (Exception ex)
                        {
                            Utils.WriteLog("webserviceclient", new string[] { metaUrl, methodname, Helper.GetItemJson(trueargs), "失败", ex.ToString() });

                            var bizEx = new BizException(ex.GetInnerMessage(), ex);
                            // HACK: UNKNOWN HTTP METHOD
                            bizEx.SetUrl(null, obj.Url);
                            throw bizEx;
                        }

                        if (refOutParams.Any())
                        {
                            var jo = JObject.FromObject(new { result = res });
                            foreach (var item in refOutParams)
                            {
                                jo.Add(item.Item2, JToken.FromObject(trueargs[item.Item1]));
                            }

                            res = jo;
                        }

                        if (bRefReturnType)
                        {
                            res = trueargs[trueargs.Length - 2];
                        }

                        for (int i = 0; i < pis.Length; i++)
                        {
                            if (pis[i].IsOut)
                            {
                                try
                                {
                                    args[i] = trueargs[i];
                                }
                                catch { }
                            }
                        }

                        Utils.WriteLog("webserviceclient", new string[] { metaUrl, methodname, Helper.GetItemJson(trueargs), "成功", Helper.GetItemJson(res) });

                        pis = null;
                        trueargs = null;
                        args_need = null;
                    }
                    finally
                    {
                        obj.Dispose();
                        obj = null;
                        mi = null;
                    }
                    break;
                }
            }

            if (bFound == false)
            {
                throw new ASFException("找不到服务或服务的方法：" + methodname);
            }
            else
            {
                returnType = null;
                return res;
            }
        }

        public static T InvokeWebService<T>(string actionUrl, string methodname, object[] args,
            string metaUrl = null,
            Dictionary<string, string> headers = null,
            string agent = null,
            int timeout = RequestHelper.DEFAULT_HTTP_TIMEOUT)
        {
            Type returnType;
            var obj = InvokeWebService(actionUrl, methodname, args, out returnType, metaUrl, headers, agent, timeout);
            if (obj == null)
            {
                return default(T);
            }
            // 从接口返回的 obj，可能是动态生成的 Assembly 中定义的类，需要重新转换为实际应该的类
            var json = Helper.GetItemJson(obj);
            return Helper.ParseJson<T>(json);
        }

        public static Assembly ResolveAssembly(string url, Action<CodeNamespace, CompilerParameters, CSharpCodeProvider, CodeCompileUnit> onPreCompile = null)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url is null/empty.");

            if (!url.ToLower().StartsWith("http://") && !url.ToLower().StartsWith("https://"))
                throw new ArgumentException("Absolute url required: " + url);

            System.Web.Services.Discovery.DiscoveryClientProtocol dcc = new System.Web.Services.Discovery.DiscoveryClientProtocol();
            dcc.CookieContainer = new CookieContainer();
            dcc.Timeout = 100000;
            dcc.ConnectionGroupName = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
            dcc.DiscoverAny(url + "?WSDL&nonce=" + DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            dcc.ResolveAll();

            CompilerResults cr = null;
            try
            {
                CodeNamespace cn = new CodeNamespace();

                //生成客户端代理类代码
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                WebReference wr = new WebReference(dcc.Documents, cn);
                WebReferenceCollection wrc = new WebReferenceCollection();
                wrc.Add(wr);
                WebReferenceOptions opt = new WebReferenceOptions();
                opt.Style = ServiceDescriptionImportStyle.Client;
                //opt.CodeGenerationOptions = System.Xml.Serialization.CodeGenerationOptions.GenerateProperties
                //    | System.Xml.Serialization.CodeGenerationOptions.GenerateNewAsync
                //    | System.Xml.Serialization.CodeGenerationOptions.EnableDataBinding;
                var ProviderParam = new Dictionary<string, string>();
                ProviderParam.Add("CompilerVersion", "v4.0");
                CSharpCodeProvider csc = new CSharpCodeProvider(ProviderParam);
                ServiceDescriptionImporter.GenerateWebReferences(wrc, csc, ccu, opt);
                //设定编译参数
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;
#if GENERATE_AS_FILE
                var fname = url.ToLower()
                    .Replace("http://", "")
                    .Replace("https://", "")
                    .Replace("tencent_oa_", "");

                var ps = fname.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                var tmp = ps.Skip(ps.Length - 2).Take(2);
                fname = string.Join("-", tmp);
                cplist.OutputAssembly = string.Format("{0}\\asf-soap-{1}.{2}.dll", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), fname, Guid.NewGuid().ToString("n").Substring(0, 6));
#else
                cplist.GenerateInMemory = true;
#endif

                List<string> assemblies = new List<string>();

                #region [- 添加当前进程相关程序集 -]
                string path;
                if (AppDomain.CurrentDomain.ShadowCopyFiles)
                {
                    path = Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile) + "\\bin";
                }
                else
                {
                    path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                }
                // 原则上，Oracle.DataAccess.dll 不需要 OraOps11w.dll，但是测试环境的客户端版本过低，因此本地需要这个文件。
                // 而这个文件是非托管内存的，因此在编译WSDL的时候，不需要添加到引用程序集
                var ignoredAssemblies = new string[] { "OraOps11w.dll", "Tencent.OA.ASF.Common.dll" };
                var binDlls = System.IO.Directory.GetFiles(path, "*.dll", System.IO.SearchOption.TopDirectoryOnly);
                foreach (var binDll in binDlls)
                {
                    if (ignoredAssemblies.Any(x => x.ToLower() == System.IO.Path.GetFileName(binDll).ToLower()))
                        continue;

                    assemblies.Add(binDll);
                }
                #endregion

                #region [- 添加默认程序集 -]
                var defaultAssemlies = new string[]{
                    "System.dll",
                    "System.Core.dll",
                    "System.Data.dll",
                    "System.Data.DataSetExtensions.dll",
                    "System.Xml.dll",
                    "System.Xml.Linq.dll",
                    "System.Web.Services.dll"
                };
                foreach (var defass in defaultAssemlies)
                {
                    assemblies.Add(defass);
                }
                #endregion

                cplist.ReferencedAssemblies.AddRange(assemblies.ToArray());

                // 执行自定义的编译前回调
                if (onPreCompile != null)
                { onPreCompile(cn, cplist, csc, ccu); }

                // 编译代理类
                cr = csc.CompileAssemblyFromDom(cplist, ccu);
                ccu = null;
                cn = null;
                wrc = null;
                wr = null;
                opt = null;
                if (true == cr.Errors.HasErrors)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }
                    throw new Exception(sb.ToString());
                }
                cr.TempFiles.Delete();
                var res = cr.CompiledAssembly;
                cr = null;
                return res;
            }
            finally
            {
                if (cr != null)
                {
                    if (cr.TempFiles != null)
                        cr.TempFiles.Delete();
                }

                dcc.Dispose();
                dcc = null;
            }
        }

        public static List<MethodInfo> GetMethods(string url, bool useCache = true)
        {
            var result = new List<MethodInfo>();
            var assembly = GetAssembly(url, useCache);
            Debug.Assert(assembly != null);

            var types = assembly.GetExportedTypes();
            if (types != null)
            {
                foreach (Type t in types)
                {
                    if (!t.IsClass || !t.IsPublic || !t.IsSubclassOf(typeof(System.Web.Services.Protocols.SoapHttpClientProtocol)))
                        continue;

                    var ms = t.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
                    if (ms != null && ms.Any())
                    {
                        // 去除自动生成的 BeginXXX、EndXXX 两个异步方法。
                        var tmp = from x in ms
                                  where x.ReturnType != typeof(IAsyncResult) && x.GetParameters().All(y => y.ParameterType != typeof(IAsyncResult))
                                  select x;

                        if (tmp != null && tmp.Any())
                            result.AddRange(tmp);
                    }
                }
            }
            return result;
        }

        public static Type[] GetTypes(string url, bool useCache = true)
        {
            var result = new List<MethodInfo>();
            var assembly = GetAssembly(url, useCache);
            Debug.Assert(assembly != null);

            var types = assembly.GetExportedTypes();
            return types ?? new Type[0];
        }

        /// <summary>
        /// 考察当前类型是否为被 SOAP Client Code 定义为数组的 Dictionary
        /// </summary>
        /// <returns></returns>
        private static bool SeemsLikeDictionary(Type targetType, out Type elType, out Type keyType, out Type valueType)
        {
            // Dictionary<string, object> 会被 SOAP Client Code 定义为 ArrayOfKeyValueOfstringanyTypeKeyValueOfstringanyType[]

            if (targetType != null
                && targetType.IsArray
                && targetType.FullName.StartsWith("ArrayOfKeyValueOf") && targetType.FullName.EndsWith("[]")
                && targetType.HasElementType
                )
            {
                // ArrayOfKeyValueOfstringanyTypeKeyValueOfstringanyType
                elType = targetType.GetElementType();

                var binding = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
                var keyProp = (MemberInfo)elType.GetField("Key", binding) ?? elType.GetProperty("Key", binding);
                var valueProp = (MemberInfo)elType.GetField("Value", binding) ?? elType.GetProperty("Value", binding);
                if (keyProp != null && valueProp != null)
                {
                    keyType = keyProp is FieldInfo ? ((FieldInfo)keyProp).FieldType : ((PropertyInfo)keyProp).PropertyType;
                    valueType = valueProp is FieldInfo ? ((FieldInfo)valueProp).FieldType : ((PropertyInfo)valueProp).PropertyType;
                    return true;
                }
            }

            elType = null;
            keyType = null;
            valueType = null;
            return false;
        }

        public static Type GetTypeInfo(string url, string targetType)
        {
            var assembly = GetAssembly(url, true);
            Debug.Assert(assembly != null);

            var t = assembly.GetType(targetType, false, true);
            return t;
        }

        private static Assembly GetAssembly(string metaUrl, bool useCache)
        {
            if (string.IsNullOrEmpty(metaUrl))
                throw new ArgumentException("url is null/empty.");

            Func<Assembly> doResolve = () =>
            {
                lock (metaUrl)
                {
                    Assembly assembly = null;

                    if (useCache)
                    {
                        if (cacheAssembly.ContainsKey(metaUrl))
                            assembly = cacheAssembly[metaUrl];
                    }

                    if (assembly == null)
                    {
                        assembly = ResolveAssembly(metaUrl, (cn, cplist, csc, ccu) =>
                        {
                            // ASF 通过此回调改变WebService客户端代码的生成逻辑，将服务的客户端类的父类指定为 ContextedSoapHttpClientProtocol.

                            // 添加对 Tencent.OA.ASF.Proxy.SOAP12Invoker.dll 的引用
                            var invokerFile = Assembly.GetExecutingAssembly().Location;
                            cplist.ReferencedAssemblies.Add(invokerFile);

                            if (cn.Types != null)
                            {
                                // 为每个服务类指定 ContextedSoapHttpClientProtocol 为父类（替换原有默认的 SoapHttpClientProtocol）
                                foreach (CodeTypeDeclaration wsType in cn.Types)
                                {
                                    var old = wsType.BaseTypes.Cast<CodeTypeReference>().FirstOrDefault(x => x != null && x.BaseType == typeof(SoapHttpClientProtocol).ToString());
                                    if (old != null)
                                    {
                                        wsType.BaseTypes.Remove(old);
                                        wsType.BaseTypes.Add(typeof(ContextedSoapHttpClientProtocol).ToString());
                                    }
                                }
                            }
                        });
                        //it is thread safe for just assign use this form
                        cacheAssembly[metaUrl] = assembly;
                    }

                    return assembly;
                }
            };

            var rst = L.WithLog(LOGSOURCE, doResolve, true,
                useCache ? null : (Action<string, string, object[]>)L.Info, (s) => string.Format("Got {0} at {1}, cache: {2}.", s, metaUrl, useCache),
                L.Warn, (e) => string.Format("Getting assembly failed at {0}: {1}, cache: {2}.", metaUrl, e.GetInnerMessage(), useCache));
            return rst;

        }

        private static object ConvertArg(object arg, Type targetType)
        {
            if (arg == null)
                return arg;

            Type argType = null, elType, keyType, valueType;

            if (((argType = arg.GetType()) != null) && (
                    argType.IsPrimitive
                    || argType == typeof(string)
                    || argType == typeof(DateTime)
                    || argType == typeof(DateTimeOffset)))
            {
                var pt = Nullable.GetUnderlyingType(targetType) ?? targetType;
                return arg.ChangeType(pt);
            }
            else if (SeemsLikeDictionary(targetType, out elType, out keyType, out valueType)) // 注意，这里只处理了 targetType 本身是 Dictionary 的情况
            {
                if (arg is JObject)
                {
                    var list = ((JObject)arg).ToObject<Dictionary<object, object>>(DictionaryKeyValuePairCompatibleSerializer);
                    if (list == null)
                        return null;

                    var result = Array.CreateInstance(elType, list.Count);
                    if (list.Count > 0)
                    {
                        var idx = 0;
                        foreach (var item in list)
                        {
                            var props = new List<KeyValuePair<string, object>>();
                            props.Add(new KeyValuePair<string, object>("Key", ConvertArg(item.Key, keyType)));
                            props.Add(new KeyValuePair<string, object>("Value", ConvertArg(item.Value, valueType)));
                            var ins = Activator.CreateInstance(elType);
                            ins.SetProperties(props, elType, ignoreCase: true);
                            result.SetValue(ins, idx++);
                        }
                    }

                    return result;
                }
                else
                {
                    var list = ((JArray)arg).ToObject(targetType);
                    return list;
                }
            }
            else if (arg is JArray)
            {
                return ((JArray)arg).ToObject(targetType);
            }
            else if (arg is JObject)
            {
                return ((JObject)arg).ToObject(targetType);
            }
            else
            {
                return arg;
            }
        }

    }
}
