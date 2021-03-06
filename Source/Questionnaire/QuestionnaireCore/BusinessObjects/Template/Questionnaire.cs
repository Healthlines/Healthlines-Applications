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
    public partial class Questionnaire 
    {
        #region Primitive Properties
    
        public virtual int QuestionnaireID
        {
            get;
            set;
        }
    
        public virtual string Name
        {
            get;
            set;
        }
    
        public virtual string CreatedBy
        {
            get;
            set;
        }
    
        public virtual System.DateTime CreationDate
        {
            get;
            set;
        }

        #endregion
        #region Navigation Properties
    
        public virtual ICollection<QuestionnaireQuestionSet> QuestionnaireQuestionSets
        {
            get
            {
                if (_questionnaireQuestionSets == null)
                {
                    var newCollection = new FixupCollection<QuestionnaireQuestionSet>();
                    newCollection.CollectionChanged += FixupQuestionnaireQuestionSets;
                    _questionnaireQuestionSets = newCollection;
                }
                return _questionnaireQuestionSets;
            }
            set
            {
                if (!ReferenceEquals(_questionnaireQuestionSets, value))
                {
                    var previousValue = _questionnaireQuestionSets as FixupCollection<QuestionnaireQuestionSet>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupQuestionnaireQuestionSets;
                    }
                    _questionnaireQuestionSets = value;
                    var newValue = value as FixupCollection<QuestionnaireQuestionSet>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupQuestionnaireQuestionSets;
                    }
                }
            }
        }
        private ICollection<QuestionnaireQuestionSet> _questionnaireQuestionSets;
    
        public virtual ICollection<AnswerSet> AnswerSets
        {
            get
            {
                if (_answerSets == null)
                {
                    var newCollection = new FixupCollection<AnswerSet>();
                    newCollection.CollectionChanged += FixupAnswerSets;
                    _answerSets = newCollection;
                }
                return _answerSets;
            }
            set
            {
                if (!ReferenceEquals(_answerSets, value))
                {
                    var previousValue = _answerSets as FixupCollection<AnswerSet>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupAnswerSets;
                    }
                    _answerSets = value;
                    var newValue = value as FixupCollection<AnswerSet>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupAnswerSets;
                    }
                }
            }
        }
        private ICollection<AnswerSet> _answerSets;

        #endregion
        #region Association Fixup
    
        private void FixupQuestionnaireQuestionSets(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (QuestionnaireQuestionSet item in e.NewItems)
                {
                    item.Questionnaire = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (QuestionnaireQuestionSet item in e.OldItems)
                {
                    if (ReferenceEquals(item.Questionnaire, this))
                    {
                        item.Questionnaire = null;
                    }
                }
            }
        }
    
        private void FixupAnswerSets(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (AnswerSet item in e.NewItems)
                {
                    item.Questionnaire = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (AnswerSet item in e.OldItems)
                {
                    if (ReferenceEquals(item.Questionnaire, this))
                    {
                        item.Questionnaire = null;
                    }
                }
            }
        }

        #endregion
    }
}
