// -----------------------------------------------------------------------
// <copyright file="GpInterventionStart.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.DocumentGenerator.Letters.CVD
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MigraDoc.DocumentObjectModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GpInterventionEnd : BaseLetterTemplate
    {


        protected override void CreateContent(Section contentSection, IDictionary<string, object> values)
        {
            contentSection.AddParagraph("Healthlines Service", "Header1");

            var p = contentSection.AddParagraph("Dear GP");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("This patient has been participating in the Healthlines Service. This consisted of monthly phone calls from a Healthlines advisor for 12 months to address various aspects of the patient’s management including adherence to medication, BP control, smoking, diet and lifestyle issues.");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("We have now finished our regular calls to the patient, although the Healthlines research team will be contacting them once more to collect final data about their outcomes from the intervention.  We hope we have been able to help you with providing support for this patient.");
            contentSection.AddParagraph("");

            string _importantInfo = values.ContainsKey("Important Information") ? (string)values["Important Information"] : "";

            if (_importantInfo.Trim() != "")
            {
                p = contentSection.AddParagraph("Important information for GP");
                p.Format.Font.Bold = true;
                p.Format.Font.Underline = Underline.Single;
                p.Format.SpaceAfter = 6;

                contentSection.AddParagraph(_importantInfo);
                contentSection.AddParagraph();
            }
        }



        public override IDictionary<string, LetterUserContent> GetFields()
        {
            Dictionary<string, LetterUserContent> fields = new Dictionary<string, LetterUserContent>();
            fields.Add("Important Information", new LetterUserContent()
            {
                Type = typeof(string),
                DefaultContent = @""
            });
            return fields;
        }



        public override string LetterName
        {
            get { return "GP Intervention End"; }
        }

        public override string LetterDescription
        {
            get { return "GP Intervention End"; }
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
