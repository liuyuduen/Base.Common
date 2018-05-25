using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;

namespace Base.Utility
{
    /// <summary>
    /// 服务寄宿类
    /// </summary> 
    /// <typeparam name="I">服务接口</typeparam>
    /// <typeparam name="T">服务接口实现类</typeparam>
    public class ServiceHostHelper<I, T>
    {
        public static void GetServiceHost(string baseurl, string endpoindurl)
        {
            Binding bind = null;

            switch (ConfigHelper.WebServiceBindingType)
            {
                case "basic":
                    bind = new BasicHttpBinding();
                    break;
                case "tcp":
                    bind = new NetTcpBinding();
                    break;
                default:
                    bind = new NetTcpBinding();
                    break;
            }
            ServiceHost host = new ServiceHost(typeof(T), new Uri(baseurl));
            host.AddServiceEndpoint(typeof(I), bind, endpoindurl);

            //公布元数据
            host.Description.Behaviors.Add(new ServiceMetadataBehavior() { HttpGetEnabled = true });
            host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
            host.Open();

            Console.WriteLine("WCF服务已经开启...");
        }
    }
}
