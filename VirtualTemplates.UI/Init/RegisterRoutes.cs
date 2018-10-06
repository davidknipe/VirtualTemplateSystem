using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace VirtualTemplates.UI.Init
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class RegisterRoutes : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
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