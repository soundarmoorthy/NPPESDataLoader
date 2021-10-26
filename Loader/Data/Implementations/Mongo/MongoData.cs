using System;
using System.Collections.Generic;
using System.Linq;
using NPPES.Loader.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace NPPES.Loader.Data.Implementation
{
    public class MongoData : IData
    {
        private static readonly string tag = $"DataLayer:Implementation:Mongo";
        private static readonly string url_name = $"{tag}:url";
        private static readonly string db_name = $"{tag}:db";
        private static readonly string roaster_name = $"{tag}:roaster_collection";
        private static readonly string zip_codes = $"{tag}:zip_codes";
        IMongoDatabase db;
        IMongoCollection<BsonDocument> nppes;
        IMongoCollection<BsonDocument> zipEntries;

        public MongoData()
        {
            var url = LoaderConfig.Get(url_name);
            var client = new MongoClient(url);
            db = client.GetDatabase(db_name);
            nppes = db.GetCollection<BsonDocument>(roaster_name);
        }

        bool IData.IsZipCodeProcessed(long zipCode)
        {
            try
            {
                zipEntries = db.GetCollection<BsonDocument>(zip_codes);
                var item = zipEntries.Find(x => x["_id"] == $"{zipCode}").Any();
                return item;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        IEnumerable<int> IData.ZipCodes()
        {
            var zipEntries = db.GetCollection<BsonDocument>(zip_codes);

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

        bool IData.SaveProvider(string json)
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