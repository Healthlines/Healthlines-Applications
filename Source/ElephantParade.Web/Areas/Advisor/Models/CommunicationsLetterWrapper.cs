using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHSD.ElephantParade.Domain.Models;
using NHSD.ElephantParade.DocumentGenerator.Letters;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NHSD.ElephantParade.Web.Areas.Advisor.Models
{
    public class CommunicationsLetterWrapper
    {
        
        public string StudyID { get ; set ; }
        public string PatientId { get; set; }
        public String LetterID { get; set; }       
 
        public String LetterName { get; set; }
        public string LetterDiscription { get; set; }

        [AllowHtml]
        public string EmailContent { get; set; }
        public string EmailTo { get; set; }

        //fields define what values a letter requires also contians the default values
        public IDictionary<string, LetterUserContent> fields { get; set; }
        
        //values is a HACK to make posting the data back to the contoller easier as it contains string only values. 
        //The values dictonary should contain the same keys as the fields dictonary along with a value string representing the fields value
        public IDictionary<string, string> values { get; set; }

        public string ResultSetID { get; set; }
    }
}