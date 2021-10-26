using System;
using System.Collections.Generic;
using System.Linq;
using NPPES.Loader.Data;

namespace Loader.Data.Implementations
{
    public class TestableMongoAbstractions : IDataAbstractions
    {
        public TestableMongoAbstractions()
        {
        }

        public bool Processed(long zipCode)
        {
            return true;
        }

        public bool Save(string json)
        {
            return true;
        }

        public IEnumerable<int> ZipCodes()
        {
            return Enumerable.Empty<int>();
        }
    }
}
