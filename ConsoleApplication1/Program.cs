using Base.Sample.Host;
using Base.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using WcfServiceLibrary1;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestBase();

            // TestWcf();

            TestWcfMessage();
        }


        #region 基础


        public void TestBase()
        {
            var baseurl = ConfigHelper.AppSettings("baseurl");
            var endpoindurl_basic = ConfigHelper.AppSettings("endpoindurl_basic");
            BasicWcf(baseurl, endpoindurl_basic);
            RunBasicWcf(endpoindurl_basic);

            //var endpoindurl_tcp = ConfigHelper.AppSettings("endpoindurl_tcp");
            // TcpWcf(baseurl, endpoindurl_tcp); 
            // RunTcpWcf(endpoindurl_basic1);

        }

        /// <summary>
        /// tcp服务 
        /// </summary>
        public static void BasicWcf(string baseurl, string endpoindurl_basic)
        {
            ServiceHost host = new ServiceHost(typeof(HomeService), new Uri(baseurl));

            host.AddServiceEndpoint(typeof(IHomeService), new BasicHttpBinding(), endpoindurl_basic);

            //公布元数据
            host.Description.Behaviors.Add(new ServiceMetadataBehavior() { HttpGetEnabled = true });
            host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

            host.Open();

            Console.WriteLine("BASIC服务已经开启。。。");

        }



        /// <summary>
        /// tcp服务 
        /// </summary>
        public static void TcpWcf(string baseurl, string endpoindurl_tcp)
        {
            ServiceHost host = new ServiceHost(typeof(HomeService), new Uri(baseurl));

            host.AddServiceEndpoint(typeof(IHomeService), new NetTcpBinding(), endpoindurl_tcp);

            //公布元数据
            host.Description.Behaviors.Add(new ServiceMetadataBehavior() { HttpGetEnabled = true });
            host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

            host.Open();

            Console.WriteLine("TCP服务已经开启。。。");

        }




        /// <summary>
        /// http://localhost:1920/homeservice
        /// </summary>
        /// <param name="baseurl"></param>
        public static void RunBasicWcf(string endpoindurl_basic)
        {

            ChannelFactory<IHomeService> factory = new ChannelFactory<IHomeService>(new BasicHttpBinding(), endpoindurl_basic);

            var channel = factory.CreateChannel();

            var result = channel.GetLength("12345");


            Console.WriteLine("执行BASIC方法，获取长度：" + result);

            Console.Read();
        }

        /// <summary>
        /// net.tcp://localhost:1920/homeservice
        /// </summary>
        public static void RunTcpWcf(string endpoindurl_tcp)
        {

            ChannelFactory<IHomeService> factory = new ChannelFactory<IHomeService>(new NetTcpBinding(), endpoindurl_tcp);

            var channel = factory.CreateChannel();

            var result = channel.GetLength("12345");


            Console.WriteLine("执行TCP方法，获取长度：" + result);

            Console.Read();
        }
        #endregion


        #region 加入配置
        public static void TestWcf()
        {
            var baseurl1 = ConfigHelper.AppSettings("baseurl1");
            var endpoindurl_basic1 = ConfigHelper.AppSettings("endpoindurl_basic1");
            ServiceHostHelper<IProviderInterface, ProviderInterface>.GetServiceHost(baseurl1, endpoindurl_basic1);


            var obj = ServiceChannelHelper<IProviderInterface>.GetMethod(endpoindurl_basic1);
            var result = obj.GetLength("abcdefg");
            Console.WriteLine("执行TCP方法，获取长度：" + result);

            Console.Read();
        }
        #endregion

        public static void TestWcfMessage()
        {
            BasicHttpBinding bingding = new BasicHttpBinding();

            BindingParameterCollection param = new BindingParameterCollection();

            var u = new Test() { str = "王八蛋" };

            Message request = Message.CreateMessage(MessageVersion.Soap11, "http://tempuri.org/IHomeService/Update", u);

            //在header中追加ip信息
            request.Headers.Add(MessageHeader.CreateHeader("ip", string.Empty, Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString()));
            request.Headers.Add(MessageHeader.CreateHeader("currenttime", string.Empty, DateTime.Now));

            IChannelFactory<IRequestChannel> factory = bingding.BuildChannelFactory<IRequestChannel>(param);

            factory.Open();

            var baseurl = ConfigHelper.AppSettings("baseurl");
            //"http://192.168.1.105:19200/HomeServie"
            IRequestChannel channel = factory.CreateChannel(new EndpointAddress(baseurl));

            channel.Open();

            var result = channel.Request(request);

            channel.Close();

            factory.Close();
        }
    }
    [DataContract(Namespace = "http://tempuri.org/", Name = "Update")]
    class Test
    {
        [DataMember]
        public string str { get; set; }
    }
}
