using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.DataAccess.Attributes
{
    /// <summary>
    /// 主键字段  
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class PrimaryKeyAttribute : Attribute
    {
        public PrimaryKeyAttribute()
        {
        }

        public PrimaryKeyAttribute(string name)
        {
            _name = name;
        }
        private string _name; public virtual string Name { get { return _name; } set { _name = value; } }
    }
}
