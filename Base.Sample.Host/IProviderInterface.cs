using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Sample.Host
{
    [System.ServiceModel.ServiceContract]
    public interface IProviderInterface
    {

        [System.ServiceModel.OperationContract]
        [System.ServiceModel.Web.WebInvoke(Method = "POST")]
        int GetLength(string name);
    }

}
