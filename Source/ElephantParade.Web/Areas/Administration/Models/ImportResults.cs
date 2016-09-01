using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHSD.ElephantParade.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace NHSD.ElephantParade.Web.Areas.Administration.Models
{
    public class ImportResults 
    {
        [Display(Name = "File name")]
        public string Filename { get; set; }
        public string Status { get; set; }
        public string[] FileErrors { get; set; }
        public string[] ImportErrors { get; set; }
        public List<StudyPatient> Created { get; set; }

        public ImportResults()
        {
            Created = new List<StudyPatient>();
        }
    }
}
