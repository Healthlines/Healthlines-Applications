// -----------------------------------------------------------------------
// <copyright file="DALContext.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Questionnaires.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Questionnaires.Core.DataAccess.Interfaces;
using System.Data.Entity;

    /// <summary>
    /// Represents a IUnit of work
    /// </summary>
    public class DALContext:IUnitOfWork
    {
        #region Ctor
        public DALContext()
        {
            this.Context = new QuestionnaireEntities();
           
        }
        #endregion

        #region Properties
        public DbContext Context
        {
            get;
            set;
        }

        public bool LazyLoadingEnabled
        {
            get
            {
                return Context.Configuration.LazyLoadingEnabled;
            }
            set
            {
                Context.Configuration.LazyLoadingEnabled = value;
            }
        }

        public bool ProxyCreationEnabled
        {
            get
            {
                return Context.Configuration.ProxyCreationEnabled;
            }
            set
            {
                Context.Configuration.ProxyCreationEnabled = value;
            }
        }

        public string ConnectionString
        {
            get
            {
                return Context.Database.Connection.ConnectionString;
            }
            set
            {
                Context.Database.Connection.ConnectionString = value;
            }
        }
        #endregion

        #region Methods
        public void SaveChanges()
        {            
            Context.SaveChanges();
        }
        #endregion

    }
}
