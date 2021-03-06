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
    public partial class AnswerSet 
    {
        #region Primitive Properties
    
        public virtual int AnswerSetID
        {
            get;
            set;
        }
    
        public virtual int QuestionnaireID
        {
            get { return _questionnaireID; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_questionnaireID != value)
                    {
                        if (Questionnaire != null && Questionnaire.QuestionnaireID != value)
                        {
                            Questionnaire = null;
                        }
                        _questionnaireID = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private int _questionnaireID;
    
        public virtual string OperatorID
        {
            get;
            set;
        }
    
        public virtual string ParticipantID
        {
            get;
            set;
        }
    
        public virtual Nullable<int> CurrentQuestionID
        {
            get { return _currentQuestionID; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_currentQuestionID != value)
                    {
                        if (Question != null && Question.QuestionID != value)
                        {
                            Question = null;
                        }
                        _currentQuestionID = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _currentQuestionID;
    
        public virtual System.DateTime StartDate
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> EndDate
        {
            get;
            set;
        }
    
        public virtual bool Completed
        {
            get;
            set;
        }

        #endregion
        #region Navigation Properties
    
        public virtual Question Question
        {
            get { return _question; }
            set
            {
                if (!ReferenceEquals(_question, value))
                {
                    var previousValue = _question;
                    _question = value;
                    FixupQuestion(previousValue);
                }
            }
        }
        private Question _question;
    
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
    
        public virtual ICollection<Answer> Answers
        {
            get
            {
                if (_answers == null)
                {
                    var newCollection = new FixupCollection<Answer>();
                    newCollection.CollectionChanged += FixupAnswers;
                    _answers = newCollection;
                }
                return _answers;
            }
            set
            {
                if (!ReferenceEquals(_answers, value))
                {
                    var previousValue = _answers as FixupCollection<Answer>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupAnswers;
                    }
                    _answers = value;
                    var newValue = value as FixupCollection<Answer>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupAnswers;
                    }
                }
            }
        }
        private ICollection<Answer> _answers;
    
        public virtual ICollection<AnswerSetHistory> AnswerSetHistories
        {
            get
            {
                if (_answerSetHistories == null)
                {
                    var newCollection = new FixupCollection<AnswerSetHistory>();
                    newCollection.CollectionChanged += FixupAnswerSetHistories;
                    _answerSetHistories = newCollection;
                }
                return _answerSetHistories;
            }
            set
            {
                if (!ReferenceEquals(_answerSetHistories, value))
                {
                    var previousValue = _answerSetHistories as FixupCollection<AnswerSetHistory>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupAnswerSetHistories;
                    }
                    _answerSetHistories = value;
                    var newValue = value as FixupCollection<AnswerSetHistory>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupAnswerSetHistories;
                    }
                }
            }
        }
        private ICollection<AnswerSetHistory> _answerSetHistories;

        #endregion
        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupQuestion(Question previousValue)
        {
            if (previousValue != null && previousValue.AnswerSets.Contains(this))
            {
                previousValue.AnswerSets.Remove(this);
            }
    
            if (Question != null)
            {
                if (!Question.AnswerSets.Contains(this))
                {
                    Question.AnswerSets.Add(this);
                }
                if (CurrentQuestionID != Question.QuestionID)
                {
                    CurrentQuestionID = Question.QuestionID;
                }
            }
            else if (!_settingFK)
            {
                CurrentQuestionID = null;
            }
        }
    
        private void FixupQuestionnaire(Questionnaire previousValue)
        {
            if (previousValue != null && previousValue.AnswerSets.Contains(this))
            {
                previousValue.AnswerSets.Remove(this);
            }
    
            if (Questionnaire != null)
            {
                if (!Questionnaire.AnswerSets.Contains(this))
                {
                    Questionnaire.AnswerSets.Add(this);
                }
                if (QuestionnaireID != Questionnaire.QuestionnaireID)
                {
                    QuestionnaireID = Questionnaire.QuestionnaireID;
                }
            }
        }
    
        private void FixupAnswers(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Answer item in e.NewItems)
                {
                    item.AnswerSet = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Answer item in e.OldItems)
                {
                    if (ReferenceEquals(item.AnswerSet, this))
                    {
                        item.AnswerSet = null;
                    }
                }
            }
        }
    
        private void FixupAnswerSetHistories(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (AnswerSetHistory item in e.NewItems)
                {
                    item.AnswerSet = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (AnswerSetHistory item in e.OldItems)
                {
                    if (ReferenceEquals(item.AnswerSet, this))
                    {
                        item.AnswerSet = null;
                    }
                }
            }
        }

        #endregion
    }
}
