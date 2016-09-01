using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHSD.ElephantParade.DocumentGenerator.Letters;
using NHSD.ElephantParade.Domain.Models;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using System.Data;
using System.Collections;
using System.Reflection;

namespace NHSD.ElephantParade.DocumentGenerator.BloodPressureReadings
{
    class BloodPressureDocument : BaseLetterTemplate
    {
        IList<BloodPressureReadingViewModel> _bpReadings;

        public BloodPressureDocument(IList<BloodPressureReadingViewModel> bpReadingsVM)
        {
            this._bpReadings = bpReadingsVM;
        }

        public override string LetterName
        {
            get { return "Blood Pressure Readings"; }
        }

        public override string LetterDescription
        {
            get { return "List of Blood Pressure Readings"; }
        }

        public override Domain.Models.LetterTarget DefaultLetterTarget
        {
            get { return Domain.Models.LetterTarget.GP; }
        }

        protected override void CreateContent(MigraDoc.DocumentObjectModel.Section contentSection, IDictionary<string, object> values)
        {
            Table table;

            // Each MigraDoc document needs at least one section.
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

            // Before you can add a row, you must define the columns
            Column column;

            for (int i = 0; i <= 2; i++)
            {
                column = table.AddColumn(Unit.FromCentimeter(6));
                column.Format.Alignment = ParagraphAlignment.Center;
            }

            // Create the header of the table
            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = TableBlue;
            row.Cells[0].AddParagraph("Date");
            row.Cells[1].AddParagraph("Blood Pressure");
            row.Cells[2].AddParagraph("Status");

            if (_bpReadings.Count > 0)
            {
                Row row1;

                foreach (var item in _bpReadings)
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
            else
            {
                Paragraph paragraph;
                paragraph = _document.LastSection.AddParagraph("There are no blood pressure readings for this patient.");
            }

            _document.LastSection.AddParagraph("");
        }

        public override IDictionary<string, LetterUserContent> GetFields()
        {
            return new Dictionary<string, LetterUserContent>();
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