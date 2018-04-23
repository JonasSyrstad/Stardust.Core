using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Stardust.Nucleus.Web;
using Stardust.Nucleus.Extensions;
using Stardust.Particles;
using ConfigurationManager = System.Configuration.ConfigurationManager;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(BlueprintLoader), nameof(BlueprintLoader.Load))]
namespace Stardust.Nucleus.Web
{

    class BlueprintLoader
    {
        public static void Load()
        {
            ConfigurationManagerHelper.SetManager(new WebConfigManager());
            if (ConfigurationManager.AppSettings["disableAutoLoader"] == "true") return;
            if (AppDomain.CurrentDomain.GetAssemblies().Where(NotSystemAssembly).Any(LookForConfigurationImplementation))
            {
                //Logging.DebugMessage("IocConfig found");
            }
        }

        private static bool LookForConfigurationImplementation(Assembly assembly)
        {

            foreach (var type in assembly.GetTypes().Where(t => t.Implements<IBlueprint>()))
            {
                var blueprintLoader = type.GetCustomAttribute(typeof(IocConfigurationAttribute), true);
                if (blueprintLoader != null)
                {
                    Resolver.LoadModuleConfiguration((IBlueprint)Activator.CreateInstance(type));
                    return true;
                }
            }
            return false;
        }

        private static bool NotSystemAssembly(Assembly a)
        {
            //File.AppendAllLines("c:\\temp\\stardustModuleLoader.log", new[] { $"Assembly: {a.FullName}" });
            return !a.FullName.StartsWith("System", StringComparison.InvariantCultureIgnoreCase)
                   && !a.FullName.StartsWith("Stardust", StringComparison.InvariantCultureIgnoreCase)
                   && !a.FullName.StartsWith("Microsoft", StringComparison.InvariantCultureIgnoreCase)
                   && !a.FullName.StartsWith("Owin", StringComparison.InvariantCultureIgnoreCase)
                   && !a.FullName.StartsWith("System", StringComparison.InvariantCultureIgnoreCase)
                   && !a.FullName.StartsWith("Newtonsoft", StringComparison.InvariantCultureIgnoreCase);
        }
    }

    internal class WebConfigManager : IConfigurationReader
    {
        public NameValueCollection AppSettings => ConfigurationManager.AppSettings;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class IocConfigurationAttribute : Attribute
    {
    }
}
