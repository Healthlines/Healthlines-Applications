// -----------------------------------------------------------------------
// <copyright file="GpInterventionStart.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using System.Reflection;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace NHSD.ElephantParade.DocumentGenerator.Letters.CVD
{
    using System.Collections.Generic;
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Tables;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GpSmoking : BaseLetterTemplate
    {
        protected override void CreateContent(Section contentSection, IDictionary<string, object> values)
        {
            contentSection.AddParagraph("Healthlines Service Referral Form", "Header1");
            contentSection.AddParagraph("Smoking Cessation Therapy", "Header1");

            contentSection.AddParagraph("Dear GP");
            contentSection.AddParagraph("");
            contentSection.AddParagraph(@"This patient is taking part in the Healthlines Service. We have identified that they may benefit from a smoking cessation aid and they appear to be suitable for it. They would like a prescription for a smoking cessation aid, if you agree. We have discussed the advantages and disadvantages of the different treatments with the patient and listed their preference below.

Please note, this letter is a referral for the patient to be considered for a smoking cessation aid. It is at your discretion whether or not to prescribe it. 
");
            contentSection.AddParagraph("");

            Paragraph paragraph = contentSection.AddParagraph("Assessment of nicotine dependence");
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Font.Underline = Underline.Single;
            paragraph.Format.SpaceAfter = 6;

            paragraph = contentSection.AddParagraph();
            paragraph.Format.Borders.Width = 2.5;
            paragraph.Format.Borders.Color = Colors.Navy;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Borders.Distance = 12;

            if (values.ContainsKey("PerDay") && values["PerDay"] != null && ((string)values["PerDay"]).Trim() != string.Empty)
            {
                if (values.ContainsKey("NumberOfYears") && values["NumberOfYears"] != null && ((string)values["NumberOfYears"]).Trim() != string.Empty)
                    paragraph.AddText("The patient smokes " + (string)values["PerDay"] + " cigarettes per day for " +
                                      (string)values["NumberOfYears"] + " years.");
                else
                {
                    paragraph.AddText("The patient smokes " + (string) values["PerDay"] + " cigarettes per day.");
                }
                paragraph.AddLineBreak();
            }

            if (values.ContainsKey("SmokeAfterWaking") && values["SmokeAfterWaking"] != null && ((string)values["SmokeAfterWaking"]).Trim() != string.Empty)
                paragraph.AddText("Smoking within 30 minutes of waking - " + (string)values["SmokeAfterWaking"]);
                paragraph.AddLineBreak();

                if (values.ContainsKey("TriedQuittingBefore") && values["TriedQuittingBefore"] != null && ((string)values["TriedQuittingBefore"]).Trim() != string.Empty)
                paragraph.AddText("Patient has attempted to quit before - " + (string)values["TriedQuittingBefore"]);
                paragraph.AddLineBreak();

            contentSection.AddParagraph();


            string _importantInfo = values.ContainsKey("Additional Information") ? (string)values["Additional Information"] : string.Empty;

            if (_importantInfo.Trim() != string.Empty)
            {
                paragraph = contentSection.AddParagraph("Additional information");
                paragraph.Format.Font.Bold = true;
                paragraph.Format.Font.Underline = Underline.Single;
                paragraph.Format.SpaceAfter = 6;

                contentSection.AddParagraph(_importantInfo);
                contentSection.AddParagraph();
            }

            //Doses
            //#######################

            paragraph = contentSection.AddParagraph("Doses");
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Font.Underline = Underline.Single;
            paragraph.Format.SpaceAfter = 6;

            Table table = new Table();
            table.Borders.Width = 0.0;
            Column column = table.AddColumn(Unit.FromCentimeter(5));
            //Column column = table.AddColumn();
            column.LeftPadding = 10;
            column = table.AddColumn(Unit.FromCentimeter(12));
            column.RightPadding = 10;            
            Row row = table.AddRow();
            row.BottomPadding = 5;
            row.TopPadding = 10;
            var cell = row.Cells[0];
            cell.AddParagraph("Nicotine Replacement ");
            cell = row.Cells[1];
            cell.AddParagraph("See NHS Smokefree website  http://smokefree.nhs.uk/ways-to-quit/patches-gum-and-nicotine-replacement-therapy/ ");
            row = table.AddRow();
            row.BottomPadding = 5;
            cell = row.Cells[0];
            cell.AddParagraph("Bupropion (Zyban®)");
            cell = row.Cells[1];
            cell.AddParagraph(@"150mg once daily for 6 days, then 150mg twice daily. 
For >70 years olds: 150mg once daily throughout the course");
            row = table.AddRow();
            row.BottomPadding = 5;
            cell = row.Cells[0];
            cell.AddParagraph("Varenicline (Champix®)");
            cell = row.Cells[1];
            cell.AddParagraph(@"Days 1-3: 0.5mg once daily. Days 4-7: 0.5 mg twice daily. Day 8 onwards: 1mg twice daily. 
Reduce to 0.5mg twice daily if side effects are intolerable");

            table.SetEdge(0, 0, 2, 3, Edge.Box, BorderStyle.Single, 2.5, Colors.Navy);
            contentSection.Add(table);

            contentSection.AddParagraph();
            paragraph = contentSection.AddParagraph("Further prescribing notes");
            paragraph.Format.Font.Bold = true;
            paragraph.Format.Font.Underline = Underline.Single;
            paragraph.Format.SpaceAfter = 6;


            // Notes
            //##########################

            //ListInfo listinfo = new ListInfo();
            //listinfo.ContinuePreviousList = true;
            //listinfo.ListType = ListType.BulletList1;
            

            table = new Table();
            table.Borders.Width = 0.0;
            column = table.AddColumn(Unit.FromCentimeter(17));
       
            column.LeftPadding = 10;            
            column.RightPadding = 10;
            row = table.AddRow();
            row.TopPadding = 10;
            row.BottomPadding = 5;
            cell = row.Cells[0];
            paragraph = cell.AddParagraph("• Agree on a quit day with the patient. Bupropion and varenicline should usually be");
            //paragraph.Format.ListInfo = listinfo.Clone();
            //paragraph.Style = "BulletList";
            paragraph = cell.AddParagraph("started 1-2 weeks before the quit day.");
            paragraph.Format.LeftIndent = "0.3cm";
            
            row = table.AddRow();
            row.BottomPadding = 5;
            cell = row.Cells[0];
            paragraph = cell.AddParagraph("• For the first prescription, give 2 weeks supply for NRT and 3-4 weeks for bupropion and");
            paragraph = cell.AddParagraph("varenicline. Continue prescribing only if the patient demonstrates on ");
            paragraph.Format.LeftIndent = "0.3cm";
            paragraph = cell.AddParagraph("re-assessment that their quit attempt is continuing.");
            paragraph.Format.LeftIndent = "0.3cm";
            row = table.AddRow();
            row.BottomPadding = 5;
            cell = row.Cells[0];
            paragraph = cell.AddParagraph("• Do not offer Nicotine Replacement Therapy, varenicline or bupropion in any");
            paragraph = cell.AddParagraph("combination.");
            paragraph.Format.LeftIndent = "0.3cm";
            row = table.AddRow();
            row.BottomPadding = 5;
            cell = row.Cells[0];
            paragraph = cell.AddParagraph("• Offer advice, encouragement and support, including referral to the NHS Stop");
            paragraph = cell.AddParagraph("Smoking Service.");
            paragraph.Format.LeftIndent = "0.3cm";
            row = table.AddRow();
            row.BottomPadding = 5;
            cell = row.Cells[0];
            paragraph = cell.AddParagraph("• Bupropion interacts with tamoxifen and several antidepressants. Varenicline is");
            paragraph = cell.AddParagraph("affected by cimetidine. Refer to BNF for further details.   ");
            paragraph.Format.LeftIndent = "0.3cm";

            table.SetEdge(0, 0, 1, 5, Edge.Box, BorderStyle.Single, 2.5, Colors.Navy);
            contentSection.Add(table);
            paragraph = contentSection.AddParagraph();
            paragraph.Format.SpaceAfter = 20;

        }

        protected override void Attachments(PdfDocument pdfDocument)
        {
            var assembly = Assembly.GetCallingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream("NHSD.ElephantParade.DocumentGenerator.Resources.Letter to GP - Smoking Cessation - CVD.pdf"))
            {
                PdfDocument document = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
                // Iterate pages
                int count = document.PageCount;
                for (int idx = 0; idx < count; idx++)
                {
                    // Get the page from the external document...
                    PdfPage page = document.Pages[idx];
                    // ...and add it to the output document.
                    pdfDocument.AddPage(page);
                }
            }
        }

        public override IDictionary<string, LetterUserContent> GetFields()
        {
            Dictionary<string, LetterUserContent> fields = new Dictionary<string, LetterUserContent>();

            fields.Add("Additional Information", new LetterUserContent()
            {
                Type = typeof(string),
                DefaultContent = @"Preferred smoking cessation aid: (NRT/Zyban®/Champix®)"
            });
            return fields;
        }



        public override string LetterName
        {
            get { return "GP Smoking"; }
        }

        public override string LetterDescription
        {
            get { return "GP Smoking Letter"; }
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
