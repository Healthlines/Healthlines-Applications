// -----------------------------------------------------------------------
// <copyright file="IUnitOfWork.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Questionnaires.Core.DataAccess.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;  
    using System.Data.Entity;

    /// <summary>
    /// Represents a unit of work 
    /// </summary>
    /// <remarks>SaveChanges() is probably all we really need here</remarks>
    public interface IUnitOfWork
    {
        DbContext Context { get; set; }
        void SaveChanges();
        bool LazyLoadingEnabled { get; set; }
        bool ProxyCreationEnabled { get; set; }
        string ConnectionString { get; set; }
    }
}
