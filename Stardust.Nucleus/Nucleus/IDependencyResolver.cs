using System;
using System.Collections.Generic;
using Stardust.Nucleus.ContextProviders;

namespace Stardust.Nucleus
{
    /// <summary>
    /// Provides a common contract for integrating DI controllers into Stardust
    /// </summary>
    public interface IDependencyResolver : IDisposable
    {
        object GetInstance(Type type, Scope scope);
        T GetService<T>();
        T GetService<T>(Action<T> initializer);

        T GetService<T>(string named);
        T GetService<T>(string named, Action<T> initializer);


        T[] GetServices<T>();

        object GetService(Type serviceType, string named);

        IEnumerable<object> GetServices(Type serviceType);
        IExtendedScopeProvider BeginExtendedScope(IExtendedScopeProvider scope);
        Dictionary<string, T> GetServicesNamed<T>(string exceptWithName);
        Dictionary<string, object> GetServicesNamed(Type serviceType);
    }
}