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
    public partial class QuestionSet 
    {
        #region Primitive Properties
    
        public virtual int QuestionSetID
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
    
        public virtual System.DateTime CreatedDate
        {
            get;
            set;
        }

        #endregion
        #region Navigation Properties
    
        public virtual ICollection<QuestionSetQuestion> QuestionSetQuestions
        {
            get
            {
                if (_questionSetQuestions == null)
                {
                    var newCollection = new FixupCollection<QuestionSetQuestion>();
                    newCollection.CollectionChanged += FixupQuestionSetQuestions;
                    _questionSetQuestions = newCollection;
                }
                return _questionSetQuestions;
            }
            set
            {
                if (!ReferenceEquals(_questionSetQuestions, value))
                {
                    var previousValue = _questionSetQuestions as FixupCollection<QuestionSetQuestion>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupQuestionSetQuestions;
                    }
                    _questionSetQuestions = value;
                    var newValue = value as FixupCollection<QuestionSetQuestion>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupQuestionSetQuestions;
                    }
                }
            }
        }
        private ICollection<QuestionSetQuestion> _questionSetQuestions;
    
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

        #endregion
        #region Association Fixup
    
        private void FixupQuestionSetQuestions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (QuestionSetQuestion item in e.NewItems)
                {
                    item.QuestionSet = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (QuestionSetQuestion item in e.OldItems)
                {
                    if (ReferenceEquals(item.QuestionSet, this))
                    {
                        item.QuestionSet = null;
                    }
                }
            }
        }
    
        private void FixupQuestionnaireQuestionSets(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (QuestionnaireQuestionSet item in e.NewItems)
                {
                    item.QuestionSet = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (QuestionnaireQuestionSet item in e.OldItems)
                {
                    if (ReferenceEquals(item.QuestionSet, this))
                    {
                        item.QuestionSet = null;
                    }
                }
            }
        }

        #endregion
    }
}
