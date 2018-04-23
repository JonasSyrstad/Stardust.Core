using System;
using System.Collections.Specialized;
using System.Configuration;
using Stardust.Particles;
using Xunit;

namespace Stardust.Nucleus.Tests
{
    public class Class1
    {
        static Class1()
        {
            ConfigurationManagerHelper.SetManager(new Manager());
            Resolver.LoadModuleConfiguration<BlueprintBase>();
        }

        [Fact]
        public void ResolverTest()
        {
            ITestService item;
            using (var scope = Resolver.CreateScopedResolver())
            {
                item = scope.GetService<ITestService>();
                Assert.NotNull(item);
                Assert.Equal(item.Self.InstanceId, item.InstanceId);

            }
            Assert.True(item.IsDisposed);
        }

        [Fact]
        public void Resolver2Test()
        {
            ITestService item;
            using (var scope = Resolver.CreateScopedResolver())
            {
                item = scope.GetService<ITestService>("test");
                Assert.NotNull(item);
                Assert.Equal(item.Self.InstanceId, item.InstanceId);

            }
            Assert.True(item.IsDisposed);
        }

        [Fact]
        public void Resolver3Test()
        {

            ITestService item;
            using (var scope = Resolver.CreateScopedResolver())
            {
                item = scope.GetService<ITestService>();
                Assert.NotNull(item);
                Assert.Equal(item.Self.InstanceId, item.InstanceId);
                var item2 = scope.GetService<ITestService>("test");
                Assert.NotNull(item2);
                Assert.NotEqual(item2.InstanceId, item.InstanceId);

            }
            using (var scope = Resolver.CreateScopedResolver())
            {
                var item2 = scope.GetService<ITestService>();
                Assert.NotNull(item);
                Assert.NotEqual(item2.InstanceId, item.InstanceId);

            }
            Assert.True(item.IsDisposed);
        }

        public class BlueprintBase : IBlueprint
        {
            public void Bind(IConfigurator configuration)
            {


                LoggingExtentions.SetLogger(typeof(Logger));
                BindWebActivator(configuration);
                ConfigureIoc(configuration);


            }

            private void ConfigureIoc(IConfigurator configuration)
            {
                configuration.Bind<ITestService>().To<TestService>().SetRequestResponseScope();
                configuration.Bind<ITestService>().ToConstructor(s => new TestService2(s), "test")
                    .SetRequestResponseScope();

            }


            private void BindWebActivator(IConfigurator configuration)
            {
                var c = configuration as IInternalConfigurator;
                var di = c?.Items["resolver"] as IDependencyResolver;

            }

            public Type LoggingType => typeof(Logger);
        }
    }

    public class Manager : IConfigurationReader
    {
        public NameValueCollection AppSettings
        {
            get { return ConfigurationManager.AppSettings; }
        }
    }

    internal class TestService : ITestService
    {
        private readonly IDependencyResolver _resolver;

        public TestService(IDependencyResolver resolver)
        {
            InstanceId = Guid.NewGuid();
            _resolver = resolver;

        }
        public ITestService Self => _resolver.GetService<ITestService>();

        public Guid InstanceId { get; }
        public bool IsDisposed { get; private set; }

        public override int GetHashCode()
        {
            return InstanceId.GetHashCode();
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    internal class TestService2 : ITestService
    {
        private readonly IDependencyResolver _resolver;

        public TestService2(IDependencyResolver resolver)
        {
            InstanceId = Guid.NewGuid();

        }
        public ITestService Self => this;

        public Guid InstanceId { get; }
        public bool IsDisposed { get; private set; }

        public override int GetHashCode()
        {
            return InstanceId.GetHashCode();
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    internal interface ITestService : IDisposable
    {
        ITestService Self { get; }
        Guid InstanceId { get; }
        bool IsDisposed { get; }
    }

    public class Logger : ILogging
    {
        public void Exception(Exception exceptionToLog, string additionalDebugInformation = null)
        {

        }

        public void HeartBeat()
        {
        }

        public void DebugMessage(string message, LogType entryType = LogType.Information, string additionalDebugInformation = null)
        {
        }

        public void SetCommonProperties(string logName)
        {
        }
    }
}
