using System;
using System.Linq;
using System.Threading;
using NPPES.Loader.Data;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace NPPES.Loader
{
    public class NPPESLoader
    {
        public NPPESLoader()
        {

        }

        public void Run()
        {
            SetupLogger();
            Console.Title = "NPPES Registry Loader";
            Queue();
        }

        private void SetupLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Debug)
                .WriteTo.File($"{DateTime.Now.ToFileTime()}-run.log", rollOnFileSizeLimit: true, fileSizeLimitBytes: 10 * 1024 * 1024)
                .CreateLogger();
        }

        private void Queue()
        {
            var addresses = DataFactory.ZipCodes();

            var scheduler = WebRequestScheduler.Instance;
            scheduler.Start();

            Log.Information($"Begin processing of NPI requests. Total ZipCodes found [{addresses.Count()}]");
            //For testing purpose only when you want to process only one batch
            //Else comment this out for production.
            //addresses = addresses.Take(1);

            foreach (var address in addresses)
            {
                var n = DataFactory.Processed(address);
                if (n == -1)//-1 means proocessing of zip code complete
                {
                    Log.Debug($"{address.ToString()} already processed. Skipping now");
                    continue;
                }

                //First time when we are queuing a pincode for processing
                //pass skip = 0, The API will treat 0 as nothing to skip,
                //Further in the processing pipeline, we will pass 1,2....
                //to skip entry level result and spool more results beyond
                //the max limit for an API request.
                var request =  NPIRequest.Create(address, n);
                Log.Debug($"{address.ToString()}, iteration {n} processing started.");
                scheduler.Submit(request);
            }
        }
    }
}