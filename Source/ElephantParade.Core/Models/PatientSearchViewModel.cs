using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace NHSD.ElephantParade.Core.Models
{
    public class PatientSearchViewModel
    {
        [DisplayName("Name of Patient")]
        public string PatientName { get; set; }
        [DisplayName("Postcode")]
        public string PatientPostCode { get; set; }
        [DisplayName("GP")]
        public string PatientGP { get; set; }
    }
}
