using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Base.Utility
{
    public class PaymentGatewayFactory
    {

        public PaymentGateway CreatePaymentGatewayByReflect(string paymentName)
        {
            string namespaceStr = "Base.Utility";
            string tempChar = "."; 
            Type type = Type.GetType(namespaceStr + tempChar + paymentName, true);
            ConstructorInfo ci = type.GetConstructor(System.Type.EmptyTypes);
            return (PaymentGateway)ci.Invoke(null);

        }
    }
}
