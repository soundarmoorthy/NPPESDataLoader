using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace NPPES.Loader.Data
{
    public static class DataFactory
    {
        private static IDataAbstractions data;

        static DataFactory()
        {

            var config = GetConfiguration();
            var impl = config["DataLayer:Implementation"].ToString();
            var type = Assembly.GetExecutingAssembly().GetType(impl, false);
            if (type != null)
                data = Activator.CreateInstance(type) as IDataAbstractions;
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


        public static void Save(string json) => data.Save(json);

        public static bool Processed(long zipCode)=> data.Processed(zipCode);

        public static IEnumerable<Int32> ZipCodes() => data.ZipCodes();
    }
}
