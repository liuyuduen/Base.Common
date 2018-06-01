using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Data.Common;
using System.Data;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Base.Utility
{
    /// <summary>
    /// 转换Json格式帮助类
    /// </summary>
    public static class JsonHelper
    {
        public static object ToJson(this string Json)
        {
            return JsonConvert.DeserializeObject(Json);
        }
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }


        public static DataTable JsonToDataTable(this string strJson)
        {
            #region
            DataTable tb = null;
            //获取数据  
            Regex rg = new Regex(@"(?<={)[^}]+(?=})");
            MatchCollection mc = rg.Matches(strJson);
            for (int i = 0; i < mc.Count; i++)
            {
                string strRow = mc[i].Value;
                string[] strRows = strRow.Split(',');
                //创建表  
                if (tb == null)
                {
                    tb = new DataTable();
                    tb.TableName = "Table";
                    foreach (string str in strRows)
                    {
                        DataColumn dc = new DataColumn();
                        string[] strCell = str.Split(':');
                        dc.DataType = typeof(String);
                        dc.ColumnName = strCell[0].ToString().Replace("\"", "").Trim();
                        tb.Columns.Add(dc);
                    }
                    tb.AcceptChanges();
                }
                //增加内容  
                DataRow dr = tb.NewRow();
                for (int r = 0; r < strRows.Length; r++)
                {
                    object strText = strRows[r].Split(':')[1].Trim().Replace("，", ",").Replace("：", ":").Replace("/", "").Replace("\"", "").Trim();
                    if (strText.ToString().Length >= 5)
                    {
                        if (strText.ToString().Substring(0, 5) == "Date(")//判断是否JSON日期格式
                        {
                            strText = StringHelper.JsonToDateTime(strText.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                    dr[r] = strText;
                }
                tb.Rows.Add(dr);
                tb.AcceptChanges();
            }
            return tb;
            #endregion
        }

        public static List<T> JonsToList<T>(this string Json)
        {
            return JsonConvert.DeserializeObject<List<T>>(Json);
        }
        public static T JsonToEntity<T>(this string Json)
        {
            return JsonConvert.DeserializeObject<T>(Json);
        }

        /// <summary>
        /// 反序列化，支持对 private set 的属性赋值。
        /// </summary>
        /// <param name="json"></param>
        /// <param name="targetType"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static object JsonToEntity(string json, Type targetType, JsonSerializerSettings settings = null)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            if (targetType == null)
                throw new ArgumentNullException("targetType");

            // 若传入 K-V 数组形式，并且需要反序列化为 Dictionary 时，提示应当反序列化为 List.
            if (targetType.GetInterface(typeof(System.Collections.IDictionary).FullName) != null && json.Trim().StartsWith("["))
            {
                throw new Exception("The target type must be List<KeyValuePair<TKey, TValue>> rather than Dictionary<TKey,TValue> while the input json is an Array of K-V pairs. e.g.([{\"Key\":\"\",\"Value\":\"\"}]).");
                /*
                // System.Collections.Generic.Dictionary`2[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
                // System.Collections.Generic.KeyValuePair`2[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
                // System.Collections.Generic.List`1[[System.Collections.Generic.KeyValuePair`2[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
                //var targetTypeFullName = targetType.FullName;
                //var kvTypeFullName = targetType.FullName.Replace("System.Collections.Generic.Dictionary", "System.Collections.Generic.KeyValuePair");
                //var listTypeFullName = "System.Collections.Generic.List`1[[" + kvTypeFullName + "]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]";
                //var listType = Type.GetType(listTypeFullName);

                var genericArgumentTypes = targetType.GetGenericArguments();
                //var instanceofKey = Activator.CreateInstance(genericArgumentTypes[0]);
                //var instanceofValue = Activator.CreateInstance(genericArgumentTypes[1]);
                //var obj = new KeyValuePair(instanceofKey, instanceofValue);

                var listType = typeof(List<KeyValuePair<object, object>>);
                var list = (List<KeyValuePair<object, object>>)ParseJson(json, listType, settings);
                if (list != null)
                {
                    Func<object, Type, object> changeType = (obj, targetT) =>
                    {
                        try
                        {
                            return Convert.ChangeType(obj, targetT);
                        }
                        catch
                        {
                            return ((JObject)obj).ToObject(targetT);
                        }
                    };

                    return list.ToDictionary(x => changeType(x.Key, genericArgumentTypes[0]), x => changeType(x.Value, genericArgumentTypes[1]));
                }
                return list;
                */
            }

            JsonSerializerSettings jss = null;
            if (settings == null)
            {
                jss = JsonHelper.CreateJsonSerializerSettings();
                // 在反序列化时，允许对私有成员赋值
                // 注意：此处和 GetItemJson 需要的 Settings 不一样。而在序列化时，不应当序列化私有成员。若允许访问私有字段，会导致序列化的字符串很长，冗余信息，内存占用暴涨。
                jss.ContractResolver = new IncludePrivateStateContractResolver()
                {
                    IgnoreSerializableAttribute = true,
                    IgnoreSerializableInterface = true
                };
            }

            object result;
            using (StringReader stringReader = new StringReader(json))
            {
                JsonSerializer jsonSerializer = JsonSerializer.Create(jss);
                jsonSerializer.Error += new EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs>((s, e) =>
                {
                    // 确保在缺失属性/字段时，直接默认为null，而不是抛出异常
                    if (e.ErrorContext.Error is JsonSerializationException && e.ErrorContext.Error.Message.StartsWith("Required property "))
                    {
                        e.ErrorContext.Handled = true;
                    }
                    else
                    {

                    }
                });
                result = jsonSerializer.Deserialize(stringReader, targetType);
            }
            return result;
        }


        public static object GetObject(this string content, Type targetType)
        {
            if (string.IsNullOrEmpty(content))
                throw new ArgumentNullException("httpResponse");

            if (targetType == null)
                throw new ArgumentNullException("targetType");

            JObject jsonObject = JObject.Parse(content);
            if (jsonObject == null || jsonObject["Code"] == null || jsonObject["Data"] == null)
                throw new Exception(string.Format("Unexpected response: {0}", content));

            var isGZip = false;
            if (jsonObject["GZip"] != null)
            {
                var zipString = jsonObject["GZip"].ToString();
                bool zip;
                if (bool.TryParse(zipString, out zip))
                {
                    isGZip = zip;
                }
                else
                {
                    throw new Exception(string.Format("Unexpected gzip: {0}", zipString));
                }
            }

            string data = null;
            if (isGZip)
            {
                if (jsonObject["Data"] != null)
                {
                    var bytes = JsonHelper.JsonToEntity<byte[]>(jsonObject["Data"].ToObject<string>());
                    Debug.Assert(bytes != null);
                    bytes = CompressHelper.Compress(bytes);
                    data = ByteHelper.BytesToString(bytes);
                }
            }
            else
            {
                if (jsonObject["Data"] != null)
                {
                    data = jsonObject["Data"].ToObject<string>();
                }
            }

            if (targetType == typeof(void) || string.IsNullOrEmpty(data))
            {
                return null;
            }
            else
            {
                return JsonHelper.JsonToEntity(data, targetType);
            }
        }



        public static JsonSerializerSettings CreateJsonSerializerSettings(IEnumerable<JsonConverter> converters = null)
        {
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            jss.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
            jss.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            jss.NullValueHandling = NullValueHandling.Include;
            jss.MissingMemberHandling = MissingMemberHandling.Ignore;
            jss.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;
            jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            jss.PreserveReferencesHandling = PreserveReferencesHandling.None;
            jss.Formatting = Formatting.None;
            jss.ContractResolver = new DefaultContractResolver()
            {
                IgnoreSerializableAttribute = true,
                IgnoreSerializableInterface = true
            };
            jss.Converters.Add(new ValueTypeConverter());
            if (converters != null)
            {
                foreach (var c in converters)
                {
                    if (c is ValueTypeConverter)
                        continue;

                    jss.Converters.Add(c);
                }
            }

            return jss;
        }
    }
}
