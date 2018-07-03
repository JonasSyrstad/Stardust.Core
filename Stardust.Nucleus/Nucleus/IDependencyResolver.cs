using System;
using System.Collections.Generic;
using Stardust.Nucleus.ContextProviders;

namespace Stardust.Nucleus
{
    /// <summary>
    /// Provides a common contract for integrating DI controllers into Stardust
    /// </summary>
    public interface IDependencyResolver : IDisposable, Particles.Abstractions.IDependencyResolver
    {
        object GetInstance(Type type, Scope scope);
        
        T GetService<T>(string named, Action<T> initializer);

        IExtendedScopeProvider BeginExtendedScope(IExtendedScopeProvider scope);

        Dictionary<string, T> GetServicesNamed<T>(string exceptWithName);

        Dictionary<string, object> GetServicesNamed(Type serviceType);
    }
}