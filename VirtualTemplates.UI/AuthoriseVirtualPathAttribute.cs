using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer.ServiceLocation;
using VirtualTemplates.Core.Interfaces;

namespace VirtualTemplates.UI
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AuthoriseVirtualPathAttribute : ActionFilterAttribute
    {
        private Injected<IPhysicalFileLister> _fileLister;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var allFiles = _fileLister.Service.ListPhysicalFiles(true);
            var virtualPath = filterContext.ActionParameters["VirtualPath"].ToString().ToLower();
            if (allFiles.Contains(virtualPath) == false)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
    }
}