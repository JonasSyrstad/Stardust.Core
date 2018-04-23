using Stardust.Nucleus;

namespace test
{
    internal class DummyServiceImplementation : IDummyService
    {
        private readonly IDependencyResolver _ioc;

        public DummyServiceImplementation(IDependencyResolver ioc)
        {
            _ioc = ioc;
        }
        public string GetMessage()
        {
            return "Hello .net core";
        }
    }
}