using EPiServer.Security;
using System;
using System.Web.Mvc;

namespace VirtualTemplates.UI
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthoriseUIAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!(PrincipalInfo.HasAdminAccess || PrincipalInfo.Current.HasPathAccess(VirtualTemplatesMenuProvider.RootUIUrl)))
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
    }
}
