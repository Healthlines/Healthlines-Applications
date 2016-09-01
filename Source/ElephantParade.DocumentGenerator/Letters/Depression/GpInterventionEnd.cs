// -----------------------------------------------------------------------
// <copyright file="DepressionQuestionnaireDocument.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.DocumentGenerator.Letters.Depression
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Shapes;
    using System.IO;
    using NHSD.ElephantParade.Domain.Models;

    /// <summary>
    /// The MigraDoc document that represents the letter.
    /// </summary>
    public class GpInterventionEnd: BaseLetterTemplate 
    {
        

       
        protected override void CreateContent(Section contentSection, IDictionary<string, object> values)
        {
            contentSection.AddParagraph("Completion of the Healthlines Service case management programme", "Header1");

            contentSection.AddParagraph("This patient has been participating in the Healthlines Service for the last few months. This consisted of regular phone calls from a Healthlines advisor, helping the patient to make good use of resources available over the internet such as cognitive behaviour therapy.");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("We have now finished our regular calls to the patient, although the Healthlines research team will be contacting them once more to collect final data about their outcomes from the intervention.  We hope we have been able to help you with providing support for this patient.");
            contentSection.AddParagraph("");

            string _importantInfo = values.ContainsKey("Important Information")?(string)values["Important Information"]:null;

            if (_importantInfo != null && _importantInfo.Trim().Length>0)
            {
                var p = contentSection.AddParagraph("Important information for GP");
                p.Format.Font.Bold = true;
                p.Format.Font.Underline=  Underline.Single;
                p.Format.SpaceAfter = 6;

                contentSection.AddParagraph(_importantInfo);
                contentSection.AddParagraph();
            }
                        
        }

        public override IDictionary<string, LetterUserContent> GetFields()
        {
            Dictionary<string, LetterUserContent> fields = new Dictionary<string, LetterUserContent>();
            fields.Add("Important Information", new LetterUserContent() { Type = typeof(string), DefaultContent = "" });
            return fields;
        }


        public override string LetterName
        {
            get { return "GP Intervention End"; }
        }

        public override string LetterDescription
        {
            get { return "Intervention End"; }
        }
        public override Domain.Models.LetterTarget DefaultLetterTarget
        {
            get
            {
                return Domain.Models.LetterTarget.GP;
            }
        }
    }
}
