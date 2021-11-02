namespace NPPES.Loader.Framework
{
    public class ZipcodeMetadata
    {
        /// <summary>
        /// The actual ZipCode in 5 digits
        /// </summary>
        public string _id { get; set; }

        /// <summary>
        /// The city name in plain english
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// The county name in plain english
        /// </summary>
        public string County { get; set; }

        /// <summary>
        /// Two digit state code in US
        /// </summary>
	    public string State { get; set; }

        /// <summary>
        /// Whether the providers for the pincode are processed. Valid values
        /// are "true" or "false"
        /// </summary>
        public string Processed { get; set; }


        /// <summary>
        /// Number of batches processed so far
        /// </summary>
        public int BatchCount { get; set; }


        public override string ToString()
        {
            return $"{City}, {County},{State}, {_id}";
        }

    }

}
