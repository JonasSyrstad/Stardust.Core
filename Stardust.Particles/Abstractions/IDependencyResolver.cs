using System;
using System.Collections.Generic;
using System.Text;

namespace Stardust.Particles.Abstractions
{
    public interface IDependencyResolver : IServiceProvider
    {
        T GetService<T>();
        T GetService<T>(Action<T> initializer);

        T GetService<T>(string named);


        T[] GetServices<T>();

        object GetService(Type serviceType, string named);

        IEnumerable<object> GetServices(Type serviceType);
    }
}
