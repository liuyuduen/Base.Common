using System;
using System.Collections.Generic;
using System.Text;
using Castle.MicroKernel;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor;
using Base.Utility;

namespace Base.Utility
{
    public class CastleContainer
    {
        private IKernel kernel;
        public static readonly CastleContainer Instance = new CastleContainer();

        public CastleContainer()
        {
            //建立容器,并通过配置文件最动加入组件

            //Castle.Core.Resource.ConfigResource source = new Castle.Core.Resource.ConfigResource();
            //XmlInterpreter interpreter = new XmlInterpreter(source);
            //cs
            //WindsorContainer windsor = new WindsorContainer(interpreter);
            //bs
            WindsorContainer windsor = new WindsorContainer(PathHelper.GetRootPath() + "\\" + ConfigHelper.IocConfigPath);
            kernel = windsor.Kernel;
        }

        /// <summary>
        /// Returns a component instance by the type of service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>()
        {
            return (T)kernel[typeof(T)];
        }

        /// <summary>
        /// Returns a component instance by the service name.
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        private object Resolve(Type service)
        {
            return kernel[service];
        }

        /// <summary>
        /// Returns a component instance by the service name.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private object Resolve(String key)
        {
            return kernel[key];
        }

        /// <summary>
        /// Release resource that be container used.
        /// </summary>
        public void Dispose()
        {
            kernel.Dispose();
        }
    }
}
