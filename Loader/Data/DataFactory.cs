using System;
using System.Collections;
using System.Collections.Generic;
using NPPES.Loader.Data;

namespace NPPES.Loader.Data
{
    public static class DataFactory
    {
        private static IDataAbstractions data;

        static DataFactory()
        {
            data = new MongoDataAbstraction();
        }

        public static void Save(string json)
        {
            data.Save(json);
        }

        public static IEnumerable<Int32> ZipCodes()
        {
            return data.ZipCodes();
        }
    }
}

