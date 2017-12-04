using EPiServer.Security;
using System;
using System.Web.Mvc;

namespace VirtualTemplates.UI
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthoriseUiAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!(PrincipalInfo.HasAdminAccess || PrincipalInfo.Current.HasPathAccess(VirtualTemplatesMenuProvider.RootUiUrl)))
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
    }
}
