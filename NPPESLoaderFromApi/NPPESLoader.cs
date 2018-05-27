using System;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Mongo.Nppes.Loader
{
    public class NPPESLoader
    {
        public NPPESLoader()
        {

        }


        IMongoDatabase us;
        IMongoCollection<BsonDocument> nppes;
        NPPESClient nppesRequests;

        public async void Run()
        {
            var url = "mongodb://localhost:27017";
            var client = new MongoClient(url);
            var database = client.GetDatabase("NPPEES");
            var collection = database.GetCollection<BsonDocument>("ZipCodeTimeZone");

            us = client.GetDatabase("US");
            nppes = us.GetCollection<BsonDocument>("NPPES_ORG"); 
            nppesRequests = new NPPESClient(process);

            Console.Title = "Provider API lookup and store";
            await Queue(collection);

            awaitAllPending();

        }

        private void awaitAllPending()
        {
            nppesRequests.WaitAll();
        }


        private async Task Queue(IMongoCollection<BsonDocument> collection)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            var t = collection.Find("{}").ForEachAsync(b =>
            {
                var str = get("zip", b);
                int zip = 0;
                if (int.TryParse(str, out zip))
                {
                    GetAndPersistAsync(zip);
                }
            }, token);
            t.Wait();
        }

        private void GetAndPersistAsync(long npi)
        {
            var task = nppesRequests.RequestAsync(npi);
            Console.WriteLine("Requested");
        }

        private void process(string json)
        {
            var obj = JObject.Parse(json);

            List<BsonDocument> documents = null;
            if (obj != null)
            {
                documents = new List<BsonDocument>();
                var results = obj["results"];
                foreach (var result in results)
                {
                    var document = BsonSerializer.Deserialize<BsonDocument>(result.ToString());
                    documents.Add(document);
                }
                if (documents.Any())
                {
                    nppes.InsertMany(documents);
                    Console.WriteLine(string.Format("\t\tInserted [Pending Insert : {0}]", nppesRequests.Count));
                }
            }
            else
                Console.WriteLine("\t\t**No Result**");
        }

        private static string get(string name, BsonDocument b)
        {
            string item = "";
            if (b.Contains(name))
                item = b.GetValue(name).AsString.Trim();

            return item;
        }
    }
}