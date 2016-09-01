using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHSD.ElephantParade.DAL.Interfaces;
using System.Data.Objects;

namespace NHSD.ElephantParade.DAL.Infrastructure
{
    class HealthlinesRepositoryContext : IRepositoryContext
    {
        private const string OBJECT_CONTEXT_KEY = "NHSD.ElephantParade.DAL.EntityModels.HealthlinesEntities";
        public IObjectSet<T> GetObjectSet<T>()
            where T : class
        {
            return ContextManager.GetObjectContext(OBJECT_CONTEXT_KEY).CreateObjectSet<T>();
        }

        /// <summary>
        /// Returns the active object context
        /// </summary>
        public ObjectContext ObjectContext
        {
            get { return ContextManager.GetObjectContext(OBJECT_CONTEXT_KEY); }
        }

        public int SaveChanges()
        {
            return this.ObjectContext.SaveChanges();
        }

        public void Terminate()
        {
            ContextManager.SetRepositoryContext(null, OBJECT_CONTEXT_KEY);
        }
    }
}

