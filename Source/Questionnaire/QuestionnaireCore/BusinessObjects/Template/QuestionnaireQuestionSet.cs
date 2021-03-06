//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Questionnaires.Core.BusinessObjects
{
    public partial class QuestionnaireQuestionSet 
    {
        #region Primitive Properties
    
        public virtual int QuestionnaireQuestionSetsID
        {
            get;
            set;
        }
    
        public virtual int Order
        {
            get;
            set;
        }
    
        public virtual int QuestionnaireID
        {
            get { return _questionnaireID; }
            set
            {
                if (_questionnaireID != value)
                {
                    if (Questionnaire != null && Questionnaire.QuestionnaireID != value)
                    {
                        Questionnaire = null;
                    }
                    _questionnaireID = value;
                }
            }
        }
        private int _questionnaireID;
    
        public virtual int QuestionSetID
        {
            get { return _questionSetID; }
            set
            {
                if (_questionSetID != value)
                {
                    if (QuestionSet != null && QuestionSet.QuestionSetID != value)
                    {
                        QuestionSet = null;
                    }
                    _questionSetID = value;
                }
            }
        }
        private int _questionSetID;

        #endregion
        #region Navigation Properties
    
        public virtual Questionnaire Questionnaire
        {
            get { return _questionnaire; }
            set
            {
                if (!ReferenceEquals(_questionnaire, value))
                {
                    var previousValue = _questionnaire;
                    _questionnaire = value;
                    FixupQuestionnaire(previousValue);
                }
            }
        }
        private Questionnaire _questionnaire;
    
        public virtual QuestionSet QuestionSet
        {
            get { return _questionSet; }
            set
            {
                if (!ReferenceEquals(_questionSet, value))
                {
                    var previousValue = _questionSet;
                    _questionSet = value;
                    FixupQuestionSet(previousValue);
                }
            }
        }
        private QuestionSet _questionSet;

        #endregion
        #region Association Fixup
    
        private void FixupQuestionnaire(Questionnaire previousValue)
        {
            if (previousValue != null && previousValue.QuestionnaireQuestionSets.Contains(this))
            {
                previousValue.QuestionnaireQuestionSets.Remove(this);
            }
    
            if (Questionnaire != null)
            {
                if (!Questionnaire.QuestionnaireQuestionSets.Contains(this))
                {
                    Questionnaire.QuestionnaireQuestionSets.Add(this);
                }
                if (QuestionnaireID != Questionnaire.QuestionnaireID)
                {
                    QuestionnaireID = Questionnaire.QuestionnaireID;
                }
            }
        }
    
        private void FixupQuestionSet(QuestionSet previousValue)
        {
            if (previousValue != null && previousValue.QuestionnaireQuestionSets.Contains(this))
            {
                previousValue.QuestionnaireQuestionSets.Remove(this);
            }
    
            if (QuestionSet != null)
            {
                if (!QuestionSet.QuestionnaireQuestionSets.Contains(this))
                {
                    QuestionSet.QuestionnaireQuestionSets.Add(this);
                }
                if (QuestionSetID != QuestionSet.QuestionSetID)
                {
                    QuestionSetID = QuestionSet.QuestionSetID;
                }
            }
        }

        #endregion
    }
}
