using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHSD.ElephantParade.DAL.Interfaces;
using NHSD.ElephantParade.DAL.Infrastructure;
using System.Data.Objects;

namespace NHSD.ElephantParade.DAL
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        //public RepositoryBase() : this(new HealthlinesRepositoryContext()) { }

        public RepositoryBase(IRepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
            _objectSet = repositoryContext.GetObjectSet<T>();
        }

        private IObjectSet<T> _objectSet;
        public IObjectSet<T> ObjectSet
        {
            get { return _objectSet; }
        }

        private IRepositoryContext _repositoryContext;
        public IRepositoryContext RepositoryContext
        {
            get { return _repositoryContext; }
        }

        #region IRepository<T> Members

        public T GetSingle(System.Linq.Expressions.Expression<Func<T, bool>> whereCondition)
        {
            return this.ObjectSet.Where(whereCondition).FirstOrDefault();
        }

        public void Add(T entity)
        {
            this.ObjectSet.AddObject(entity);
        }

        public void Delete(T entity)
        {
            this.ObjectSet.DeleteObject(entity);
        }

        public void Attach(T entity)
        {
            this.ObjectSet.Attach(entity);
        }

        public IList<T> GetAll(System.Linq.Expressions.Expression<Func<T, bool>> whereCondition)
        {
            return this.ObjectSet.Where(whereCondition).ToList<T>();
        }

        public IList<T> GetAll()
        {
            return this.ObjectSet.ToList<T>();
        }

        public IQueryable<T> GetQueryable()
        {
            return this.ObjectSet.AsQueryable<T>();
        }

        public long Count(System.Linq.Expressions.Expression<Func<T, bool>> whereCondition)
        {
            return this.ObjectSet.Where(whereCondition).LongCount<T>();
        }

        public long Count()
        {
            return this.ObjectSet.LongCount<T>();
        }

        public int SaveChanges()
        {
            return this.RepositoryContext.SaveChanges();
        }

        public void TerminateChanges()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

