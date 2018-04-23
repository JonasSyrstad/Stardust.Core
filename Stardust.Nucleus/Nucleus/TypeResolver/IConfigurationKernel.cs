using System;
using System.Collections.Generic;
using Stardust.Nucleus.ContainerIntegration;

namespace Stardust.Nucleus.TypeResolver
{
    public interface IConfigurationKernel
    {
        IScopeContext Resolve(Type type, string named, IDependencyResolver resolver,
            bool skipAlternateResolving = false);

        IEnumerable<IScopeContext> ResolveAll(Type type);
        void Bind(Type concreteType, IScopeContext existingBinding, string identifier);
        void Unbind(Type type, string identifier);
        void UnbindAll(Type type);

        void Unbind(Type type, IScopeContext scopeContext, string identifier);
        IEnumerable<KeyValuePair<string, string>> ResolveList(Type type);

        IDictionary<string, IScopeContext> ResolveAllNamed(Type type);
        /// <summary>
        /// </summary>
        void UnbindAll();
    }
}