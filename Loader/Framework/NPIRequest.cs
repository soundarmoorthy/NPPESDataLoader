using System;
using NPPES.Loader.Framework;

namespace NPPES.Loader
{
    public class NPIRequest  
    {
        //This is the maximum number of results that the 
        //NPPES API will return. 
        public static readonly int MAX_RESULTS = 200;
        private static string baseURL = "https://npiregistry.cms.hhs.gov/api/";
        public string URI { get; private set; }

    
        public ZipcodeMetadata Address { get; private set; }
        public int Skip { get; private set; }

        public static NPIRequest Create(ZipcodeMetadata address, int skip = 0)
        {
            return new NPIRequest(address, skip);
        }

        private NPIRequest(ZipcodeMetadata address, int skip)
        {
            this.Address = address;
            this.Skip = skip;
            this.URI = baseURL + "?number=&enumeration_type=" +
		               "&taxonomy_description=&first_name=&last_name=&organization_name" +
		               "=&address_purpose=&city="+address.City.Trim() +
		               "&state="+address.State.Trim() + "&postal_code="+ address._id.Trim() +
		               "&country_code=US&limit=" + MAX_RESULTS + "&skip="+(Skip * MAX_RESULTS) +
	                   "&version=2.0";
        }

        public static NPIRequest Next(NPIRequest source)
        {
            return NPIRequest.Create(source.Address, source.Skip + 1);
        }

        public override string ToString()
        {
            return $"[{Address.ToString()}], [Iteration - {Skip}],{URI}";
        }
    }
}

