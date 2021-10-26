using System;
using System.Collections.Generic;
using NPPES.Loader.Framework;

namespace NPPES.Loader.Data
{
    public interface IData
    {
        /// <summary>
		/// Returns a list of valid zip codes for US.
		/// </summary>
		/// <returns>A valid 3 (or) 5 (or) 7 (or) 9 digit zip code</returns>
        IList<Address> ZipCodes();

        /// <summary>
		/// Given a json representation of provider NPI data stores them
		/// to the underlying data store
		/// </summary>
		/// <param name="response">The response information to be saved</param>
		/// <remarks>This method is not expected to return </remarks>
        bool SaveProvider(NpiResponse response);

		/// <summary>
        /// Decides whether providers exist in the database for the given
        /// zip code.
        /// </summary>
        /// <param name="address">The address information that includes zipcode, city, county, state
        /// <returns>0 if the zip is not processed. A positive integer
	    /// that represents the number of batches processed so far</returns>
		int Processed(Address address);
    }
}

