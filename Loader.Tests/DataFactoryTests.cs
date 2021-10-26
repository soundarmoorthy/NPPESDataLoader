using NPPES.Loader.Data;
using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace Loader.Tests
{

    public class DataFactoryTests
    {
        public class TestDataProvider : IDataProvider
        {

            private Mock<IData> mock = new Mock<IData>();
            public TestDataProvider(string json, long zipCode)
            {
                mock.Setup(x => x.SaveProvider(json)).Returns(true);
                mock.Setup(x => x.IsZipCodeProcessed(zipCode)).Returns(true);
                mock.Setup(x => x.ZipCodes()).Returns(Enumerable.Empty<int>().ToList());
            }
            public IData Create()
            {
                return mock.Object;
            }
        }

        [SetUp]
        public void Setup()
        {
            DataFactory.Initialize(new TestDataProvider("", 601));
        }

        [Test]
        public void DataFactory_DoesNotThrow_Processed()
        {
            Assert.DoesNotThrow(delegate
            {
                DataFactory.Processed(601);
            });
        }

        [Test]
        public void DataFactory_DoesNotThrow_Save()
        {
            Assert.DoesNotThrow(delegate
            {
                DataFactory.Save(string.Empty);
            });
        }

        [Test]
        public void DataFactory_DoesNotThrow_ZipCodes()
        {
            Assert.DoesNotThrow(delegate
            {
                DataFactory.ZipCodes();
            });
        }
    }
}
