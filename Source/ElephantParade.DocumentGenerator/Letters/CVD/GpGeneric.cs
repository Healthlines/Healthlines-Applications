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
    public class GpGeneric : BaseLetterTemplate
    {


        protected override void CreateContent(Section contentSection, IDictionary<string, object> values)
        {
            contentSection.AddParagraph("Healthlines Service Information for GP", "Header1");

            var p = contentSection.AddParagraph("Dear GP");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("This patient is participating in the Healthlines Service. This aims to support people with long term conditions and consists of regular phone calls from a Healthlines advisor, along with helping the patient to make good use of resources available over the internet. For patients with hypertension (and who do not have atrial fibrillation) we also monitor their home blood pressure readings.");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("We are writing to inform you about the following issue that has arisen from our phone calls with the patient:");
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
                DefaultContent = @"

The patient has/has not been advised to seek medical advise.

We have/have not advised the patient to contact you to discuss this.
"
            });
            return fields;
        }



        public override string LetterName
        {
            get { return "GP Generic"; }
        }

        public override string LetterDescription
        {
            get { return "Generic Letter"; }
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
