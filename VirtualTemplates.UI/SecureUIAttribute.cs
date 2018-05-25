using System;
using System.Web.Mvc;
using EPiServer.Security;

namespace VirtualTemplates.UI
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
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
