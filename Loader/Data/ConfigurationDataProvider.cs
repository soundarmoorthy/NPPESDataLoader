using System;
using NPPES.Loader.Configuration;

namespace NPPES.Loader.Data
{
    public class ConfigurationDataProvider : IDataProvider
    {
        public IData Create() => LoaderConfig.Current;
    }
}
