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
    public class GpPHQ9: BaseLetterTemplate 
    {

        private bool RequiredUserInput { get; set; }

        public GpPHQ9(bool userInput)
            : base()
        {
            RequiredUserInput = userInput;
        }

        protected override void CreateContent(Section contentSection, IDictionary<string, object> values)
        {
            contentSection.AddParagraph("PHQ 9 Score", "Header1");

            contentSection.AddParagraph("This patient is participating in the Healthlines Service. This is a Telecoaching Case Management programme designed to support patients with long term health conditions.");
            contentSection.AddParagraph("");
            contentSection.AddParagraph(@"This patient is taking part in the Healthlines service because of depression.  

This consists of regular phone calls from an advisor, helping the patient to make good use of resources available over the internet such as cognitive behaviour therapy.
");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("As part of this service, we regularly assess the patients PHQ9 score in order to monitor their progress. We inform you of these scores for your information.");
            var p = contentSection.AddParagraph("");
            p.Format.SpaceAfter = 10;

            string phq9Score = values.ContainsKey("PHQ9Score")? values["PHQ9Score"].ToString():"";
            DateTime phq9Date = values.ContainsKey("PHQ9Date") ? (DateTime)values["PHQ9Date"] : DateTime.Now ;

            p = contentSection.AddParagraph(string.Format("Their latest PHQ9 score was {0}  on {1}", phq9Score, phq9Date.ToShortDateString()));
            p.Format.Font.Bold = true;
            p.Format.SpaceAfter = 10;

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
            fields.Add("Important Information", new LetterUserContent() { Type = typeof(string), DefaultContent = "" });
            if (RequiredUserInput)
            {
                fields.Add("PHQ9Score", new LetterUserContent() { Type = typeof(string), DefaultContent = "" });
                fields.Add("PHQ9Date", new LetterUserContent() { Type = typeof(DateTime), DefaultContent = "" });
            }
            return fields;
        }

        public override string LetterName
        {
            get { return "GP PHQ9"; }
        }

        public override string LetterDescription
        {
            get { return "GP PHQ9 Letter"; }
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
