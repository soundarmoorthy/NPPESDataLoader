using NPPES.Loader.Data;
using NPPES.Loader.Data.Implementation;
using NUnit.Framework;

namespace IntegrationTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void MongoAbstractions_IsProcessed_Returns_True_If_Processed_Is_True()
        {
            IData data = new MongoData();
            var codes = data.ZipCodes();
            Assert.IsNotNull(codes);
        }
    }
}
