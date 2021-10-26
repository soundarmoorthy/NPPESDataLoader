using System;
using System.Linq;
using System.Threading;
using NPPES.Loader.Data;

namespace NPPES.Loader
{
    public class NPPESLoader
    {
        public NPPESLoader()
        {

        }

        public void Run()
        {
            Console.Title = "Provider API lookup and store";
            Queue();
        }

        private void Queue()
        {
            var addresses = DataFactory.ZipCodes();

            var scheduler = WebRequestScheduler.Instance;
            scheduler.Start();

            //For testing purpose only when you want to process only one batch
            //Else comment this out for production.
            //addresses = addresses.Take(1);

            foreach (var address in addresses)
            {
                var n = DataFactory.Processed(address);
                if (n == -1)//-1 means proocessing of zip code complete
                    continue;

                //First time when we are queuing a pincode for processing
                //pass skip = 0, The API will treat 0 as nothing to skip,
                //Further in the processing pipeline, we will pass 1,2....
                //to skip entry level result and spool more results beyond
                //the max limit for an API request.
                var request =  NPIRequest.Create(address, n);
                scheduler.Submit(request);
            }
        }
    }
}