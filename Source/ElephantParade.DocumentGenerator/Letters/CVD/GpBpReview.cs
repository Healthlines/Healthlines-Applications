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
    using PdfSharp.Pdf.IO;
    using System.IO;
    using PdfSharp.Pdf;
    using MigraDoc.Rendering;
    using System.Resources;
    using System.Reflection;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GpBpReview : BaseLetterTemplate
    {

        IDictionary<string, object> _values = null;

        protected override void CreateContent(Section contentSection, IDictionary<string, object> values)
        {
            //save for use later
            _values = values;

            contentSection.AddParagraph("Healthlines Service Patient Assessment Form", "Header1");
            contentSection.AddParagraph("Request for review of BP medication", "Header1");

            contentSection.AddParagraph("Dear GP");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("This patient is participating in the Healthlines Service. This aims to support people with long term conditions and consists of regular phone calls from a Healthlines advisor, along with helping the patient to make good use of resources available over the internet. For patients with hypertension (and who do not have atrial fibrillation) we also monitor their home blood pressure readings.");
            contentSection.AddParagraph("");
           
            Paragraph p;
            if (values.ContainsKey("Average BP") && ((string)values["Average BP"].ToString().Trim('/') != string.Empty))
            {
                p = contentSection.AddParagraph("•	This patient has an average blood pressure reading of: " +
                                                (string) values["Average BP"]);
                p.Format.LeftIndent = "15";
                p.Format.SpaceAfter = 3;
            }
            p = contentSection.AddParagraph("•	Please see recent readings attached.");
            p.Format.LeftIndent = "15";
            p.Format.SpaceAfter = 3;
            if (values.ContainsKey("Taking Medication") && ((string)values["Taking Medication"]).Trim().Length > 0)
            {
                p = contentSection.AddParagraph("•	" + (string) values["Taking Medication"]);
                p.Format.LeftIndent = "15";
                p.Format.SpaceAfter = 3;
            }
            
            p = contentSection.AddParagraph("•	This patient has a blood pressure above the recommended threshold as set out by NICE (CG127).");
            p.Format.LeftIndent = "15";
            contentSection.AddParagraph("");

            Table table = new Table();
            table.Borders.Width = 0.0;
            Column column = table.AddColumn(contentSection.Document.DefaultPageSetup.PageWidth - contentSection.Document.DefaultPageSetup.LeftMargin - contentSection.Document.DefaultPageSetup.RightMargin);
            //Column column = table.AddColumn();
            column.LeftPadding = 10;
            column.RightPadding = 10;
            Row row = table.AddRow();
            row.TopPadding = 10;
            var cell = row.Cells[0];
            cell.Format.Alignment = ParagraphAlignment.Center;
            cell.AddParagraph("The current NICE recommendations for blood pressure targets are:");
            row = table.AddRow();
            row.TopPadding = 10;
            cell = row.Cells[0];
            cell.AddParagraph("Clinic BP < 140/90; ambulatory/home BP < 135/85 for people aged <80 years");
            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("NB:");
            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("Clinic BP < 130/80; ambulatory/home BP <125/75 for people with type 1 diabetes ");
            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("Clinic BP < 140/80; ambulatory/home BP <135/75 for people with type 2 diabetes");
            row = table.AddRow();
            row.BottomPadding = 10;
            cell = row.Cells[0];
            cell.AddParagraph("Clinic BP < 130/80; ambulatory/home BP <125/75 for people with CKD and diabetes (type 1 or 2)");
            table.SetEdge(0, 0, 1, 6, Edge.Box, BorderStyle.Single, 2.0, Colors.Navy);
            contentSection.Add(table);

            contentSection.AddPageBreak();
            p = contentSection.AddParagraph("Additional information");
            p.Format.Font.Bold = true;
            p.Format.Font.Underline = Underline.Single;
            //p.Format.SpaceBefore = 15;
            p = contentSection.AddParagraph("");
            p = contentSection.AddParagraph(
                    @"We would advise the patient have a blood pressure medication review to optimise drug dosages and/or consider an escalation to next stage of blood pressure treatment as set out in the NICE treatment algorithm below. We have also provided a ‘NICE Bite’ summary of the NICE guidelines on hypertension.

");

            if (values.ContainsKey("Additional Information") && (values["Additional Information"]!=null) && ((string)values["Additional Information"]).Trim().Length > 0)
            {
                p = contentSection.AddParagraph((string)values["Additional Information"]);
                //p.Format.LeftIndent = "15";
                p.Format.SpaceAfter = 12;
            }
                

            p = contentSection.AddParagraph(@"Thank you for your help.

");

            

        }

        protected override void CreateSignatory(Document document)
        {
            base.CreateSignatory(document);


            var contentSection = document.LastSection;
            contentSection.AddPageBreak();


            contentSection.AddParagraph("Please return this form to the Healthlines Service", "Header1");

            contentSection.AddParagraph(@"The Healthlines Study
c/o Solent NHS Trust
Adelaide Health Centre
Western Community Hospital Campus
William Macleod Way
Millbrook
Southampton
SO16 4XE

Telephone: 0345 603 0897
Email:   Snhs.healthlines@nhs.net
");
            contentSection.AddParagraph("");
            PatientDetails(contentSection);
            contentSection.AddParagraph("");
            contentSection.AddParagraph("Please indicate below:");
            contentSection.AddParagraph("");

            var table = new Table();
            table.Borders.Width = 0.5;
            var column = table.AddColumn(Unit.FromCentimeter(1));
            var row = table.AddRow();
            contentSection.Add(table);
            var p = contentSection.AddParagraph("I have changed the patient’s treatment as follows:");
            contentSection.AddParagraph(@"......................................................................

......................................................................

......................................................................

");
            table = new Table();
            table.Borders.Width = 0.5;
            column = table.AddColumn(Unit.FromCentimeter(1));
            row = table.AddRow();
            contentSection.Add(table);
            p = contentSection.AddParagraph("I do not wish to increase the patient’s BP treatment. I understand that this may mean that their BP is above the targets recommended by NICE.  Please do not contact me again unless the patient’s BP reaches or exceeds: ");
            contentSection.AddParagraph(@"

                                       ....../........   ( please enter new target for this patient)


");
            contentSection.AddParagraph("Signed __________________________________");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("GP Name __________________________________");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("Date  __________________________________");


            if (_values.ContainsKey("BP Readings"))
            {

                var bpReadings = _values["BP Readings"] as IList<NHSD.ElephantParade.Domain.Models.BloodPressureReadingViewModel>;

                contentSection.AddPageBreak();

                contentSection.AddParagraph("Blood Pressure Readings", "Header2");

                this.PatientDetails(contentSection);


                Color TableBorder = new Color(81, 125, 192);
                Color TableBlue = new Color(235, 240, 249);
                Color TableGray = new Color(242, 242, 242);

                // Create the item table
                table = contentSection.AddTable();
                table.Style = "Table";
                table.Borders.Color = TableBorder;
                table.Borders.Width = 0.25;
                table.Borders.Left.Width = 0.5;
                table.Borders.Right.Width = 0.5;
                table.Rows.LeftIndent = 0;

                //define columns                
                column = table.AddColumn(Unit.FromCentimeter(3));
                column.Format.Alignment = ParagraphAlignment.Center;
                column = table.AddColumn(Unit.FromCentimeter(3));
                column.Format.Alignment = ParagraphAlignment.Center;
                column = table.AddColumn(Unit.FromCentimeter(6));
                column.Format.Alignment = ParagraphAlignment.Center;
                

                // Create the header of the table
                row = table.AddRow();
                row.HeadingFormat = true;
                row.Format.Alignment = ParagraphAlignment.Center;
                row.Format.Font.Bold = true;
                row.Shading.Color = TableBlue;
                row.Cells[0].AddParagraph("Date");
                row.Cells[1].AddParagraph("Blood Pressure");
                row.Cells[2].AddParagraph("Status");
                Row row1;

                foreach (var item in bpReadings)
                {
                    row1 = table.AddRow();

                    row1.TopPadding = 1.5;

                    row1.Cells[0].AddParagraph(item.DateOfReading.Value.Date.ToShortDateString());
                    row1.Cells[0].Format.Font.Bold = false;
                    row1.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                    row1.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;

                    row1.Cells[1].AddParagraph(string.IsNullOrEmpty(item.BPHighAndLowReading) ? "No Reading" : item.BPHighAndLowReading);
                    row1.Cells[1].Format.Font.Bold = false;
                    row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                    row1.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;

                    row1.Cells[2].AddParagraph(item.BPStatus.DescriptionAttribute());
                    //row1.Cells[2].AddParagraph(string.IsNullOrEmpty(item.BPStatus) ? "Within Target" : item.BPStatus);
                    row1.Cells[2].Format.Font.Bold = false;
                    row1.Cells[2].Format.Alignment = ParagraphAlignment.Left;
                    row1.Cells[2].VerticalAlignment = VerticalAlignment.Bottom;
                }
                table.SetEdge(0, 0, 3, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
            }
        }

        protected override void Attachments(PdfDocument pdfDocument)
        {
            var assembly = Assembly.GetCallingAssembly();           
            
            using (Stream stream = assembly.GetManifestResourceStream("NHSD.ElephantParade.DocumentGenerator.Resources.NICE treatment algorithm for hypertension.pdf"))            
            {
                PdfDocument document = PdfReader.Open(stream,PdfDocumentOpenMode.Import);
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
            fields.Add("Taking Medication", new LetterUserContent()
            {
                DefaultContent = @"We have checked with the patient and they [are / are not] taking their medication as prescribed.",
                Type= typeof(string)
            });
            fields.Add("Average BP", new LetterUserContent()
            {
                Type = typeof(string),
                DefaultContent = @""
            });
            fields.Add("BP Readings", new LetterUserContent()
            {
                DefaultContent = null,
                Type = typeof(IList<NHSD.ElephantParade.Domain.Models.BloodPressureReadingViewModel>)
            });
            fields.Add("Additional Information", new LetterUserContent()
            {
                Type = typeof(string),
                DefaultContent = @"This patient has/has not been advised to make an appointment with you to review their medication."
            });

            return fields;
        }



        public override string LetterName
        {
            get { return "GP BP Review"; }
        }

        public override string LetterDescription
        {
            get { return "GP BP Review letter"; }
        }

        public override Domain.Models.LetterTarget DefaultLetterTarget
        {
            get
            {
                return Domain.Models.LetterTarget.GP;
            }            
        }
    }
    public static class Extenshions
    {
        public static string DescriptionAttribute(this Enum value)
        {
            //Using reflection to get the field info
            FieldInfo info = value.GetType().GetField(value.ToString());

            //Get the Displayname Attributes
            System.ComponentModel.DescriptionAttribute[] attributes = (System.ComponentModel.DescriptionAttribute[])info.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);

            //Only capture the description attribute if it is a concrete result (i.e. 1 entry)
            if (attributes.Length == 1)
            {
                return attributes[0].Description;
            }
            else //Use the value for display if not concrete result
            {
                return value.ToString();
            }
        }
    }
}
