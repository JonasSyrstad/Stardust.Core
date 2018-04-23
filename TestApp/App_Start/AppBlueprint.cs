



using System.Collections.Specialized;
using System.Configuration;
using Stardust.Interstellar.Rest.Extensions;

namespace Thrillout.Web.Ui
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Stardust.Interstellar.Rest.Annotations;
    using Stardust.Interstellar.Rest.Client;
    using Stardust.Nucleus;
    using Stardust.Nucleus.Web;
    using Stardust.Particles;

    [IocConfiguration]//Remove this and call Resolver.LoadModuleConfiguration<AppBlueprint>(); from Global.asax Application_Start if you experience problems with early loading of the ioc bindings
    public partial class AppBlueprint : BlueprintBase<LoggingDefaultImplementation>
    {
        static AppBlueprint()
        {
            ConfigurationManagerHelper.SetManager(new ConfigReader());
            //var s = new StardustApiControllerActivator();
        }
        partial void LoadConfiguration(IConfigurator configuration)
        {
            //configuration.BindWebApiController<IProfileService>()
            //    .ToConstructor(() => CreateProxy<IProfileService>("profile.service.url"));
            //configuration.Bind<IProfileService>().ToConstructor(() => CreateProxy<IProfileService>("profile.service.url"));
        }


    }

    public class ConfigReader : IConfigurationReader
    {
        public NameValueCollection AppSettings { get { return ConfigurationManager.AppSettings; } }
    }
}
