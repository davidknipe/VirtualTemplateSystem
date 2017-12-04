using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using System.Web.Mvc;
using System.Web.Routing;
using VirtualTemplates.UI;

namespace VirtualTemplates.Init
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class FeedRoutingInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var uiUrl = EPiServer.Configuration.Settings.Instance.UIUrl.OriginalString.TrimStart("~/".ToCharArray()).TrimEnd("/".ToCharArray());
             
            RouteTable.Routes.MapRoute(
                name: "VirtualTemplatesUI",
                url: VirtualTemplatesMenuProvider.RootUiUrl + "VirtualTemplates/{action}",
                defaults: new { controller = "VirtualTemplates", action = "Index" }
            );
        }

        public void Uninitialize(InitializationEngine context) { }

        public void Preload(string[] parameters) { }
    }
}