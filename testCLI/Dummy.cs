using System;
using Stardust.Nucleus;
using Stardust.Particles;

namespace test
{
    internal class Dummy : IBlueprint
    {
        public Type LoggingType => typeof(LoggingDefaultImplementation);

        public void Bind(IConfigurator Resolver)
        {
            Resolver.Bind<IDummyService>().To<DummyServiceImplementation>().SetRequestResponseScope();
        }
    }
}