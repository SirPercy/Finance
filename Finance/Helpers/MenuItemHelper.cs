using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Finance.Helpers
{
    public static class MenuItemHelper
    {
        public static MvcHtmlString MenuItem(this HtmlHelper helper, string linkText, string actionName, string controllerName)
        {
            var currentControllerName = helper.ViewContext.RouteData.GetRequiredString("Controller");
            //var currentActionName = helper.ViewContext.RouteData.GetRequiredString("Action");

            var sb = new StringBuilder();

            //Om både controller och action ska vara rätt för att meny ska markeras - Tas bort då endast controller räcker
            //if (currentControllerName.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase) && currentActionName.Equals(actionName, StringComparison.CurrentCultureIgnoreCase))

            if (currentControllerName.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase))
                sb.Append("<li class=\"active\">");
            else
                sb.Append("<li>");

            sb.Append(helper.ActionLink(linkText, actionName, controllerName));
            sb.Append("</li>");
            return MvcHtmlString.Create(sb.ToString());
        }

    }
}