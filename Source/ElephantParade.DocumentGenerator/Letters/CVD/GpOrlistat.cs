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
    public class GpOrlistat : BaseLetterTemplate
    {
         

        protected override void CreateContent(Section contentSection, IDictionary<string, object> values)
        {
            contentSection.AddParagraph("Healthlines Service Patient Assessment Form", "Header1");
            contentSection.AddParagraph("Orlistat (Xenical®) Therapy", "Header1");

            contentSection.AddParagraph("Dear GP");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("This patient is taking part in the Healthlines Service. We have identified that they may benefit from orlistat and they appear to be suitable for it. We have discussed the advantages and disadvantages of orlistat with the patient and other ways in which they can change their diet and lifestyle to help them lose weight. They would like a prescription for orlistat, if you agree. ");
            contentSection.AddParagraph("");
            contentSection.AddParagraph("Please note, this letter is a referral for the patient to be considered for orlistat. It is at your discretion whether or not to prescribe it. ");
            contentSection.AddParagraph("");

            var p = contentSection.AddParagraph("Weight/Height/BMI of patient");
            p.Format.Font.Underline = Underline.Single;
            p.Format.Font.Bold = true;
            p.Format.SpaceAfter = 6;

            Table table = new Table();
            table.Borders.Width = 0.0;
            table.Rows.LeftIndent = 0;
            Column column = table.AddColumn(contentSection.Document.DefaultPageSetup.PageWidth - contentSection.Document.DefaultPageSetup.LeftMargin - contentSection.Document.DefaultPageSetup.RightMargin);
            //Column column = table.AddColumn();
            
            column.LeftPadding = 10;
            column.RightPadding = 10;
            Row row = table.AddRow();
           
            row.TopPadding = 10;
            var cell = row.Cells[0];
            cell.Format.Alignment = ParagraphAlignment.Center;
            decimal weight = (values.ContainsKey("Weight kg")?(decimal)values["Weight kg"]:0);
            decimal height = (values.ContainsKey("Height meters") ? (decimal)values["Height meters"] : 0);
            decimal BMI = 0;
            if (height >0 && weight > 0)
                BMI = (weight / height) / height;
            cell.AddParagraph("The patient weighs " + (weight > 0 ? weight.ToString() : "______") + " kg and height is " + (height>0?height.ToString() : "______") + " m");
            row = table.AddRow();
         
            row.TopPadding = 10;
            cell = row.Cells[0];
            cell.Format.Alignment = ParagraphAlignment.Center;
            cell.AddParagraph("This equates to a BMI of " + (BMI > 0 ? BMI.ToString("F") : "______") + "");
            row = table.AddRow();

            row.TopPadding = 10;
            cell = row.Cells[0];
            cell.AddParagraph("Orlistat is indicated if BMI ≥30 kg/m² or BMI ≥28 kg/m² and other risk factors.");
            row = table.AddRow();
        
            cell = row.Cells[0];
            cell.AddParagraph("This patient has a BMI ≥28 kg/m² and a CVD risk of ≥20% so orlistat is indicated");
            row = table.AddRow();
        
            row.BottomPadding = 10;
            table.SetEdge(0, 0, 1, 5, Edge.Box, BorderStyle.Single, 2.5, Colors.Navy);
            contentSection.Add(table);


            p = contentSection.AddParagraph("Dose of orlistat (Xenical®)");
            p.Format.Font.Underline = Underline.Single;
            p.Format.Font.Bold = true;
            p.Format.SpaceBefore = 15;
            p.Format.SpaceAfter = 6;

            p = contentSection.AddParagraph("One x 120 mg capsule taken with water immediately before, during or up to one hour after each main meal. Use of a multivitamin supplement could be considered. If a multivitamin supplement is recommended, it should be taken at least two hours after the administration of orlistat or at bedtime.");
            p.Style = "TextBox";

            string _additionalInfo = values.ContainsKey("Medicines") ? (string)values["Medicines"] : string.Empty;

            if (_additionalInfo!=null && _additionalInfo.Trim() != string.Empty)
            {
                p = contentSection.AddParagraph("");
                p = contentSection.AddParagraph("Additional information (e.g. prescribed / over-the-counter / herbal medicines)");
                p.Format.Font.Bold = true;
                p.Format.Font.Underline = Underline.Single;
                p.Format.SpaceAfter = 6;

                contentSection.AddParagraph(_additionalInfo);
                contentSection.AddParagraph();
            }

            p = contentSection.AddParagraph(@"• Orlistat is susceptible to several drug interactions. For further details on these and other information refer to the SPC monograph as follows:

http://www.medicines.org.uk/EMC/medicine/1746/SPC/Xenical+120mg+hard+capsules/

• Orlistat may indirectly reduce the availability of oral contraceptives and lead to unexpected pregnancies in some individual cases. An additional contraceptive method is recommended in case of severe diarrhoea. 
");
            p.Style = "TextBox";
           p.Format.Alignment = ParagraphAlignment.Left;

            contentSection.AddParagraph("");
        }



        public override IDictionary<string, LetterUserContent> GetFields()
        {
            Dictionary<string, LetterUserContent> fields = new Dictionary<string, LetterUserContent>();
                    
            fields.Add("Weight kg", new LetterUserContent()
            {
                Type = typeof(decimal),
                DefaultContent = @""
            });
            fields.Add("Height meters", new LetterUserContent()
            {
                Type = typeof(decimal),
                DefaultContent = @""
            });

            return fields;
        }



        public override string LetterName
        {
            get { return "GP Orlistat"; }
        }

        public override string LetterDescription
        {
            get { return "GP Orlistat"; }
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
