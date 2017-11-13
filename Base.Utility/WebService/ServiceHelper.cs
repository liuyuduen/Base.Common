using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Base.Utility
{
    /// <summary>
    /// WCF Head的操作
    /// </summary>
    public class ServiceHelper
    {

        public static string GetHeaderValue(string name, string ns)
        {
            return GetHeaderValue<string>(name, ns);
        }
        public static T GetHeaderValue<T>(string name, string ns)
        {
            MessageHeaders headers = OperationContext.Current.IncomingMessageHeaders;
            int intIndex = headers.FindHeader(name, ns);
            if (intIndex > -1) return headers.GetHeader<T>(intIndex);
            return default(T);
        }
    }
}
