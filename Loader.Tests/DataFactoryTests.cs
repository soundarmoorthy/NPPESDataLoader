using NPPES.Loader.Data;
using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Linq;
using NPPES.Loader;
using NPPES.Loader.Framework;

namespace Loader.Tests
{

    public class DataFactoryTests
    {
        public class TestDataProvider : IDataProvider
        {

            private Mock<IData> mock = new Mock<IData>();
            public TestDataProvider(NpiResponse response)
            {
                mock.Setup(x => x.SaveProvider(response)).Returns(true);
                mock.Setup(x => x.Processed(response.Request.Address)).Returns(0);
                mock.Setup(x => x.ZipCodes()).Returns(Enumerable.Empty<ZipcodeMetadata>().ToList());
            }
            public IData Create()
            {
                return mock.Object;
            }
        }

        [SetUp]
        public void Setup()
        {
            DataFactory.Initialize(new TestDataProvider
                    (NpiResponse.Create(NPIRequest.Create(new ZipcodeMetadata()), "")));
        }

        [Test]
        public void DataFactory_DoesNotThrow_Processed()
        {
            Assert.DoesNotThrow(delegate
            {
                DataFactory.Processed(new ZipcodeMetadata());
            });
        }

        [Test]
        public void DataFactory_DoesNotThrow_Save()
        {
            Assert.DoesNotThrow(delegate
            {
                var request = NPIRequest.Create(new ZipcodeMetadata());
                DataFactory.SaveProvider(NpiResponse.Create(request, ""));
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
