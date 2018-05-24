using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeaRun.Utilities.Base.Extension
{
    public class ICustomer
    {
        public string CustomerId { get; set; }

        public string UserName { get; set; }

        public decimal AccountMoney { get; set; }

        public double PositionBackPoint { get; set; }

        public double? NonPositionBackPoint { get; set; }
    }
}
