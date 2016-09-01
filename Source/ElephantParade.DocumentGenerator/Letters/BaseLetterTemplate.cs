// -----------------------------------------------------------------------
// <copyright file="BaseLetterTemplate.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.DocumentGenerator.Letters
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
    {
        protected Document _document;
        protected PdfDocument _pdfDocument;
        protected Address _heathlinesAddress;
        protected StudyPatient _patient = null;
        protected string _signatory = string.Empty;
        protected string _signatoryImgPath = string.Empty;
        protected string _imgLetterLogoPath = string.Empty;

        public BaseLetterTemplate()
        {
                                  
        }

        public virtual PdfDocument  CreateDocument(StudyPatient patient, Address heathlinesAddress, string signatoryName, string signatoryImgPath, string imgLetterLogoPath, IDictionary<string, object> values)
        {
            _patient = patient;
            _heathlinesAddress = heathlinesAddress;
            _signatory = signatoryName ?? string.Empty; 
            _signatoryImgPath = signatoryImgPath;
            _imgLetterLogoPath = imgLetterLogoPath; 

            this._document = new Document();
            this._document.Info.Title = "Letter";
            this._document.Info.Subject = "Healthlines Study";
            this._document.Info.Author = "NHS Direct";

            DefineStyles(_document);

            CreateHeader(_document);

            CreateContent(_document.LastSection,values);

            CreateSignatory(_document);

            return Render();
        }
  
        #region abstract Methods
        public abstract string LetterName{get;}
        public abstract string LetterDescription{get;}
        public abstract LetterTarget DefaultLetterTarget { get;}
        protected abstract void CreateContent(Section contectSection, IDictionary<string, object> values);
        public abstract IDictionary<string, LetterUserContent> GetFields();
      
        #endregion
        
        #region Protected Methods

        /// <summary>
        /// render the output
        /// </summary>
        /// <returns></returns>
        protected virtual PdfDocument Render()
        {
            _pdfDocument = new PdfDocument();

            // Create a renderer and prepare (=layout) the document
            MigraDoc.Rendering.DocumentRenderer docRenderer = new DocumentRenderer(_document);
            docRenderer.PrepareDocument();
            int pageCount = docRenderer.FormattedDocument.PageCount;
            for (int idx = 0; idx < pageCount; idx++)
            {
                PdfPage page = _pdfDocument.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                // HACK²
                gfx.MUH = PdfFontEncoding.Unicode;
                gfx.MFEH = PdfFontEmbedding.Default;
                docRenderer.RenderPage(gfx, idx + 1);
            }

            Attachments(_pdfDocument);

            return _pdfDocument;
           
        }

        /// <summary>
        /// allows for overloaded method to add attachments
        /// </summary>
        /// <param name="pdfDocument"></param>
        protected virtual void Attachments(PdfDocument pdfDocument)
        {

        }

        protected virtual void DefineStyles(Document document)
        {
            // Get the predefined style Normal.
            Style style = document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            //style.Font.Name = "Frutiger";

            

            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);


            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);
            style.Font.Color = Colors.Gray;

            style = document.Styles.AddStyle("SignatureName", "Normal");
            style.Font.Bold = true;



            // Create a new style called Title based on style Normal
            style = document.Styles.AddStyle("Header1", "Normal");
            style.Font.Size = 14;
            style.Font.Bold = true;
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);

            // Create a new style called Question based on style Normal
            style = document.Styles.AddStyle("Header2", "Header1");
            style.Font.Size = 12;
            style.Font.Bold = true;
            style.ParagraphFormat.SpaceBefore = "8mm";
            style.ParagraphFormat.SpaceAfter = "3mm";

            // Create a new style called Question based on style Normal
            style = document.Styles.AddStyle("Header3", "Header2");
            style.Font.Size = 10;
            style.Font.Bold = true;
            style.ParagraphFormat.SpaceBefore = "2mm";
            style.ParagraphFormat.SpaceAfter = "2mm";

            // Create a new style called TextBox based on style Normal
            style = document.Styles.AddStyle("TextBox", "Normal");
            style.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
            style.ParagraphFormat.Borders.Width = 2.5;
            style.ParagraphFormat.Borders.Distance = "3pt";
            //TODO: Colors
            style.ParagraphFormat.Shading.Color = Colors.LightBlue;

            style = document.Styles.AddStyle("TextBoxPlain", "Normal");
            style.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            style.ParagraphFormat.Borders.Width = 2.5;
            style.ParagraphFormat.Borders.Distance = "12pt";
            //TODO: Colors
            style.ParagraphFormat.Borders.Color = Colors.Navy;
            style.ParagraphFormat.Shading.Color = Colors.White;

            style = _document.AddStyle("BulletList", "Normal");
            style.ParagraphFormat.LeftIndent = "0.3cm"; 
        }

        protected virtual void CreateHeader(Document document)
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
            paragraph.Format.Alignment = ParagraphAlignment.Right;
            paragraph.AddLineBreak();
            outputAddress(paragraph, _heathlinesAddress);

            #endregion

            #region GP Address

            if (_patient.GPPractice.Name != null)
            {
                paragraph = section.AddParagraph();
                paragraph.Format.Alignment = ParagraphAlignment.Left;
                paragraph.AddText(_patient.GPPractice.Name);
            }

            if (_patient.GPPractice.Practice != null)
            {
                paragraph = section.AddParagraph();
                paragraph.Format.Alignment = ParagraphAlignment.Left;
                paragraph.AddText(_patient.GPPractice.Practice);
            }

            paragraph = section.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            outputAddress(paragraph, Address.Convert(_patient.GPPractice.Address));
            paragraph.Format.SpaceAfter = 12;

            paragraph = section.AddParagraph();
            paragraph.AddDateField("dd/MM/yyyy");
            paragraph.Format.SpaceAfter = 6;

            #endregion

            #region Patient Details

            PatientDetails(section);

            #endregion
        }

        protected void PatientDetails(Section section)
        {
            Paragraph paragraph = section.AddParagraph();


            paragraph.AddFormattedText("Patient's name: ", TextFormat.Bold);

            string name = _patient.Title ?? "";
            if (name.Trim().Length > 0 && !name.EndsWith(" "))
                name = name + " ";
            name = name + _patient.Forename ?? "";
            if (name.Trim().Length > 0 && !name.EndsWith(" "))
                name = name + " ";
            name = name + _patient.Surname ?? "";

            string address = _patient.Address.Line1 ?? "";
            if (address.Trim().Length > 0 && !address.EndsWith(" "))
                address = address + ", ";
            address = address + _patient.Address.Line2 ?? "";
            if (address.Trim().Length > 0 && !address.EndsWith(" "))
                address = address + ", ";
            address = address + _patient.Address.PostCode ?? "";

            paragraph.AddText(name);
            paragraph.AddLineBreak();
            paragraph.AddFormattedText("Patient's date of birth:  ", TextFormat.Bold);
            paragraph.AddText(_patient.DOB.HasValue ? _patient.DOB.Value.ToShortDateString() : "");
            paragraph.AddLineBreak();
            paragraph.AddFormattedText("Address: ", TextFormat.Bold);
            paragraph.AddText(address);
            paragraph.AddLineBreak();
            paragraph.AddFormattedText("NHS number: ", TextFormat.Bold);
            paragraph.AddText(_patient.NhsNumber ?? "");
            paragraph.AddLineBreak();
            paragraph.AddFormattedText("Patient's Healthlines study number: ", TextFormat.Bold);
            paragraph.AddText(_patient.StudyTrialNumber ?? "");
            paragraph.AddLineBreak();
            paragraph.Format.SpaceAfter = 30;
        }

        protected void outputAddress(Paragraph paragraph, Address address)
        {
            if (address != null)
            {
                if (address.Name != null && address.Name.Trim().Length > 0)
                {
                    paragraph.AddText(address.Name);
                    paragraph.AddLineBreak();
                }
                if (address.Line1 != null && address.Line1.Trim().Length > 0)
                {
                    paragraph.AddText(address.Line1);
                    paragraph.AddLineBreak();
                }
                if (address.Line2 != null && address.Line2.Trim().Length > 0)
                {
                    paragraph.AddText(address.Line2);
                    paragraph.AddLineBreak();
                }
                if (address.Town != null && address.Town.Trim().Length > 0)
                {
                    paragraph.AddText(address.Town);
                    paragraph.AddLineBreak();
                }
                if (address.County != null && address.County.Trim().Length > 0)
                {
                    paragraph.AddText(address.County);
                    paragraph.AddLineBreak();
                }
                if (address.Postcode != null && address.Postcode.Trim().Length > 0)
                {
                    paragraph.AddText(address.Postcode);
                    paragraph.AddLineBreak();
                }
                if (address.Telephone != null && address.Telephone.Trim().Length > 0)
                {
                    paragraph.AddText(address.Telephone);
                    paragraph.AddLineBreak();
                }
                if (address.Email != null && address.Email.Trim().Length > 0)
                {
                    paragraph.AddText(address.Email);
                    paragraph.AddLineBreak();
                }
            }
        }

        protected virtual void CreateSignatory(Document document)
        {
            Section section = document.LastSection;
            section.AddParagraph();
            Paragraph p = section.AddParagraph("Kind regards");
            p.Format.SpaceAfter = 8;
            
            if (!String.IsNullOrEmpty(_signatoryImgPath) && File.Exists(_signatoryImgPath))
            {
                p = section.AddParagraph();
                //p.Format.LeftIndent = "10";
                p.AddImage(_signatoryImgPath);
            }
            else
            {
                p = section.AddParagraph();
                var sig = _signatory.Split(new string[] { "." },StringSplitOptions.None);  
                string name =_signatory;
                if(sig.Count()>1)
                 name = char.ToUpper(sig[0][0]) + sig[0].Substring(1);
                
                p.AddFormattedText(name ,"SignatureName");
            }
            p.Format.SpaceAfter = 8;

            p = section.AddParagraph("Healthlines Service");           
        }
                
        #endregion

        
    }
}
