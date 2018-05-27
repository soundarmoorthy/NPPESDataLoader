using System;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using System.Threading;
using Newtonsoft.Json.Linq;

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
            var collection = database.GetCollection<BsonDocument>("US_Provider_Roaster");

            us = client.GetDatabase("US");
            nppes = us.GetCollection<BsonDocument>("NPPES_TEMP"); 
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
            var t = collection.Find("{Entity_Type_Code : '2'}").ForEachAsync(b =>
            {
                var id = long.Parse(get("_id", b));
                if (id % 2 == 0)
                {
                    GetAndPersistAsync(id);
                    Thread.Sleep(100);
                }
            }, token );
            t.Wait(5000);
            source.Cancel();
        }

        private void GetAndPersistAsync(long npi)
        {
            var task = nppesRequests.RequestAsync(npi);
            Console.WriteLine("Requested");
        }

        private void process(string json)
        {
            var obj = JObject.Parse(json);
            if (obj != null)
            {
                var x = obj["results"][0];
                var npi = x["number"].ToString();
                var document = BsonSerializer.Deserialize<BsonDocument>(x.ToString());
                document.Add("_id", npi);
                document.Remove("number");
                nppes.InsertOne(document);
            }
            Console.WriteLine("\t\tInserted");
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