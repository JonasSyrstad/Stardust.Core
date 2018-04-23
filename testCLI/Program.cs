using System;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization.Json;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using test;
using Stardust.Nucleus;
using Stardust.Nucleus.ContextProviders;
using Stardust.Particles;

namespace testCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigurationManagerHelper.SetManager(new ConfigManager());
            Resolver.LoadModuleConfiguration<Dummy>();
            var resolver = Resolver.CreateScopedResolver();

            {
                var instance = resolver.GetService<IDummyService>();
                Console.WriteLine(instance.GetMessage());

            }
        }
    }

    internal class ConfigManager : IConfigurationReader
    {
        public NameValueCollection AppSettings
        {
            get
            {
                var builder = new ConfigurationBuilder();
                //.SetBasePath(env.ContentRootPath)
                //.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                //.AddEnvironmentVariables();

                Configuration = builder.Build();
                var v = new NameValueCollection();
                foreach (var appsetting in Configuration.GetSection("AppSettings").GetChildren())
                {
                    v.Add(appsetting.Key, appsetting.Value);
                }
                return v;
            }
        }

        public IConfigurationRoot Configuration { get; set; }
    }
}
