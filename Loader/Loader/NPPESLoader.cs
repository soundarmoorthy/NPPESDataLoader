using System;
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
            var zips = DataFactory.ZipCodes();

            var scheduler = new WebRequestScheduler();
            scheduler.Start();

            foreach (var zip in zips)
            {
                NPIRequest request = new NPIRequest(zip);
                scheduler.Submit(request);
            }
        }
    }
}