using System;
namespace NPPES.Loader
{
    public class NPIRequest
    {
        public static readonly string URI = "https://npiregistry.cms.hhs.gov/api/?number=&enumeration_type=&taxonomy_description=&first_name=&last_name=&organization_name=&address_purpose=&city=&state=&postal_code={0}&country_code=US&limit=500&skip=&version=2.0";
        public long Zip { get; }
        public NPIRequest(long zip)
        {
            this.Zip = zip;
        }
    }
}

