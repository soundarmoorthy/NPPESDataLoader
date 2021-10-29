using System;
using System.IO;
using System.Net;
using System.Text;
using NPPES.Loader.Data;
using NPPES.Loader.Framework;
using Serilog;

namespace NPPES.Loader
{
    internal class WebRequestHandler : AsyncWorkerBase<NPIRequest>
    {
        public WebRequestHandler()
        {
        }

        private void postProcess(HttpWebResponse response, NPIRequest request)
        {
            if (response.StatusCode != HttpStatusCode.OK)
                return;

            var encoding = ASCIIEncoding.ASCII;
            string json = string.Empty;
            using (var reader = new StreamReader(response.GetResponseStream(), encoding))
            {
                json = reader.ReadToEnd();
            }

            if (string.IsNullOrEmpty(json))
            {
                Log.Debug($"Empty resonse returned for address {request.Address}"
                    +", iteration {request.Skip}. URL is {request.URI}");
                return;
            }

            try
            {
                var npiResponse = NpiResponse.Create(request, json);
                DataFactory.SaveProvider(npiResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        protected override void Process(NPIRequest npiRequest)
        {
            try
            {
                var request = HttpWebRequest.Create(npiRequest.URI);
                var response = (HttpWebResponse)request.GetResponse();
                postProcess(response, npiRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}