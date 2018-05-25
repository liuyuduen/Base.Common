using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace WcfServiceLibrary1
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“Service1”。
    public class HomeService : IHomeService
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public int GetLength(string name)
        {
            return name.Length;
        }

        public Message Update(Message message)
        {
            var header = message.Headers;

            var ip = header.GetHeader<string>("ip", string.Empty);

            var currentTime = header.GetHeader<string>("currenttime", string.Empty);

            //这个就是牛逼的 统计信息。。。
            Console.WriteLine("客户端的IP=" + ip + " 当前时间=" + currentTime);

            return Message.CreateMessage(message.Version, message.Headers.Action + "Response", "等我吃完肯德基，再打死你这个傻逼！！！");
        }
    }
}
