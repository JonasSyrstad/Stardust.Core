using System;
using System.Collections.Generic;
using Stardust.Nucleus.ContainerIntegration;
using Stardust.Nucleus.ContextProviders;
using Stardust.Nucleus.Internals;
using Stardust.Particles;
using Stardust.Particles.Collection;

namespace Stardust.Nucleus.TypeResolver
{
    internal static class Helper
    {
        public static IScopeContext Convert(this IScopeContext item, IDependencyResolver resolver)
        {
            if (item is InternalScopeContext) return item;
            if (item == null) return null;
            return new InternalScopeContext((IScopeContextInternal) item,((StardustDependencyResolver)resolver)._scopeContext);
        }
    }
    /// <summary>
    /// The main entry point for type resolving, used by the Resolver only 
    /// </summary>
    internal sealed class TypeResolverConfigurationKernel : IConfigurationKernel
    {
        private readonly IOptimizer Optimizer;
        private readonly IAssemblyScanningTypeResolver AssemblyScanner;
        private readonly IConfigurationTypeResolver ConfigurationResolver;

        public TypeResolverConfigurationKernel(IOptimizer optimizer, IConfigurationTypeResolver configResolver, IAssemblyScanningTypeResolver assemblyScanner)
        {
            Optimizer = optimizer;
            AssemblyScanner = assemblyScanner;
            ConfigurationResolver = configResolver;
            assemblyScanner.ConfigurationKernel = this;
        }

        public IScopeContext Resolve(Type type, string named, IDependencyResolver resolver,
            bool skipAlternateResolving = false)
        {
            var item = Optimizer.FindSubtypeOf(type, named);
            if (item.IsInstance() || skipAlternateResolving) return item.Convert(resolver);
            item =ResolveFromConfigAndCache(type, named);
            if (item.IsInstance()) return item.Convert(resolver);
            item = LocateInLoadedAssembliesAndCache(type, named, item, resolver);
            return item.Convert(resolver);
        }

        public IEnumerable<IScopeContext> ResolveAll(Type type)
        {
            var items = Optimizer.GetAllSubClassesOf(type);
            if (items.IsNull())
            {
                items = LoadItemsFromConfig(type, items);
            }
            return items;
        }

        private IEnumerable<IScopeContext> LoadItemsFromConfig(Type type, IEnumerable<IScopeContext> items)
        {
            items = ConfigurationResolver?.GetTypeBindingsFromConfig(type);
            if (items.IsEmpty())
            {
                Optimizer.AddBaseTypeIfNonExisting(type);
            }
            else
            {
                foreach (var scopeContext in items)
                {
                    Optimizer.SetImplementationType(type, scopeContext, scopeContext.ImplementationKey);
                }
            }
            return items;
        }

        public void Bind(Type concreteType, IScopeContext existingBinding, string identifier)
        {
            Optimizer.SetImplementationType(concreteType, existingBinding, identifier);
        }

        public void Unbind(Type type, string identifier)
        {
            Optimizer.RemoveImplementation(type, identifier);
        }

        public void UnbindAll(Type type)
        {
            Optimizer.RemoveImplementationType(type);
        }

        public void Unbind(Type type, IScopeContext scopeContext, string identifier)
        {
            Optimizer.RemoveImplementationType(type, scopeContext, identifier);
        }

        public IEnumerable<KeyValuePair<string, string>> ResolveList(Type type)
        {
            return Optimizer.GetImplementationsOf(type);
        }

        public void UnbindAll()
        {
            Optimizer.RemoveAll();
        }

        private IScopeContext LocateInLoadedAssembliesAndCache(Type type, string named, IScopeContext item, IDependencyResolver resolver)
        {
            Logging.DebugMessage(string.Format("Type scanning initiated. Looking for implementations of {0} {1}", type.FullName, GetNameString(named)));
            var items = AssemblyScanner.LocateInLoadedAssemblies(type,resolver);
            foreach (var scopeContext in items)
            {
                Optimizer.SetImplementationType(type, scopeContext, scopeContext.ImplementationKey);
                if (scopeContext.ImplementationKey == named)
                    item = scopeContext;
            }
            return item;
        }

        private static string GetNameString(string named)
        {
            return named == TypeLocatorNames.DefaultName ? "as default" : string.Format("named '{0}'", named);
        }

        private IScopeContext ResolveFromConfigAndCache(Type type, string named)
        {
            IScopeContext item = null;
            var items = ConfigurationResolver?.GetTypeBindingsFromConfig(type);
            if (items == null) return item;
            foreach (var scopeContext in items)
            {
                Optimizer.SetImplementationType(type, scopeContext, scopeContext.ImplementationKey);
                if (scopeContext.ImplementationKey == named && named.IsInstance())
                    item = scopeContext;
            }
            return item;
        }

        public IDictionary<string, IScopeContext> ResolveAllNamed(Type type)
        {
            var items = Optimizer.GetAllSubClassesOfNamed(type);

            return items;
        }
    }
}