// -----------------------------------------------------------------------
// <copyright file="AnswerOptionRepository.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Questionnaires.Core.BusinessObjects;
using Questionnaires.Core.DataAccess.Interfaces;

namespace Questionnaires.DAL.Repositories
{
    

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AnswerOptionRepository : Repository<AnswerOption>, IAnswerOptionRepository
    {
        public AnswerOptionRepository()
            : this(new DALContext())
        {

        }

        public  AnswerOptionRepository(IUnitOfWork unitOfWork)
        :base(unitOfWork)
        {

        }
    }
}
