// -----------------------------------------------------------------------
// <copyright file="Address.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.Domain.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel.DataAnnotations;
    using NHSD.ElephantParade.Domain.Properties;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Address
    {
        

        [Display(Name = "Address1LabelText", ResourceType = typeof(Resources))]
        public string Line1
        {
            get;
            set;
        }
        [Display(Name = "Address2LabelText", ResourceType = typeof(Resources))]
        public string Line2
        {
            get;
            set;
        }

        //removed Line3 as duke does not support so using lowest denominator

        //[Display(Name = "Address3LabelText", ResourceType = typeof(Resources))]
        //public string Line3
        //{
        //    get;
        //    set;
        //}
        [Display(Name = "AddressCountyLabelText", ResourceType = typeof(Resources))]
        public string County
        {
            get;
            set;
        }
        [StringLength(10, ErrorMessageResourceName = "PostalCodeStringLengthValidationError", ErrorMessageResourceType = typeof(Resources))]
        [Display(Name = "AddressPostalCodeLabelText", ResourceType = typeof(Resources))]
        public string PostCode
        {
            get;
            set;
        }

        //removed Country as duke does not support so using lowest denominator

        //[Display(Name = "AddressCountryLabelText", ResourceType = typeof(Resources))]
        //public string Country
        //{
        //    get;
        //    set;
        //}

        #region overrides

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Line1 != null ? Line1.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Line2 != null ? Line2.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (County != null ? County.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PostCode != null ? PostCode.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Address) obj);
        }
        
        protected bool Equals(Address other)
        {
            return string.Equals(Line1, other.Line1) && string.Equals(Line2, other.Line2) && string.Equals(County, other.County) && string.Equals(PostCode, other.PostCode);
        }

        public override string ToString()
        {
            string address = (this.Line1??"").Trim();

            if (this.Line2!=null && this.Line2.Trim() != "")
                address = address + (address.Length>0?", ":"") + this.Line2;
            if (this.County != null && this.County.Trim() != "")
                address = address + (address.Length > 0 ? ", " : "") + this.County;
            if (this.PostCode != null && this.PostCode.Trim() != "")
                address = address + (address.Length > 0 ? ", " : "") + this.PostCode;
            return address;
        }

        #endregion

        
    }
}
