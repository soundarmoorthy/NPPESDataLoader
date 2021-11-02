// <auto-generated />
//
// This code is auto generated from the json response that is  returned by
// the nppes API using the CodeBeautify online tool
//

using System;
using MongoDB.Bson.Serialization.Attributes;

namespace NPPES.Loader.Framework
{
    public class Provider
    {
        [BsonElement("_id")]
        public long Id { get; set; }

        [BsonElement("enumeration_type")]
        public string EnumerationType { get; set; }

        [BsonElement("number")]
        public long Number { get; set; }

        [BsonElement("last_updated_epoch")]
        public long LastUpdatedEpoch { get; set; }

        [BsonElement("created_epoch")]
        public long CreatedEpoch { get; set; }

        [BsonElement("basic")]
        public Basic Basic { get; set; }

        [BsonElement("other_names")]
        public OtherName[] OtherNames { get; set; }

        [BsonElement("addresses")]
        public Address[] Addresses { get; set; }

        [BsonElement("taxonomies")]
        public Taxonomy[] Taxonomies { get; set; }

        [BsonElement("identifiers")]
        public Identifier[] Identifiers { get; set; }
    }

    public class Address
    {
        [BsonElement("country_code")]
        public string CountryCode { get; set; }

        [BsonElement("country_name")]
        public string CountryName { get; set; }

        [BsonElement("address_purpose")]
        public string AddressPurpose { get; set; }

        [BsonElement("address_type")]
        public string AddressType { get; set; }

        [BsonElement("address_1")]
        public string Address1 { get; set; }

        [BsonElement("address_2")]
        public string Address2 { get; set; }

        [BsonElement("city")]
        public string City { get; set; }

        [BsonElement("state")]
        public string State { get; set; }

        [BsonElement("postal_code")]
        public string PostalCode { get; set; }

        [BsonElement("telephone_number")]
        public string TelephoneNumber { get; set; }

        [BsonElement("fax_number")]
        public string FaxNumber { get; set; }
    }

    public class Basic
    {
        [BsonElement("organization_name")]
        public string OrganizationName { get; set; }

        [BsonElement("organizational_subpart")]
        public string OrganizationalSubpart { get; set; }

        [BsonElement("enumeration_date")]
        public DateTimeOffset EnumerationDate { get; set; }

        [BsonElement("last_updated")]
        public DateTimeOffset LastUpdated { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        [BsonElement("authorized_official_first_name")]
        public string AuthorizedOfficialFirstName { get; set; }

        [BsonElement("authorized_official_last_name")]
        public string AuthorizedOfficialLastName { get; set; }

        [BsonElement("authorized_official_telephone_number")]
        public string AuthorizedOfficialTelephoneNumber { get; set; }

        [BsonElement("authorized_official_title_or_position")]
        public string AuthorizedOfficialTitleOrPosition { get; set; }

        [BsonElement("authorized_official_name_prefix")]
        public string AuthorizedOfficialNamePrefix { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }
    }

    public class Identifier
    {
        [BsonElement("identifier")]
        public string IdentifierIdentifier { get; set; }

        [BsonElement("code")]
        public string Code { get; set; }

        [BsonElement("desc")]
        public string Desc { get; set; }

        [BsonElement("state")]
        public string State { get; set; }

        [BsonElement("issuer")]
        public string Issuer { get; set; }
    }

    public class OtherName
    {
        [BsonElement("organization_name")]
        public string OrganizationName { get; set; }

        [BsonElement("code")]
        public long Code { get; set; }

        [BsonElement("type")]
        public string Type { get; set; }
    }

    public class Taxonomy
    {
        [BsonElement("code")]
        public string Code { get; set; }

        [BsonElement("desc")]
        public string Desc { get; set; }

        [BsonElement("primary")]
        public bool Primary { get; set; }

        [BsonElement("state")]
        public string State { get; set; }

        [BsonElement("license")]
        public string License { get; set; }
    }
}
