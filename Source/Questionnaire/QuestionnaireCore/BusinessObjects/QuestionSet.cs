// -----------------------------------------------------------------------
// <copyright file="QuestionSet.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Questionnaires.Core.DataAccess;
using Questionnaires.Core.DataAccess.Interfaces;
using Questionnaires.Core.BusinessObjects.Interfaces;
namespace Questionnaires.Core.BusinessObjects
{
   

    /// <summary>
    /// Represents a collection of configured questions 
    /// </summary>
    public partial class QuestionSet : IObjectWithState
    {
        public State State
        {
            get;
            set;
        }
        Dictionary<string, object> _originalValues = new Dictionary<string, object>();
        public Dictionary<string, object> OriginalValues
        {
            get
            {
                return _originalValues;
            }
            set
            {
                _originalValues = value;
            }
        }






        ///// <summary>
        ///// lists all configured question sets
        ///// </summary>
        ///// <returns></returns>
        //public static IList<QuestionSet> List()
        //{
        //    return RepositoryFactory.GetQuestionSetRepository().All().ToList();
        //}

        ///// <summary>
        ///// Creates a new Question Set
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="createdBy"></param>
        ///// <returns></returns>
        //public static QuestionSet Create(string name,string createdBy)
        //{
        //    QuestionSet qs = new QuestionSet();
        //    qs.CreatedBy = createdBy;
        //    qs.CreatedDate = DateTime.Now;
        //    qs.Name = name;
        //    IQuestionSetRepository r = RepositoryFactory.GetQuestionSetRepository();
        //    //using (IQuestionSetRepository r = RepositoryFactory.GetQuestionSetRepository())
        //    //{
        //        qs = r.Create(qs);
        //        //r.SaveChanges();                
        //        r.UnitOfWork.Context.SaveChanges();
        //    //}           
            
        //    return qs;
        //}

        /// <summary>
        /// Deletes a question set
        /// </summary>
        /// <param name="questionSetID"></param>
        //public static void Delete(int questionSetID)
        //{
        //    IQuestionSetRepository r = RepositoryFactory.GetQuestionSetRepository();
        //    //using (IQuestionSetRepository r = RepositoryFactory.GetQuestionSetRepository())
        //    //{
        //        r.Delete(new QuestionSet { QuestionSetId = questionSetID });
        //        //r.SaveChanges();
        //        r.UnitOfWork.Context.SaveChanges();
        //    //}            
        //}
    }
}
