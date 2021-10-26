using System;
using System.Collections.Generic;
using System.Linq;
using NPPES.Loader.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NPPES.Loader.Framework;

namespace NPPES.Loader.Data.Implementation
{
    public class MongoData : IData
    {
        private static readonly string tag = $"DataLayer:Implementation:Mongo";
        private static readonly string url_name = $"{tag}:url";
        private static readonly string db_name = $"{tag}:db";
        private static readonly string roaster_name = $"{tag}:roaster_collection";
        private static readonly string zip_codes = $"{tag}:zip_codes";
        private static readonly string TRUE="true";
        private static readonly string FALSE="false";
        IMongoDatabase db;
        IMongoCollection<BsonDocument> nppes;
        IMongoCollection<Address> zipEntries;

        public MongoData()
        {
            db = new MongoClient(LoaderConfig.Get(url_name)).
		            GetDatabase(LoaderConfig.Get(db_name));

            nppes = db.GetCollection<BsonDocument>
		            (LoaderConfig.Get(roaster_name));

            zipEntries = db.GetCollection<Address>
		            (LoaderConfig.Get(zip_codes));
        }

        int IData.Processed(Address address)
        {
            try
            {
                var filter = Builders<Address>.Filter.Eq(x => x._id, address._id);
                var entry = zipEntries.Find(filter);
                if (entry == null || entry.First() == null)
                    return -1; //Unknown pincode from our list

                var first = entry.First();

                if (string.IsNullOrEmpty(first.Processed))
                    return 0;

                if (first.Processed == TRUE)
                    return -1;

                if (first.Processed == FALSE)
                    return first.BatchCount + 1;

                throw new InvalidProgramException("This is an invalid state for the processed attribute for zipCode " + address._id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }

        IList<Address> IData.ZipCodes()
        {
            List<Address> addresses = new List<Address>();
            foreach (var address in zipEntries.Find(_ => true).ToList())
            {
                addresses.Add(address);
            }
            return addresses;
        }

        bool IData.SaveProvider(NpiResponse response)
        {
            try
            {
                AddDocuments(response);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void AddDocuments(NpiResponse response)
        {

            var obj = BsonDocument.Parse(response.Contents);
            if (obj == null)
                return;
            var documents = GetDocuments(obj);

            if (!documents.Any())
            {
                ZipCodeComplete(response);
                return;
            }

            nppes.InsertMany(documents);
            var count = obj["result_count"].AsInt32;
            if (count < NPIRequest.MAX_RESULTS)
            {
                //If we have less than MAX_RESULTS then it means that we have
                //processed all zip codes. So we can set the "processed" flag
                //to true
                ZipCodeComplete(response);
            }
            else
            {
                ScheduleNext(response);
            }

        }

        private IEnumerable<BsonDocument> GetDocuments(BsonDocument doc)
        {
            var documents = new List<BsonDocument>();
            var results = doc["results"];
            if (results == null)
                return null;


            foreach (var result in results.AsBsonArray)
            {
                var document = BsonSerializer.Deserialize<BsonDocument>
                    (result.ToString());
                documents.Add(document);
            }
            return documents;
        }

        private void ScheduleNext(NpiResponse response)
        {
            var add = response.Request.Address;
            var filter = Builders<Address>.Filter.Eq(x => x._id, add._id);
            var update = Builders<Address>.Update
                        .Set(x => x.BatchCount, add.BatchCount + 1)
                        .Set(x => x.Processed, FALSE);

            zipEntries.UpdateOne(filter, update);

            var request = NPIRequest.Next(response.Request);
            WebRequestScheduler.Instance.Submit(request);
        }

        private void ZipCodeComplete(NpiResponse response)
        {
            var filter = Builders<Address>.Filter.Eq(x => x._id, response.Request.Address._id);
            var update = Builders<Address>.Update
                        .Set(x => x.BatchCount, response.Request.Skip + 1)
                        .Set(x => x.Processed, TRUE);
            var document = zipEntries.FindOneAndUpdate(filter, update);
        }
    }
}