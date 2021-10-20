using System;
using System.IO;
using System.Net;
using System.Text;
using NPPES.Loader.Data;
using NPPES.Loader.Framework;

namespace NPPES.Loader
{
    internal class WebRequestHandler : AsyncWorkerBase<NPIRequest>
    {
        public WebRequestHandler()
        {
        }

        private void postProcess(HttpWebResponse response)
        {
            Console.WriteLine("\tReceived");

            if (response.StatusCode != HttpStatusCode.OK)
                return;

            var encoding = ASCIIEncoding.ASCII;
            string json = string.Empty;
            using (var reader = new StreamReader(response.GetResponseStream(), encoding))
            {
                json = reader.ReadToEnd();
            }

            if (string.IsNullOrEmpty(json))
                return;

            try
            {
                DataFactory.Save(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        protected override void Process(NPIRequest info)
        {
            try
            {
                var formed_uri = string.Format(NPIRequest.URI, info.Zip);
                var request = HttpWebRequest.Create(formed_uri);
                var response = (HttpWebResponse)request.GetResponse();
                postProcess(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}