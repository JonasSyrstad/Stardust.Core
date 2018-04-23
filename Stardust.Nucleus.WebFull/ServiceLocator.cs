using System;
using System.Collections.Generic;
using Stardust.Interstellar.Rest.Extensions;
using Stardust.Nucleus.ContainerIntegration;
using Stardust.Nucleus.ObjectActivator;

namespace Stardust.Nucleus.Web
{
    public class ServiceLocator : IServiceLocator
    {
        private readonly IDependencyResolver _resolver;

        public ServiceLocator(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public T GetService<T>()
        {
            return _resolver.GetService<T>();
        }

        public object GetService(Type serviceType)
        {
            return _resolver.GetService(serviceType,"default");
        }

        public IEnumerable<T> GetServices<T>()
        {
            return _resolver.GetServices<T>();
        }

        public object CreateInstanceOf(Type type)
        {
            return _resolver.GetInstance(type, Scope.PerRequest);
        }

        public T CreateInstance<T>() where T : class
        {
            return _resolver.GetInstance(typeof(T), Scope.PerRequest) as T;
        }
    }
}