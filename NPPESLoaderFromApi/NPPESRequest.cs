using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Mongo.Nppes.Loader
{
    internal class NPPESClient
    {
        readonly Action<string> callback;
        public NPPESClient(Action<string> callback)
        {
            this.callback = callback;
        }


        public int Count
        {
            get
            {
                return running.Count;
            }
        }


        ConcurrentDictionary<int,Task> running = new ConcurrentDictionary<int, Task>();

        const string uri = "https://npiregistry.cms.hhs.gov/api/?postal_code={0}&=NPI-2&limit=200";

        public async Task RequestAsync(long npi)
        {
            var request = WebRequest.Create(string.Format(uri, npi)) as HttpWebRequest;
            var task = request.GetResponseAsync();
            task.ContinueWith(promise);
            running.TryAdd(task.Id, task);
        }

        public void WaitAll()
        {
            Task.WaitAll(running.Select(x => x.Value).ToArray());
        }

        private void promise(Task<WebResponse> task)
        {
            Console.WriteLine("\tRecived");
            var response = (HttpWebResponse)task.Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var encoding = ASCIIEncoding.ASCII;
                string body = string.Empty;
                using (var reader = new StreamReader(response.GetResponseStream(), encoding))
                {
                    body = reader.ReadToEnd();
                }

                if(body != string.Empty)
                {
                    try
                    {
                        callback(body);
                    }
                    finally
                    {
                        Task t;
                        running.TryRemove(task.Id, out t);
                    }

                }

            }
        }
    }
}