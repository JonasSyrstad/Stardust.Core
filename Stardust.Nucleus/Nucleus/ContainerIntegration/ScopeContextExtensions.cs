using System.Collections.Generic;
using Stardust.Particles.Collection;

namespace Stardust.Nucleus.ContainerIntegration
{
    internal static class ScopeContextExtensions
    {
        internal static T[] ActivateAll<T>(this IEnumerable<IScopeContext> contexts)
        {
            if (contexts.IsEmpty()) return new T[0];
            var list = new List<T>();
            foreach (var scopeContext in contexts)
            {
                list.Add((T)scopeContext?.Activate());
            }
            return list.ToArray();
        }
    }
}
