using MongoDB.Bson;
using System.Linq;
using MongoDB.Driver;
using NPPES.Loader.Configuration;
using NPPES.Loader.Data;
using NPPES.Loader.Data.Implementation;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace IntegrationTests
{
    public class NPPESCSVValidationTests
    {
        IMongoCollection<Provider> nppes;
        [SetUp]
        public void Setup()
        {
            var db = new MongoClient(
                      LoaderConfig.Get("DataLayer:CSVAnalysis:url")).
                      GetDatabase(LoaderConfig.Get("DataLayer:CSVAnalysis:db"));
            nppes = db.GetCollection<Provider>
                      (LoaderConfig.Get("DataLayer:CSVAnalysis:nppes_roaster"));
        }

        [Test]
        public void NPI_DOESNT_HAVE_DUPLICATES()
        {
            var results = nppes.Find(_ => true)
		                       .Project(x => uint.Parse(x.NPI.Trim()));

            var npis = results.ToList().ToHashSet();
        }
    }
}
