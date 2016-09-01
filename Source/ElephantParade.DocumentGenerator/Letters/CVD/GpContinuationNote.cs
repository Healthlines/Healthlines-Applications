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
    public class GpContinuationNote : BaseLetterTemplate
    {


        protected override void CreateContent(Section contentSection, IDictionary<string, object> values)
        {
            contentSection.AddParagraph("Healthlines Service", "Header1");
            contentSection.AddParagraph("Monthly Report", "Header1");

            contentSection.AddParagraph("Dear GP");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("This patient is participating in the Healthlines Service. This aims to support people with long term conditions and consists of regular phone calls from a Healthlines advisor, along with helping the patient to make good use of resources available over the internet. For patients with hypertension (and who do not have atrial fibrillation) we also monitor their home BP readings.");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("We contacted your patient today. This is a record of the issues discussed.");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("SELF-MANAGEMENT TOPICS DISCUSSED:");
            contentSection.AddParagraph("");

            if (values.ContainsKey("Topics Discussed"))
            {
                string[] modules = (string[])values["Topics Discussed"];
                foreach (var item in modules)
                {
                    contentSection.AddParagraph(item);
                }
            }

            contentSection.AddParagraph("");
            var p = contentSection.AddParagraph();
            p.AddFormattedText("Target BP " + (values.ContainsKey("Target BP")? (string)values["Target BP"] : ""), TextFormat.Underline);
            contentSection.AddParagraph("");

            p = contentSection.AddParagraph();
            p.AddFormattedText("Latest average home BP reading, based on average of readings over last 6 weeks (or last 6 days for first readings): " + (values.ContainsKey("Average BP") ? (string)values["Average BP"] : ""), TextFormat.Underline);
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
            fields.Add("Topics Discussed", new LetterUserContent()
            {
                Type = typeof(string[]),
                DefaultContent = @""
            });
            fields.Add("Target BP", new LetterUserContent()
            {
                Type = typeof(string),
                DefaultContent = @""
            });
            fields.Add("Average BP", new LetterUserContent()
            {
                Type = typeof(string),
                DefaultContent = @""
            });
            fields.Add("Important Information", new LetterUserContent()
            {
                Type = typeof(string),
                DefaultContent = @""
            });
            return fields;
        }



        public override string LetterName
        {
            get { return "GP Continuation"; }
        }

        public override string LetterDescription
        {
            get { return "GP Continuation"; }
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
