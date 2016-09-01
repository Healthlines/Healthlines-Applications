// -----------------------------------------------------------------------
// <copyright file="Address.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.DocumentGenerator.Letters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Address
    {
        public string Name { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }

        internal static Address Convert(Domain.Models.Address address)
        {
            if (address == null)
                return null;

            return new Address()
            {
                Line1 = address.Line1,
                Line2=address.Line2,
                County = address.County,
                Postcode =  address.PostCode
            };
        }

        
    }
}
