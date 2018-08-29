using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace Base.Utility
{
    public class ServiceChannelHelper<T>
    {
        public static T GetMethod(string apiUrl)
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

            ChannelFactory<T> factory = new ChannelFactory<T>(bind, apiUrl);

            return factory.CreateChannel();
        }
    }
}
