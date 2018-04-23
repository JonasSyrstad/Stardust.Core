using System;
using System.Collections.Generic;
using Stardust.Nucleus.ContainerIntegration;
using Stardust.Nucleus.TypeResolver;

namespace Stardust.Nucleus.Internals
{
    internal class Configurator : IInternalConfigurator
    {
        private readonly IConfigurationKernel kernel;
        private IDependencyResolver _resolver => Items["resolver"] as IDependencyResolver;

        internal Configurator(IConfigurationKernel kernel)
        {
            this.kernel = kernel;
            Items = new Dictionary<string, object> { { "resolver", Resolver.CreateScopedResolver() } };
            ((IInternalConfigurator) this).Bind<IDependencyResolver>().To(_resolver.GetType())
                .SetRequestResponseScope();
            //((IInternalConfigurator) this).Bind<IDependencyResolver>().To(_resolver.GetType())
            //    .SetRequestResponseScope();
        }

        IBindContext<T> IConfigurator.Bind<T>()
        {
            return new BindContext<T>(kernel, _resolver);
        }

        IBindContext IConfigurator.Bind(Type type)
        {
            return new BindContext(kernel, type, _resolver);
        }

        IBindContext IConfigurator.BindAsGeneric(Type genericUnboundType)
        {
            return new BindContext(kernel, genericUnboundType, true, _resolver);
        }

        void IConfigurator.RemoveAll()
        {
            kernel.UnbindAll();
        }

        IUnbindContext<T> IConfigurator.UnBind<T>()
        {
            return new UnbindContext<T>(kernel, _resolver);
        }

        public Dictionary<string, object> Items { get; }
    }
}