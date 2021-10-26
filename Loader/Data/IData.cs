using System;
using System.Collections.Generic;

namespace NPPES.Loader.Data
{
    public interface IData
    {
        /// <summary>
		/// Returns a list of valid zip codes for US.
		/// </summary>
		/// <returns>A valid 3 (or) 5 (or) 7 (or) 9 digit zip code</returns>
        IList<Int32> ZipCodes();

        /// <summary>
		/// Given a json representation of provider NPI data stores them
		/// to the underlying data store
		/// </summary>
		/// <param name="json">True if saved succesfully, false otherwise</param>
		/// <remarks>This method is not expected to return </remarks>
        bool SaveProvider(string json);

		/// <summary>
        /// Decides whether providers exist in the database for the given
        /// zip code.
        /// </summary>
        /// <param name="zipCode"></param>
        /// <returns></returns>
		bool IsZipCodeProcessed(long zipCode);
    }
}

