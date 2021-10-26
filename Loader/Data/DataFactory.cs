using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace NPPES.Loader.Data
{
    public class DataFactory
    {
        private static IData data;

        static DataFactory()
        {
            var provider = new ConfigurationDataProvider();
            data = provider.Create();
        }

        public static void Initialize(IDataProvider overrided)
        {
            data = overrided.Create();
        }

        public static void Save(string json) => data.SaveProvider(json);

        public static bool Processed(long zipCode)=> data.IsZipCodeProcessed(zipCode);

        public static IEnumerable<Int32> ZipCodes() => data.ZipCodes();
    }
}
