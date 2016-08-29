// -----------------------------------------------------------------------
// <copyright file="FileRepository.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NHSD.ElephantParade.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NHSD.ElephantParade.DAL.Interfaces;
    using NHSD.ElephantParade.DAL.EntityModels;
    using NHSD.ElephantParade.DAL.Infrastructure;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class QuestionnaireActionLetterRepository :
        RepositoryBase<QuestionnaireActionLetter>,
        IQuestionnaireActionLetterRepository
    {
        public QuestionnaireActionLetterRepository() : this(new HealthlinesRepositoryContext()) { }

        public QuestionnaireActionLetterRepository(IRepositoryContext repositoryContext) : base(repositoryContext) { }
    }
}
