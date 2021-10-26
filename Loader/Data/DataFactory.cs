using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using NPPES.Loader.Framework;

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

        public static void SaveProvider(NpiResponse response) => data.SaveProvider(response);

        public static int Processed(Address address)=> data.Processed(address);

        public static IEnumerable<Address> ZipCodes() => data.ZipCodes();
    }
}
