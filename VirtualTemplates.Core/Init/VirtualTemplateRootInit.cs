using System.Linq;
using System.Reflection;
using EPiServer;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging.Compatibility;
using EPiServer.Security;
using VirtualTemplates.Core.Models;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace VirtualTemplates.Core.Init
{
    [ModuleDependency(typeof(DataInitialization))]
    [InitializableModule]
    [ModuleDependency(typeof(InitializationModule))]
    public class VirtualTemplateRootInit : IInitializableModule
    {
        public static ContentReference VirtualTemplateRoot;
        private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Initialize(InitializationEngine context)
        {
            var contentRepository = context.Locate.ContentRepository();

            var rootContent = contentRepository.GetChildren<VirtualTemplateRoot>(ContentReference.RootPage);
            if (rootContent == null || !rootContent.Any())
            {
                var templatesRoot = contentRepository.GetDefault<VirtualTemplateRoot>(ContentReference.RootPage);
                templatesRoot.Name = "Virtual templates root";
                VirtualTemplateRoot = contentRepository.Publish(templatesRoot, AccessLevel.NoAccess);

                //TODO: Set default permissions?
            }
            else
            {
                VirtualTemplateRoot = rootContent.First().ContentLink;
            }
        }

        public void Uninitialize(InitializationEngine context) { }
    }
}