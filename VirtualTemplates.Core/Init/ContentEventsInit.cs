using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using VirtualTemplates.Core.Interfaces;
using VirtualTemplates.Core.Models;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace VirtualTemplates.Core.Init
{
    [InitializableModule]
    [ModuleDependency(typeof(InitializationModule))]
    public class ContentEventsInit : IInitializableModule
    {
        private IContentLoader _contentLoader;
        private IVirtualTemplateRepository _templateRepository;
        private IContentEvents _contentEvents;

        public void Initialize(InitializationEngine context)
        {
            _contentLoader = context.Locate.Advanced.GetInstance<IContentLoader>();
            _templateRepository = context.Locate.Advanced.GetInstance<IVirtualTemplateRepository>();
            _contentEvents = context.Locate.Advanced.GetInstance<IContentEvents>();
            _contentEvents.DeletingContent += ContentEvents_DeletingContent;
            _contentEvents.PublishedContent += ContentEvents_PublishedContent;
            _contentEvents.MovedContent += ContentEvents_MovedContent;
        }

        private void ContentEvents_MovedContent(object sender, ContentEventArgs e)
        {
            if (e.Content.GetOriginalType() == typeof(VirtualTemplateContent))
            {
                _templateRepository.ResetState();
            }
        }

        private void ContentEvents_PublishedContent(object sender, ContentEventArgs e)
        {
            if (e.Content.GetOriginalType() == typeof(VirtualTemplateContent))
            {
                _templateRepository.ResetState();
            }
        }

        private void ContentEvents_DeletingContent(object sender, EPiServer.DeleteContentEventArgs e)
        {
            var content = _contentLoader.Get<VirtualTemplateContent>(e.ContentLink);
            if (content != null)
            {
                _templateRepository.ResetState();
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
            _contentEvents.DeletingContent -= ContentEvents_DeletingContent;
            _contentEvents.PublishedContent -= ContentEvents_PublishedContent;
            _contentEvents.MovedContent -= ContentEvents_MovedContent;
        }
    }
}
