using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace NHSD.ElephantParade.Web.Helpers
{
    public static class HtmlMenuHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="linkText"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="selectList">Dictionary<string controller,string[] actions> if a controller is specified with no actions then all actions are deemed valid </param>
        /// <returns></returns>
        public static MvcHtmlString MenuItem(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, Dictionary<string, string[]> selectList)
        {
            string currentControllerName = (string)htmlHelper.ViewContext.RouteData.Values["controller"];
            string currentActionName = (string)htmlHelper.ViewContext.RouteData.Values["action"];
            string currentAreaName = (string)htmlHelper.ViewContext.RouteData.DataTokens["area"];
            string area = "";
            if (routeValues != null && routeValues.HasProperty("area"))
            {
                area = (string)routeValues.GetType().GetProperty("area").GetValue(routeValues, null);
            }

            var builder = new TagBuilder("li");
            string[] actions = null;
            if (selectList.ContainsKey(currentControllerName))
                actions = selectList[currentControllerName] ?? new string[] { };

            if ((currentControllerName.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase) || actions!=null)
                && (area == "" || currentAreaName.Equals(area, StringComparison.CurrentCultureIgnoreCase))
                && (currentActionName.Equals(actionName, StringComparison.CurrentCultureIgnoreCase) || (actions!=null && ( actions.Count()==0   || actions.Where(c => c.IndexOf(currentActionName) != -1).Count() > 0))))
                builder.AddCssClass("selected");
            builder.InnerHtml = htmlHelper.ActionLink(linkText, actionName, controllerName,routeValues,null).ToHtmlString();
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }
       /// <summary>
       /// Creates a <li></li> tag with a class of selected if the current route matches the specified controller/action
       /// </summary>
       /// <param name="htmlHelper"></param>
       /// <param name="linkText"></param>
       /// <param name="actionName"></param>
       /// <param name="controllerName"></param>
       /// <param name="altActions">a list of alternative actions that will 'select' this menu item</param>
       /// <returns></returns>
        public static MvcHtmlString MenuItem(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues,string[] altActions)
        {
            string currentControllerName = (string)htmlHelper.ViewContext.RouteData.Values["controller"];
            string currentActionName = (string)htmlHelper.ViewContext.RouteData.Values["action"];
            string currentAreaName = (string)htmlHelper.ViewContext.RouteData.DataTokens["area"];
            string area = "";
            if (routeValues != null && routeValues.HasProperty("area"))
            {
                area = (string)routeValues.GetType().GetProperty("area").GetValue(routeValues, null);
            }

            var builder = new TagBuilder("li");
            if (currentControllerName.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase)
                && (area == "" || currentAreaName.Equals(area, StringComparison.CurrentCultureIgnoreCase))
                && (currentActionName.Equals(actionName, StringComparison.CurrentCultureIgnoreCase) || altActions.Where(c => c.IndexOf(currentActionName) != -1).Count() > 0))
                builder.AddCssClass("selected");
            builder.InnerHtml = htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues,null).ToHtmlString();
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }      
        /// <summary>
        /// Creates a <li></li> tag with a class of selected if the current route matches the specified controller/action
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="linkText"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="selectAllActions">any action on the controller will select this menu item</param>
        /// <returns></returns>  
        public static MvcHtmlString MenuItem(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues,bool selectAllActions)
        {
            string currentControllerName = (string)htmlHelper.ViewContext.RouteData.Values["controller"];
            string currentActionName = (string)htmlHelper.ViewContext.RouteData.Values["action"];
            string currentAreaName = (string)htmlHelper.ViewContext.RouteData.DataTokens["area"];
            string area = "";
            if (routeValues != null && routeValues.HasProperty("area"))
            {
                area = (string)routeValues.GetType().GetProperty("area").GetValue(routeValues, null);
            }

            var builder = new TagBuilder("li");
            if (currentControllerName.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase)
                && (area=="" || currentAreaName.Equals(area,StringComparison.CurrentCultureIgnoreCase))
                && (selectAllActions || currentActionName.Equals(actionName, StringComparison.CurrentCultureIgnoreCase)))
                builder.AddCssClass("selected");
            builder.InnerHtml = htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues,null).ToHtmlString();
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }

       

        /// <summary>
        /// Creates a <li></li> tag with a class of selected if the current route matches the specified controller/action
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="linkText"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        public static MvcHtmlString MenuItem(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues)
        {
            return MenuItem(htmlHelper, linkText, actionName, controllerName,routeValues, false);
        }

        public static MvcHtmlString MenuItem(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName)
        {
            return MenuItem(htmlHelper, linkText, actionName, controllerName,null, false);
        }


        public static bool HasProperty(this object objectToCheck, string propertyName)
        {
            var type = objectToCheck.GetType();
            return type.GetProperty(propertyName) != null;
        } 
    }
}