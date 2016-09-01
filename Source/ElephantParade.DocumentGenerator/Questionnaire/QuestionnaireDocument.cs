// -----------------------------------------------------------------------
// <copyright file="DepressionQuestionnaireDocument.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.DocumentGenerator.Questionnaire
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Shapes;
    using System.IO;
    using NHSD.ElephantParade.Domain.Models;
    using System.Text.RegularExpressions;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class QuestionnaireDocument
    {
        /// <summary>
        /// The MigraDoc document that represents the questionnaire.
        /// </summary>
        Document _document;
        private QuestionnaireResults _questionnaireResults;
        readonly static Color TableGray = new Color(242, 242, 242);
        //IList<AnswerSetAnswer> _answers;
        //AnswerSet _answerSet;

      
        public QuestionnaireDocument(QuestionnaireResults questionnaire)
        {
            // TODO: Complete member initialization
            this._questionnaireResults = questionnaire;
        }

        public Document CreateDocument(string imgLogoPath = null)
        {
            this._document = new Document();
            this._document.Info.Title = _questionnaireResults.QuestionnaireTitle;
            this._document.Info.Subject = "Long Term Conditions Study";
            this._document.Info.Author = "Healthlines";

            DefineStyles();

            CreateCoverPage(imgLogoPath);

            DefineContentSection(this._document.LastSection,null);

            FillContent();

            return this._document;
        }

        private void DefineStyles()
        {
            // Get the predefined style Normal.
            Style style = this._document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Verdana";

            style = this._document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = this._document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);
            style.Font.Color = Colors.Gray;

            // Create a new style called Title based on style Normal
            style = this._document.Styles.AddStyle("Title", "Normal");
            style.Font.Name = "Tahoma";
            style.Font.Size = 14;
            style.Font.Bold = true;
            style.Font.Color = Colors.DarkGreen;
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);

            // Create a new style called Question based on style Normal
            style = this._document.Styles.AddStyle("Header2", "Title");
            style.Font.Size = 12;
            style.Font.Bold = true;
            style.ParagraphFormat.SpaceBefore = "8mm";
            style.ParagraphFormat.SpaceAfter = "3mm";

            // Create a new style called Question based on style Normal
            style = this._document.Styles.AddStyle("Header3", "Title");
            style.Font.Size = 10;
            style.Font.Bold = true;
            style.ParagraphFormat.SpaceBefore = "2mm";
            style.ParagraphFormat.SpaceAfter = "2mm";

            // Create a new style called Question based on style Normal
            style = this._document.Styles.AddStyle("Question", "Normal");
            style.Font.Size = 10;
            style.Font.Bold = false;

            // Create a new style called Question based on style Normal
            style = this._document.Styles.AddStyle("Explaination", "Normal");
            style.Font.Size = 8;
            style.Font.Italic = true;
            style.Font.Bold = false;
            style.ParagraphFormat.SpaceAfter = 12;

            // Create a new style called Answer based on style Normal
            style = this._document.Styles.AddStyle("Answer", "Question");
            style.ParagraphFormat.Shading.Color = TableGray;
            style.ParagraphFormat.LeftIndent = "1cm";
            style.ParagraphFormat.SpaceAfter = "1mm";
      
        }

        private void CreateCoverPage(string imgLogoPath)
        {
            // Each MigraDoc document needs at least one section.
            Section section = this._document.AddSection();

            Paragraph paragraph;
            
            if (imgLogoPath != null && File.Exists(imgLogoPath))
            {
                //adds image to first page only
                paragraph = section.AddParagraph();
                paragraph.Format.Alignment = ParagraphAlignment.Right;
                paragraph.AddImage(imgLogoPath);
            }

            // Add the print date field
            paragraph = section.AddParagraph();            
            //paragraph.Format.SpaceBefore = "1cm";
            paragraph.Style = "Title";
            paragraph.AddFormattedText("Study: " + _questionnaireResults.StudyID, TextFormat.Bold);

            paragraph = section.AddParagraph();
            //paragraph.Format.SpaceBefore = "1cm";
            paragraph.Style = "Title";            
            paragraph.AddFormattedText("Questionnaire: " + _questionnaireResults.QuestionnaireTitle, TextFormat.Bold);
            paragraph.AddTab();
            if(_questionnaireResults.StartDate > DateTime.MinValue )
                paragraph.AddText(_questionnaireResults.StartDate.ToShortDateString());
            else if (_questionnaireResults.EndDate.HasValue)
                paragraph.AddText(_questionnaireResults.EndDate.Value.ToShortDateString());
            
        }

        private void DefineContentSection(Section section, string imgLogoPath)
        {            
            
            //Create Header
            if (imgLogoPath != null && File.Exists(imgLogoPath))
            {
                HeaderFooter header = section.Headers.Primary;               

                //adds to every page in the header
                Image image = header.AddImage(imgLogoPath);

                image.Height = "1.5cm";
                image.LockAspectRatio = true;
                image.RelativeVertical = RelativeVertical.Line;
                image.RelativeHorizontal = RelativeHorizontal.Margin;
                image.Top = ShapePosition.Top;
                image.Left = ShapePosition.Right;
                image.WrapFormat.Style = WrapStyle.Through;
            }


            // Create footer
            Paragraph paragraph = section.Footers.Primary.AddParagraph();
            paragraph.AddText("Healthines");
            paragraph.Format.Font.Size = 9;
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            //start page number
            section.PageSetup.StartingNumber = 1;    
            // Create a paragraph with centered page number. See definition of style "Footer".
            paragraph = new Paragraph();
            paragraph.AddTab();
            paragraph.AddPageField();
            paragraph.Format.Alignment = ParagraphAlignment.Right;
            // Add paragraph to footer for odd pages.
            section.Footers.Primary.Add(paragraph);
            // Add clone of paragraph to footer for odd pages. Cloning is necessary because an object must
            // not belong to more than one other object. If you forget cloning an exception is thrown.
            section.Footers.EvenPage.Add(paragraph.Clone());
        }

        private void FillContent()
        {
            int i = 1;
            foreach (var item in _questionnaireResults.Questions)
            {
                Paragraph paragraph;

                paragraph = _document.LastSection.AddParagraph("Question " + i, "Header2");
                if (item.QuestionTitle!=null && item.QuestionTitle.Trim().Length > 0)
                    paragraph.AddFormattedText(string.Format(" ({0})",item.QuestionTitle), "Question");
                if (item.QuestionExplaination.Trim().Length > 0)
                {
                    paragraph = _document.LastSection.AddParagraph(StripTagsRegexCompiled(item.QuestionExplaination.Trim()),"Explaination");
                }
                if (item.QuestionText.Trim().Length > 0)
                {
                    _document.LastSection.AddParagraph(StripTagsRegexCompiled(item.QuestionText.Trim()), "Question");
                }
                paragraph = _document.LastSection.AddParagraph("Answers", "Header3");
                paragraph.Format.Font.Bold = true;
                paragraph.Format.SpaceBefore = "2mm";
                paragraph.Format.SpaceAfter = "2mm";
                
                foreach (var answer in item.Answers)
                {
                    paragraph = _document.LastSection.AddParagraph(answer, "Answer");            
                }
                i++;
            }
            _document.LastSection.AddParagraph("");        
        }

        /// <summary>
        /// Remove HTML from string with compiled Regex.
        /// </summary>
        private string StripTagsRegexCompiled(string source)
        {
            Regex regexBR = new Regex(
                        @"<br\s*/?>",
                        RegexOptions.IgnoreCase
                        | RegexOptions.CultureInvariant
                        | RegexOptions.IgnorePatternWhitespace
                        | RegexOptions.Compiled
                    );           
            source = regexBR.Replace(source,  System.Environment.NewLine);
            source = source.Replace("&nbsp;", " ");
            Regex regex = new Regex(
                        @"<.*?>",
                        RegexOptions.IgnoreCase
                        | RegexOptions.CultureInvariant
                        | RegexOptions.IgnorePatternWhitespace
                        | RegexOptions.Compiled
                    );
            
            return regex.Replace(source, string.Empty);
        }
        
    }
}
