using MongoDB.Bson;
using System.Linq;
using MongoDB.Driver;
using NPPES.Loader.Configuration;
using NUnit.Framework;

namespace IntegrationTests
{
    public class NPPESCSVValidationTests
    {
        IMongoCollection<BsonDocument> nppes_csv;
        IMongoCollection<BsonDocument> nppes_api;
        [SetUp]
        public void Setup()
        {
            var db = new MongoClient(
                      LoaderConfig.Get("DataLayer:CSVAnalysis:url")).
                      GetDatabase(LoaderConfig.Get("DataLayer:CSVAnalysis:db"));
            nppes_csv = db.GetCollection<BsonDocument>
                      (LoaderConfig.Get("DataLayer:CSVAnalysis:nppes_csv_roaster"));

            nppes_api = db.GetCollection<BsonDocument>
                      (LoaderConfig.Get("DataLayer:CSVAnalysis:nppes_api_roaster"));
        }

        [Test]
        public void NPI_DOESNT_HAVE_DUPLICATES()
        {
            var results = nppes_csv.Find(_ => true)
	             .Project(x => uint.Parse(x["NPI"].ToString().Trim()));

            var npis_csv = results.ToList().ToHashSet();

            results = nppes_api.Find(_ => true)
                .Project(x => uint.Parse(x["_id"].ToString().Trim()));

	        var npis_api = results.ToList().ToHashSet();

            var expected = npis_csv.Distinct().Count();
            var actual = npis_csv.Count();


            Assert.AreEqual(expected, actual);
	    }

        [Test]
        public void Are_NPIS_Missing_In_Api_Dataset()
        {
            var results = nppes_csv.Find(_ => true)
	             .Project(x => uint.Parse(x["NPI"].ToString().Trim()));

            var npis_csv = results.ToList().ToHashSet();

            results = nppes_api.Find(_ => true)
                .Project(x => uint.Parse(x["_id"].ToString().Trim()));

	        var npis_api = results.ToList().ToHashSet();

            var expected = npis_csv.Except(npis_api);
            var actual = npis_api.Count();
            System.IO.File.WriteAllLines("/tmp/missing", expected.Select(x => x.ToString()).ToArray());
            Assert.AreEqual(expected, actual);
	    }
    }
}
