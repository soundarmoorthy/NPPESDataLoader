using System;
using System.Collections.Generic;
using System.Linq;
using NPPES.Loader.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NPPES.Loader.Framework;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Serilog;
using System.Text;

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

            RefreshCache();
        }


        private static object _cache_lock = new object();

        private void RefreshCache()
        {
            if (nppes == null || !nppes.Find(_ => true).Any())
                return;

            var npis = nppes.Find(_ => true)
		               .Project(x => x["_id"]).ToList();
            foreach (var npi in npis)
            {
                var value = Int32.Parse(npi.ToString());
                lock (_cache_lock)
                {
                    if (!cache.Contains(value))
                        cache.Add(value);
                }
            }
            Log.Verbose($"Refreshed cache from underlying data source, Found {cache.Count()} unique NPI's");
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
                Log.Error(ex, "Failed to save provider data");
                return false;
            }
        }

        static HashSet<int> cache = new HashSet<int>();
        private void AddDocuments(NpiResponse response)
        {
            var obj = BsonDocument.Parse(response.Contents);
            if (obj == null)
                return;
            var documents = GetDocuments(obj);

            if (!documents.Any())
            {
                Log.Information($"No documents found for {response.ToString()}");
                ZipCodeComplete(response);
                return;
            }

            List<BsonDocument> uniqueDocs = new List<BsonDocument>();
            StringBuilder builder = new StringBuilder();
            int duplicate = 0;
            foreach (var doc in documents)
            {
                var npi = doc["number"].ToInt32();
                lock (_cache_lock)
                {
                    if (!cache.Contains(npi))
                    {
                        doc["_id"] = npi;
                        uniqueDocs.Add(doc);
                        cache.Add(npi);
                    }
                    else
                    {
                        ++duplicate;
                        builder.Append($",{npi}");
                    }
                }
            }
            Log.Warning($"{duplicate} of {documents.Count()} documents duplicate. Discarding them. Duplicated NPI's are [{builder.ToString().Substring(0,100 > builder.Length ? builder.Length : 100)} ... and more]");

            if (uniqueDocs.Any())
                nppes.InsertMany(uniqueDocs);

            var count = obj["result_count"].AsInt32;
            if (count < NPIRequest.MAX_RESULTS || duplicate == NPIRequest.MAX_RESULTS /* Are all returned NPI's duplicat */)
            {
                //If we have less than MAX_RESULTS then it means that we have
                //processed all zip codes. So we can set the "processed" flag
                //to true
                ZipCodeComplete(response);
                Log.Verbose($"{response.Request.Address} is comlete");
            }
            else
            {
                Log.Verbose($"{response.Request.Address} is scheduled for next iteration");
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