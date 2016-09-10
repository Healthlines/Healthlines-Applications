// -----------------------------------------------------------------------
// <copyright file="GenericConvertor.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Questionnaires.Core.Services.Covertors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using System.Diagnostics;
    using AutoMapper;

    ///// <summary>
    ///// Attempts to convert one oject into another
    ///// </summary>
    ///// <typeparam name="Tone"></typeparam>
    ///// <typeparam name="Ttwo"></typeparam>
    //public class GenericConvertorBase<Tone, Ttwo> //: Questionnaire.Core.Services.Covertors.IConvertor<Tone, Ttwo>
    //    where Tone : class
    //    where Ttwo : class
    //{

    //    public virtual Ttwo Convert(Tone oObject)
    //    {
    //        return GenericConvertor.Convert<Ttwo>(oObject);
    //    }

    //    public virtual Tone Convert(Ttwo oObject)
    //    {
    //        return GenericConvertor.Convert<Tone>(oObject);
    //    }

    //    //public void Fill(Tone source,  Ttwo target)
    //    //{           
    //    //    GenericConvertor.Fill(source, target);
    //    //}

    //    //public void Fill(Ttwo source,  Tone target)
    //    //{        
    //    //    GenericConvertor.Fill(source,  target);
    //    //}
    //}

    /// <summary>
    /// Converts one object to another
    /// </summary>
    /// <remarks> Updated to use Automapper </remarks>
    public class GenericConvertor
    {
        private static bool isAutoMapperConfigured = false;
        public GenericConvertor()
        {
            doMapperConfig();
        }

        public static TOut Convert<TOut>(object source)
            where TOut : class
        {
            if (source == null) return null;
            TOut target = Activator.CreateInstance<TOut>();
            object obj = target;

            GenericConvertor.Fill(source, obj);
            return target;
        }

        //public static TOut Convert<TOut>(object source)
        //    where TOut : class
        //{
        //    doMapperConfig();

        //    if (source == null) return null;
        //    Type ts = source.GetType();
        //    Type td = typeof(TOut);

        //    Mapper.CreateMap(ts, td).IgnoreAllNonExisting(ts,td);
        //    return Mapper.Map<TOut>(source);
        //}

        public static void Fill(object source, object target)
        //where TOut : class
        //where TIn : class
        {
            if (source == null || target == null) return;

            var targetProperties = target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var sourceProperties = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var property in targetProperties)
            {
                if (property.CanWrite)
                {
                    var foundproperty = sourceProperties.FirstOrDefault(p => p.Name == property.Name);
                    if (foundproperty != null)
                    {
                        try
                        {
                            //if the same type set it
                            if (foundproperty.PropertyType == property.PropertyType)
                            {
                                property.SetValue(target, foundproperty.GetValue(source, null), null);
                            }
                            else//attempt to convert it
                            {
                                //check if nullable
                                Type convertToType = Nullable.GetUnderlyingType(property.PropertyType);
                                if (convertToType == null)
                                    convertToType = property.PropertyType;

                                if (property.PropertyType.BaseType != null && property.PropertyType.BaseType.Equals(typeof(System.Enum)))
                                {
                                    object foundValue = foundproperty.GetValue(target, null);
                                    if (IsNumeric(foundValue))
                                        property.SetValue(target, System.Enum.ToObject(convertToType, System.Convert.ToInt32(foundValue)), null);
                                }
                                else
                                {
                                    property.SetValue(target, System.Convert.ChangeType(foundproperty.GetValue(source, null), convertToType), null);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //skip the property and carry on
                            Debug.WriteLine(string.Format("GenericConvertor Failed to parse property {0}. Error was thrown.", foundproperty.Name), "Convertor");
                            Debug.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        //leave the property as its default value                        
                    }
                }
            }
        }

        /// <summary>
        /// Returns a Boolean value indicating whether an expression can be evaluated as a number.
        /// </summary>
        /// <param name="Expression">An <see cref="object"/> expression</param>
        /// <returns>a bool</returns>
        public static bool IsNumeric(object Expression)
        {
            bool isNum;
            double retNum;
            isNum = Double.TryParse(System.Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        private static void doMapperConfig()
        {
            if (isAutoMapperConfigured)
                return;
            Mapper.CreateMap<BusinessObjects.Question, Models.Question>();
            isAutoMapperConfigured = true;
        }
    }

    
    public static class AutoMapperExtensions
    {
        public static IMappingExpression<TSource, TDestination>
          IgnoreAllNonExisting<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);
            var existingMaps = Mapper.GetAllTypeMaps().First(x => x.SourceType.Equals(sourceType) && x.DestinationType.Equals(destinationType));
            foreach (var property in existingMaps.GetUnmappedPropertyNames())
            {
                expression.ForMember(property, opt => opt.Ignore());
            }
            return expression;
        }

        public static IMappingExpression
          IgnoreAllNonExisting(this IMappingExpression expression, Type sourceType, Type destinationType)
        {           
            var existingMaps = Mapper.GetAllTypeMaps().First(x => x.SourceType.Equals(sourceType) && x.DestinationType.Equals(destinationType));
            foreach (var property in existingMaps.GetUnmappedPropertyNames())
            {
                expression.ForMember(property, opt => opt.Ignore());
            }
            return expression;
        }
    }
}
