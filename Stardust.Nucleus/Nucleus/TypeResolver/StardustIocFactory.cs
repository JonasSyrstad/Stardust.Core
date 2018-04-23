using System.Collections.Generic;
using Stardust.Nucleus.ContainerIntegration;
using Stardust.Nucleus.ContextProviders;
using Stardust.Nucleus.Internals;

namespace Stardust.Nucleus.TypeResolver
{
    internal class StardustIocFactory : IContainerSetup
    {
        public IConfigurator GetConfigurator(IConfigurationKernel configurationKernel)
        {
            return new Configurator(configurationKernel);
        }

        public IConfigurationKernel GetKernel()
        {
            return new TypeResolverConfigurationKernel(new TypeLocatorOptimizer(), null, new TypeResolverFromAssemblyScanning());
        }

        public IDependencyResolver GetResolver(IConfigurationKernel configurationKernel)
        {
            var resolver = new StardustDependencyResolver(() => configurationKernel);
            ContainerFactory.Current(resolver._scopeContext).Bind(typeof(IDependencyResolver), resolver, Scope.Context);
            ContainerFactory.Current(resolver._scopeContext).Bind(resolver.GetType(), resolver, Scope.Context);
            return resolver;
        }
    }
}