// -----------------------------------------------------------------------
// <copyright file="IRepository.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Questionnaires.DAL.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data.Entity;
    using System.Linq.Expressions;
    using System.Data;
    using Questionnaires.Core.DataAccess.Interfaces;
    using System.Data.Entity.Validation;
    using System.Diagnostics;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Design.PluralizationServices;
    using System.Globalization;

    /// <summary>
    /// Represents a base Repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Repository<T> : IRepository<T>
    where T : class
    {
        #region Fields

        private readonly PluralizationService _pluralizer = PluralizationService.CreateService(CultureInfo.GetCultureInfo("en"));
        private IDbSet<T> _objectset;
        //protected QuestionnaireEntities _context;
        //private bool shareContext = false;
        #endregion

        #region Ctor

        public Repository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
        //public Repository(QuestionnaireEntities context)
        //{
        //    _context = context;
        //    shareContext = true;
        //}
        //public Repository()
        //{
        //    _context = new QuestionnaireEntities();
        //}

        //public Repository(QuestionnaireEntities context)
        //{
        //    _context = context;
        //    shareContext = true;
        //}
        #endregion

        #region Properties

        //protected IDbSet<T> DbSet
        //{
        //    get
        //    {
        //        return _context.Set<T>();
        //    }
        //}

        public IUnitOfWork UnitOfWork { get; set; }
        
        protected IDbSet<T> DbSet
        {
            get
            {
                if (_objectset == null)
                {
                    _objectset = UnitOfWork.Context.Set<T>();
                }
                return _objectset;
            }
        }

        #endregion

        #region Methods

        public virtual IQueryable<T> All()
        {
            return DbSet.AsQueryable();
        }

        public virtual IQueryable<T> Filter(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Where(predicate).AsQueryable<T>();
        }

        public virtual IQueryable<T> Filter(Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50)
        {
            int skipCount = index * size;
            var _resetSet = filter != null ? DbSet.Where(filter).AsQueryable() : DbSet.AsQueryable();
            _resetSet = skipCount == 0 ? _resetSet.Take(size) : _resetSet.Skip(skipCount).Take(size);
            total = _resetSet.Count();
            return _resetSet.AsQueryable();
        }

        public bool Contains(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Count(predicate) > 0;
        }

        public virtual T Find(params object[] keys)
        {
            return DbSet.Find(keys);
        }

        public virtual T Find(Expression<Func<T, bool>> predicate)
        {
            return DbSet.FirstOrDefault(predicate);
        }

        public virtual T Create(T TObject)
        {
            var newEntry = DbSet.Add(TObject);
            return newEntry;
        }

        public virtual int Count
        {
            get
            {
                return DbSet.Count();
            }
        }

        public virtual void Update(T TObject)
        {
            //var entry = _context.Entry(TObject);
            var entry = UnitOfWork.Context.Entry(TObject);
            DbSet.Attach(TObject);
            entry.State = EntityState.Modified;
        }

        public virtual void Delete(T TObject)
        {
            DbSet.Remove(TObject);
        }

        public virtual void Delete(Expression<Func<T, bool>> predicate)
        {
            var objects = Filter(predicate);
            foreach (var obj in objects)
                DbSet.Remove(obj);
        }
        
        //public virtual int SaveChanges()
        //{
        //    try
        //    {
        //        return _context.SaveChanges();
        //    }
                           
        //    catch (DbEntityValidationException dbEx)
        //    {
        //        foreach (var validationErrors in dbEx.EntityValidationErrors)
        //        {
        //            foreach (var validationError in validationErrors.ValidationErrors)
        //            {
        //                Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
        //            }
        //        }
        //        throw dbEx;
        //    }            
        //}

        protected virtual IQueryable<TEntity> GetQuery<TEntity>() where TEntity : class
        {
            /* with 4.1 release, call GetQuery<TEntity>().AsEnumerable(), there is an exception:
             * ... System.ObjectDisposedException : The ObjectContext instance has been disposed and can no longer be used for operations that require a connection.
             */

            // here is a work around: 
            // - cast DbContext to IObjectContextAdapter then get ObjectContext from it
            // - call CreateQuery<TEntity>(entityName) method on the ObjectContext
            // - perform querying on the returning IQueryable, and it works!
            var entityName = GetEntityName<TEntity>();
            return ((IObjectContextAdapter)UnitOfWork.Context).ObjectContext.CreateQuery<TEntity>(entityName);
        }

        protected virtual IQueryable<TEntity> GetQuery<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return GetQuery<TEntity>().Where(predicate);
        }

        private string GetEntityName<TEntity>() where TEntity : class
        {
            return string.Format("{0}.{1}", ((IObjectContextAdapter)UnitOfWork.Context ).ObjectContext.DefaultContainerName, _pluralizer.Pluralize(typeof(TEntity).Name));
        }

        #endregion
    }
}
