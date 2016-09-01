// -----------------------------------------------------------------------
// <copyright file="BaseLetterTemplate.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.DocumentGenerator.Letters.CVD
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NHSD.ElephantParade.Domain.Models;
    using MigraDoc.DocumentObjectModel;
    using System.IO;
    using System.IO.IsolatedStorage;
    using PdfSharp.Pdf;
    using MigraDoc.Rendering;
    using PdfSharp.Drawing;
    using MigraDoc.DocumentObjectModel.Shapes;

    /// <summary>
    /// The MigraDoc document that represents a letter.
    /// </summary>
    public abstract class BaseLetterTemplate
        : NHSD.ElephantParade.DocumentGenerator.Letters.BaseLetterTemplate
    {
       
        
        protected override void CreateHeader(Document document)
        {
            // Each MigraDoc document needs at least one section.
            Section section = document.AddSection();

            Paragraph paragraph;

            #region Healthlines header

            if (!String.IsNullOrEmpty(_imgLetterLogoPath) && File.Exists(_imgLetterLogoPath))
            {
                //adds image to first page only
                paragraph = section.AddParagraph();
                paragraph.Format.Alignment = ParagraphAlignment.Right;
                paragraph.AddImage(_imgLetterLogoPath);
            }
                  
            #endregion

            #region Healthlines Address
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceAfter = 120;

            var tf = section.AddTextFrame();
            tf.Left = ShapePosition.Right;
            tf.Width = "7.0cm";
            tf.RelativeHorizontal = RelativeHorizontal.Margin;
            tf.Top = "5.5cm";
            tf.RelativeVertical = RelativeVertical.Page;

            paragraph = tf.AddParagraph(); //section.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Right;
            outputAddress(paragraph, _heathlinesAddress);

            #endregion

            #region GP Address
            tf = section.AddTextFrame();
            tf.Left = ShapePosition.Left;
            tf.Width = "7.0cm";
            tf.RelativeHorizontal = RelativeHorizontal.Margin;
            tf.Top = "5.5cm";
            tf.RelativeVertical = RelativeVertical.Page;
            
            if (_patient.GPPractice.Name != null)
            {
                paragraph = tf.AddParagraph();// section.AddParagraph();
                paragraph.Format.Alignment = ParagraphAlignment.Left;
                paragraph.AddText(_patient.GPPractice.Name);
            }

            if (_patient.GPPractice.Practice != null)
            {
                paragraph = tf.AddParagraph();//section.AddParagraph();
                paragraph.Format.Alignment = ParagraphAlignment.Left;
                paragraph.AddText(_patient.GPPractice.Practice);
            }

            paragraph = tf.AddParagraph();//section.AddParagraph();
            

            //paragraph = section.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            outputAddress(paragraph, NHSD.ElephantParade.DocumentGenerator.Letters.Address.Convert(_patient.GPPractice.Address));
            paragraph.Format.SpaceAfter = 12;

            paragraph = section.AddParagraph();
            paragraph.AddDateField("dd/MM/yyyy");
            paragraph.Format.SpaceAfter = 6;

            #endregion

            #region Patient Details

            PatientDetails(section);

            #endregion
        }

        
    }
}
