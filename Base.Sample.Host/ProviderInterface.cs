using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Sample.Host
{
    public class ProviderInterface : IProviderInterface
    {
        public int GetLength(string name)
        {
            return name.Length;
        }
    }
}
