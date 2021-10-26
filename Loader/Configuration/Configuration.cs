using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using NPPES.Loader.Data;

namespace NPPES.Loader.Configuration
{
    public static class LoaderConfig
    {
        private static IConfiguration config { get; set; }
        static LoaderConfig()
        {
        }

        public static void Initialize()
        {
            config = GetConfiguration();
        }

        public static IData Current
        {
            get
            {
                try
                {
                    if (config == null)
                        Initialize();

                    //Find the current enabled implementation
                    var current = config["DataLayer:Current"];
                    //Create an instance of the type for the implementation
                    return CreateImplementation(current);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static string Get(string key)
        {
            if (config == null)
                Initialize();

            return config[key];
	}

        private static IData CreateImplementation(string current)
        {
            var impl = config[$"DataLayer:Implementation:{current}:Name"];
            var type = Assembly.GetExecutingAssembly().GetType(impl, false);
            if (type != null)
                return Activator.CreateInstance(type) as IData;
            else
                throw new TypeLoadException($"The given type {type.ToString()} is not available in the assembly {Assembly.GetExecutingAssembly().FullName}. Please make sure to give a valid fully qualified type name in app.config");
        }

        private static IConfiguration GetConfiguration()
        {
            var config = new ConfigurationBuilder().
            AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();
            return config;
        }
    }
}
