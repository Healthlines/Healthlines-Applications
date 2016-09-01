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
    using MigraDoc.DocumentObjectModel.Tables;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GpStatins
        : BaseLetterTemplate
    {


        protected override void CreateContent(Section contentSection, IDictionary<string, object> values)
        {
            contentSection.AddParagraph("Healthlines Service Referral Form", "Header1");
            contentSection.AddParagraph("Statins for Primary Prevention of Cardiovascular Disease", "Header1");

            var p = contentSection.AddParagraph("Dear GP");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("This patient is taking part in the Healthlines Service. We have identified that they may benefit from a statin and they appear to be suitable for it. They would like a prescription for a statin if you agree. ");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("Please note, this letter is a referral for the patient to be considered for a statin. It is at your discretion whether or not to prescribe it.");
            contentSection.AddParagraph("");

            p = contentSection.AddParagraph("Assessment of cardiovascular risk");
            p.Format.Font.Bold = true;
            p.Format.Font.Underline = Underline.Single;
            p.Format.SpaceAfter = 6;
            p = contentSection.AddParagraph("The patient has a 10 year cardiovascular risk of ≥20%.");
            p.Style = "TextBoxPlain";
            p.Format.SpaceAfter = 10;

            string _importantInfo = values.ContainsKey("Additional information") ? (string)values["Additional information"] : "";

            if (_importantInfo.Trim() != "")
            {
                p = contentSection.AddParagraph("Additional information");
                p.Format.Font.Bold = true;
                p.Format.Font.Underline = Underline.Single;
                p.Format.SpaceAfter = 6;

                contentSection.AddParagraph(_importantInfo);
                contentSection.AddParagraph();
            }
            
            contentSection.AddPageBreak();


            p = contentSection.AddParagraph("Dose of statin");
            p.Format.Font.Bold = true;
            p.Format.Font.Underline = Underline.Single;
            p.Format.SpaceAfter = 3;
            p = contentSection.AddParagraph(@"NICE guidance recommends simvastatin 40mg daily for primary prevention of cardiovascular disease in patients with a CVD risk ≥20%. 
If simvastatin 40mg is not tolerated - offer pravastatin or a lower dose of simvastatin. ");
            p.Style = "TextBoxPlain";

            presctribingNotes(contentSection);

        }



        private void presctribingNotes(Section contentSection)
        {
            var p = contentSection.AddParagraph("Additional prescribing notes");
            p.Format.Font.Bold = true;
            p.Format.Font.Underline = Underline.Single;
            p.Format.SpaceBefore = 16;
            p.Format.SpaceAfter = 3;

            Table table = new Table();
            table.Borders.Width = 0.0;
            Column column = table.AddColumn(Unit.FromCentimeter(17));

            column.LeftPadding = 10;
            column.RightPadding = 10;
            Row row = table.AddRow();
            row.TopPadding = 10;
            row.BottomPadding = 5;
            Cell cell = row.Cells[0];
            Paragraph paragraph = cell.AddParagraph("• Measure liver function before starting treatment, within 3 months and at 12 months.");
            //paragraph = cell.AddParagraph("");
            //paragraph.Format.LeftIndent = "0.3cm";

            row = table.AddRow();
            row.BottomPadding = 5;
            cell = row.Cells[0];
            paragraph = cell.AddParagraph("• If simvastatin is contraindicated, offer an alternative e.g. pravastatin.");
            //paragraph = cell.AddParagraph("");
            //paragraph.Format.LeftIndent = "0.3cm";
            
            row = table.AddRow();
            row.BottomPadding = 5;
            cell = row.Cells[0];
            paragraph = cell.AddParagraph("• If introducing a drug that interferes with statin metabolism e.g. erythromycin, reduce the statin dose or stop the statin temporarily / permanently. ");
            //paragraph = cell.AddParagraph("");
            //paragraph.Format.LeftIndent = "0.3cm";
            row = table.AddRow();
            row.BottomPadding = 5;
            cell = row.Cells[0];
            paragraph = cell.AddParagraph("• If unexplained peripheral neuropathy develops - stop statin and seek specialist advice.");
            //paragraph = cell.AddParagraph("");
            //paragraph.Format.LeftIndent = "0.3cm";
            row = table.AddRow();
            row.BottomPadding = 5;
            cell = row.Cells[0];
            paragraph = cell.AddParagraph("• Do not use a statin and gemfibrozil together. Concomitant use of a statin with fibrates or nicotinic acid increases muscle toxicity – use with caution.");
            //paragraph = cell.AddParagraph("");
            //paragraph.Format.LeftIndent = "0.3cm";
            row = table.AddRow();
            row.BottomPadding = 5;
            cell = row.Cells[0];
            paragraph = cell.AddParagraph("• Only monitor creatine kinase in patients with muscle symptoms.");
            //paragraph = cell.AddParagraph("");
            //paragraph.Format.LeftIndent = "0.3cm";
            row = table.AddRow();
            row.BottomPadding = 5;
            cell = row.Cells[0];
            paragraph = cell.AddParagraph("• For further information see NICE Bites “Lipid modification” Feb 2011 http://www.nelm.nhs.uk/en/NeLM-Area/Health-In-Focus/NICE-Bites-February-2011-No-27-Lipid-modification-update/");
            //paragraph = cell.AddParagraph("");
            //paragraph.Format.LeftIndent = "0.3cm";

            table.SetEdge(0, 0, 1, 7, Edge.Box, BorderStyle.Single, 2.5, Colors.Navy);
            contentSection.Add(table);
            paragraph = contentSection.AddParagraph();
        }

        public override IDictionary<string, LetterUserContent> GetFields()
        {
            Dictionary<string, LetterUserContent> fields = new Dictionary<string, LetterUserContent>();
            fields.Add("Additional information", new LetterUserContent()
            {
                Type = typeof(string),
                DefaultContent = @"The patient currently takes the following over-the-counter or herbal medicines:"
            });
            return fields;
        }



        public override string LetterName
        {
            get { return "GP Statins"; }
        }

        public override string LetterDescription
        {
            get { return "GP Statins"; }
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
