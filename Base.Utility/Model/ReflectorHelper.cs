using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Web;

namespace Base.Utility
{

    public static class ReflectorTool
    {
        public static T CreateInstance<T>(string typeName)
        {
            return CreateInstance<T>(typeName, true);
        }

        public static T CreateInstance<T>(string typeName, bool cacheable)
        {
            return CreateInstance<T>(Type.GetType(typeName), cacheable);
        }

        public static T CreateInstance<T>(Type type, bool cacheable)
        {
            T local = default(T);
            if (cacheable)
            {
                local = (T)HttpRuntime.Cache[type.FullName];
            }
            if (local == null)
            {
                if (type != null)
                {
                    local = (T)Activator.CreateInstance(type);
                }
                if (!cacheable || (local == null))
                {
                    return local;
                }
                string fullName = type.FullName;
                Cache cache = HttpRuntime.Cache;
                if (cache[fullName] != null)
                {
                    cache.Remove(fullName);
                }
                cache.Insert(fullName, local, null, DateTime.Now.AddHours(1.0), TimeSpan.Zero);
            }
            return local;
        }


    }
}
