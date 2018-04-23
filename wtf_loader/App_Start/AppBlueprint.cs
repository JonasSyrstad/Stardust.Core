



using System.Collections.Specialized;
using System.Configuration;
using Stardust.Interstellar.Rest.Annotations;
using Stardust.Interstellar.Rest.Client;
using wtf_loader.Controllers;

namespace Thrillout.Web.Ui
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Stardust.Nucleus;
    using Stardust.Nucleus.Web;
    using Stardust.Particles;

    [IocConfiguration]//Remove this and call Resolver.LoadModuleConfiguration<AppBlueprint>(); from Global.asax Application_Start if you experience problems with early loading of the ioc bindings
    public partial class AppBlueprint : BlueprintBase<NullLogger>
    {
        static AppBlueprint()
        {
            ConfigurationManagerHelper.SetManager(new ConfigReader());
            //var s = new StardustApiControllerActivator();
        }
        partial void LoadConfiguration(IConfigurator configuration)
        {
            configuration.Bind<IDummy>().To<Dummy>().SetTransientScope();
            configuration.Bind<IDummyService>()
                .ToConstructor(s => new ServiceLocator(s).CreateRestClient<IDummyService>(""))
                .SetRequestResponseScope();
            configuration.Bind<IHelper>().To<Helper>().SetRequestResponseScope();
            //configuration.BindWebApiController<IProfileService>()
            //    .ToConstructor(() => CreateProxy<IProfileService>("profile.service.url"));
            //configuration.Bind<IProfileService>().ToConstructor(() => CreateProxy<IProfileService>("profile.service.url"));
        }

        //private static T CreateProxy<T>(string urlkey)
        //{
        //    return ProxyFactory.CreateInstance<T>(ConfigurationManagerHelper.GetValueOnKey(urlkey));
        //}
    }
    [IRoutePrefix("api")]
    public interface IDummyService
    {
        [IRoute("profile")]
        [Get]
        Task<string> GetProfile();
    }

    public class NullLogger : ILogging
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

    public class ConfigReader : IConfigurationReader
    {
        public NameValueCollection AppSettings { get { return ConfigurationManager.AppSettings; } }
    }
}
