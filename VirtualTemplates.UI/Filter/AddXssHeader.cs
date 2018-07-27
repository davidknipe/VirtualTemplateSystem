using System.Web.Mvc;

namespace VirtualTemplates.UI.Filter
{
    public class AddXssHeader : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            filterContext.HttpContext.Response.Headers.Add("X-XSS-Protection", "0");
        }
    }
}
