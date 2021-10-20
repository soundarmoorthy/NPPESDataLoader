using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace NPPES.Loader.Data
{
    public class MongoDataAbstraction : IDataAbstractions
    {
        IMongoDatabase db;
        IMongoCollection<BsonDocument> nppes;

        public MongoDataAbstraction()
        {
            var url = "mongodb://localhost:27017";
            var client = new MongoClient(url);
            db = client.GetDatabase("healthcare");
            nppes = db.GetCollection<BsonDocument>("nppes_roaster");
        }

        IEnumerable<int> IDataAbstractions.ZipCodes()
        {
            var zipEntries = db.GetCollection<BsonDocument>("us_zip_codes");

            foreach (var entry in zipEntries.AsQueryable())
            {
                var value = entry.GetElement("_id").Value;
                if (!value.IsNumeric)
                    continue;

                var zip = value.AsNullableInt32;
                if (!zip.HasValue)
                    continue;

                yield return zip.Value;
            }
        }

        bool IDataAbstractions.Save(string json)
        {
            try
            {
                AddDocuments(json);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void AddDocuments(string json)
        {
            var obj = BsonDocument.Parse(json);
            if (obj == null)
                return;

            var documents = new List<BsonDocument>();
            var results = obj["results"];
            if (results == null)
                return;

            foreach (var result in results.AsBsonArray)
            {
                var npi = result["number"].ToInt64();

                var document = BsonSerializer.Deserialize<BsonDocument>
                    (result.ToString());
                document.Add("_id", npi);
                documents.Add(document);
            }

            if (documents.Any())
            {
                nppes.InsertManyAsync(documents);
            }
        }

    }
}