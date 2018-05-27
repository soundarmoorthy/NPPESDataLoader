using System;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

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
            us = client.GetDatabase("US");
            nppes = us.GetCollection<BsonDocument>("NPPES1"); 
            nppesRequests = new NPPESClient(process);

            Console.Title = "Provider API lookup and store";
            await Queue();

            awaitPendingInserts();

        }

        private void awaitPendingInserts()
        {
            nppesRequests.WaitAll();
        }


        private async Task Queue()
        {
            foreach (var zip in ZCTA.Zips)
            {
                GetAndPersistAsync(zip);
                Thread.Sleep(100);
            }
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
                    nppes.InsertManyAsync(documents).ContinueWith(display);
                }
            }
        }

        private void display(Task t)
        {
            if(t.Exception == null)
            {

                Console.WriteLine("\t\t Inserted");
            }
            else
            {
                Console.WriteLine("\t\t"+t.Exception.Message);
            }
        }

        ConcurrentDictionary<int, Task> insertions = new ConcurrentDictionary<int, Task>();

        private static string get(string name, BsonDocument b)
        {
            string item = "";
            if (b.Contains(name))
                item = b.GetValue(name).AsString.Trim();

            return item;
        }
    }
}