using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Reflection;
using System.EnterpriseServices;
using System.Web.Mvc.Html;
using System.ComponentModel;

namespace NHSD.ElephantParade.Web.Helpers
{
    public static class HtmlEnumExtensions
    {

        public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper htmlHelper, string name, TEnum selectedValue,object htmlAttribuites)
        {
            IEnumerable<TEnum> values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
            IEnumerable<SelectListItem> items = null;
            if (typeof(TEnum).IsEnum)
            {
                items =
                from value in values
                select new SelectListItem
                {
                    Text = DescriptionAttribute(value),
                    Value = value.ToString(),
                    Selected = (value.Equals(selectedValue))
                };

            }
            else
            {
                items =
                from value in values
                select new SelectListItem
                {
                    Text = value.ToString(),
                    Value = value.ToString(),
                    Selected = (value.Equals(selectedValue))
                };

            }
            return htmlHelper.DropDownList(
                name,
                items,
                htmlAttribuites 
                );
        }


        /// <summary>
        /// Can use the TypeConverter attribute on enum to give spacing to enums
        /// [TypeConverter(typeof(PascalCaseWordSplittingEnumConverter))]
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression)
        {
            return EnumDropDownListFor(htmlHelper, expression, new { });
        }
        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression,object htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            Type enumType = GetNonNullableModelType(metadata);
            IEnumerable<TEnum> values = Enum.GetValues(enumType).Cast<TEnum>();

            TypeConverter converter = TypeDescriptor.GetConverter(enumType);
            IEnumerable<SelectListItem> items = null;
            if (typeof(TEnum).IsEnum)
            {
                items =
                from value in values
                select new SelectListItem
                {
                    Text = DescriptionAttribute(value),
                    Value = value.ToString(),
                    Selected = value.Equals(metadata.Model)
                };
            }
            else
            {
                items =
                from value in values
                select new SelectListItem
                {
                    Text = value.ToString(),
                    Value = value.ToString(),
                    Selected = value.Equals(metadata.Model)
                };
            }

            if (metadata.IsNullableValueType)
            {
                items = SingleEmptyItem.Concat(items);
            }

            return htmlHelper.DropDownListFor(
                expression,
                items,
                htmlAttributes
                );
        }

        private static Type GetNonNullableModelType(ModelMetadata modelMetadata)
        {
            Type realModelType = modelMetadata.ModelType;

            Type underlyingType = Nullable.GetUnderlyingType(realModelType);
            if (underlyingType != null)
            {
                realModelType = underlyingType;
            }
            return realModelType;
        }

        private static readonly SelectListItem[] SingleEmptyItem = new[] { new SelectListItem { Text = "", Value = "" } };
         

        public static string DescriptionAttribute(this Enum value)
        {
            //Using reflection to get the field info
            FieldInfo info = value.GetType().GetField(value.ToString());
            
            //Get the Displayname Attributes
            System.ComponentModel.DescriptionAttribute[] attributes = (System.ComponentModel.DescriptionAttribute[])info.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);

            //Only capture the description attribute if it is a concrete result (i.e. 1 entry)
            if (attributes.Length == 1)
            {
                return attributes[0].Description;
            }
            else //Use the value for display if not concrete result
            {
                return value.ToString();
            }
        }

        private static string DescriptionAttribute(object value)
        {
            //Using reflection to get the field info
            FieldInfo info = value.GetType().GetField(value.ToString());

            //Get the Displayname Attributes
            System.ComponentModel.DescriptionAttribute[] attributes = (System.ComponentModel.DescriptionAttribute[])info.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);

            //Only capture the description attribute if it is a concrete result (i.e. 1 entry)
            if (attributes.Length == 1)
            {
                return attributes[0].Description;
            }
            else //Use the value for display if not concrete result
            {
                return value.ToString();
            }
        }
    }
}