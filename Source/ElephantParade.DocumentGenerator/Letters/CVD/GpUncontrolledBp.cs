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
    public class GpUncontrolledBp : BaseLetterTemplate
    {
        protected override void CreateContent(Section contentSection, IDictionary<string, object> values)
        {
            contentSection.AddParagraph("Healthlines Service", "Header1");
            contentSection.AddParagraph("Important Information for GP", "Header1");

            contentSection.AddParagraph("Dear GP");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("This patient is participating in the Healthlines Service. This aims to support people with long term conditions.");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("As part of this service, we support patients in monitoring their home BP (excepting those who have atrial fibrillation). This patient has recently recorded BP readings which are outside safe levels:");
            contentSection.AddParagraph("");

            var p = contentSection.AddParagraph();

            if (values.ContainsKey("Average BP") && !string.IsNullOrEmpty((string)values["Average BP"]) && !string.IsNullOrEmpty((string)values["Average BP"].ToString().Trim('/')))
            {
                p.Format.SpaceAfter = 6;
                p.AddText("• Their average BP is " + (values.ContainsKey("Average BP") ? (string)values["Average BP"] : "_____"));
            }

            if (values.ContainsKey("BP History") && !string.IsNullOrEmpty((string)values["BP History"]))
            {
                p = contentSection.AddParagraph();
                p.Format.SpaceAfter = 6;
                p.AddText("• We attach a summary of the patient’s recent BP readings: ");
                p = contentSection.AddParagraph();
                p.Format.SpaceAfter = 6;
                p.Format.LeftIndent = "15";
                p.AddText((string)values["BP History"]);
            }

            if (values.ContainsKey("Symptoms") && !string.IsNullOrEmpty((string)values["Symptoms"]))
            {
                p = contentSection.AddParagraph();
                p.Format.SpaceAfter = 6;
                p.AddText("• " + (string)values["Symptoms"]);
            }

            p = contentSection.AddParagraph("We have advised the patient to contact you soon and we would be grateful if you would review their condition and their medication. ");
            p.Format.Font.Bold = true;

            string _importantInfo = values.ContainsKey("Important Information") ? (string)values["Important Information"] : "";

            if (_importantInfo.Trim() != "")
            {
                p = contentSection.AddParagraph("Other Important information for GP");
                p.Format.Font.Bold = true;
                p.Format.Font.Underline = Underline.Single;
                p.Format.SpaceAfter = 6;
                p.Format.SpaceBefore = 16;
                contentSection.AddParagraph(_importantInfo);
                contentSection.AddParagraph();
            }

            string[] bpNotes = values.ContainsKey("BpNotes") ? (string[])values["BpNotes"] : new string[] { string.Empty };

            if (bpNotes[0] != string.Empty)
            {
                p = contentSection.AddParagraph();
                p.Format.SpaceAfter = 6;
                foreach (var item in bpNotes)
                {
                    p = contentSection.AddParagraph();
                    p.AddText("• " + item + ". ");
                    p.Format.LeftIndent = "15";
                }
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
            fields.Add("Average BP", new LetterUserContent()
            {
                Type = typeof(string),
                DefaultContent = @""
            });
            fields.Add("BP History", new LetterUserContent()
            {
                Type = typeof(string),
                DefaultContent = @""
            });
            fields.Add("Symptoms", new LetterUserContent()
            {
                Type = typeof(string),
                DefaultContent = @"The patient is/is not reporting symptoms:

"
            });
            return fields;
        }



        public override string LetterName
        {
            get { return "GP Uncontrolled BP"; }
        }

        public override string LetterDescription
        {
            get { return "GP Uncontrolled Blood Pressure Letter"; }
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
