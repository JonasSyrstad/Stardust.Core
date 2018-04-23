using Stardust.Nucleus;
using Stardust.Nucleus.ContextProviders;
using System;
using Xunit;

namespace test
{
    public class Class1
    {
        [Fact]
        public void ResolverTest()
        {
            var msg = new Object();
            Resolver.LoadModuleConfiguration<Dummy>();
            //using (ContextScopeExtensions.CreateScope())
            //{
            //    var instance = Resolver.Activate<IDummyService>();
            //    Assert.NotNull(instance);
            //}
        }
    }
}
