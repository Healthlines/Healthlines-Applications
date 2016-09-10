// -----------------------------------------------------------------------
// <copyright file="Helper.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Questionnaires.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Questionnaires.Core.BusinessObjects.Interfaces;
    using System.Reflection;
    using System.Data.Entity.Infrastructure;

    /// <summary>
    /// Provieds Methods to build original values when entities are created in the QuestionnaireEntities class    /// 
    /// </summary>
    /// <remarks>Not Currently used as this was intended for client side change tracking functionality</remarks>
    public class Helper
    {

        public static Dictionary<string, object> BuildOriginalValues(object entity)
        {
            var result = new Dictionary<string, object>();
            PropertyInfo[] propertyInfos;
            propertyInfos = entity.GetType().GetProperties();

            foreach (var property in propertyInfos)
            {                
                if (property.PropertyType.GetProperties().Count() > 0)
                {
                    result[property.Name] = BuildOriginalValues(property.GetValue(entity,null));
                }
                else
                {
                    result[property.Name] = property.GetValue(entity, null);
                }
            }
            return result;
        }

        public static Dictionary<string, object> BuildOriginalValues(DbPropertyValues originalValues)
        {
            var result = new Dictionary<string, object>();
            foreach (var propertyName in originalValues.PropertyNames)
            {
                var value = originalValues[propertyName];
                if (value is DbPropertyValues)
                {
                    result[propertyName] = BuildOriginalValues((DbPropertyValues)value);
                }
                else
                {
                    result[propertyName] = value;
                }
            }
            return result;
        }
    }
}
