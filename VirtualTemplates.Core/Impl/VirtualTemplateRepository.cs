using System.Collections.Generic;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Framework.Cache;
using EPiServer.Security;
using VirtualTemplates.Core.Init;
using VirtualTemplates.Core.Interfaces;
using VirtualTemplates.Core.Models;

namespace VirtualTemplates.Core.Impl
{
    public class VirtualTemplateRepository : IVirtualTemplateRepository
    {
        private static readonly string _registeredViewsCacheKey = "__TemplateRepo_Templates";
        private readonly IVirtualTemplatesCache _virtualTemplatesCache;
        private readonly ISynchronizedObjectInstanceCache _cache;
        private readonly IContentRepository _contentRepo;

        public VirtualTemplateRepository(IVirtualTemplatesCache virtualTemplatesCache,
            ISynchronizedObjectInstanceCache cache, IContentRepository contentRepo)
        {
            _virtualTemplatesCache = virtualTemplatesCache;
            _cache = cache;
            _contentRepo = contentRepo;
        }

        private Dictionary<string, ContentReference> RegisteredViews
        {
            get
            {
                //Backed onto Episerver cache manager to ensure muliple instances share the same content
                return _cache.ReadThrough(
                    _registeredViewsCacheKey,
                    () =>
                    {
                        var cacheVal = new Dictionary<string, ContentReference>();
                        var allTemplates =
                            _contentRepo.GetChildren<VirtualTemplateContent>(
                                VirtualTemplateRootInit.VirtualTemplateRoot);
                        foreach (var template in allTemplates)
                        {
                            if (!cacheVal.ContainsKey(template.VirtualPath))
                            {
                                cacheVal.Add(template.VirtualPath, template.ContentLink);
                            }
                        }
                        return cacheVal;
                    },
                    ReadStrategy.Wait);
            }
        }

        public bool Exists(string virtualPath)
        {
            //Optimise for performance by using a Dictionary
            return RegisteredViews.ContainsKey(virtualPath);
        }

        public VirtualTemplate GetTemplate(string virtualPath)
        {
            if (!RegisteredViews.ContainsKey(virtualPath))
                return null;

            var contentRef = RegisteredViews[virtualPath];
            var template = _contentRepo.Get<VirtualTemplateContent>(contentRef);
            if (template != null)
            {
                return new VirtualTemplate(virtualPath, template.TemplateContents);
            }

            return null;
        }

        public bool SaveTemplate(string virtualPath, string fileContents)
        {
            VirtualTemplateContent template;
            if (RegisteredViews.ContainsKey(virtualPath))
            {
                var contentRef = RegisteredViews[virtualPath];
                template =
                    _contentRepo.Get<VirtualTemplateContent>(contentRef)
                        .CreateWritableClone() as VirtualTemplateContent;
            }
            else
            {
                template = _contentRepo.GetDefault<VirtualTemplateContent>(VirtualTemplateRootInit.VirtualTemplateRoot);
                template.Name = virtualPath;
            }

            if (template != null)
            {
                template.VirtualPath = virtualPath;
                template.TemplateContents = fileContents;
                _contentRepo.Save(template, SaveAction.ForceNewVersion | SaveAction.Publish, AccessLevel.NoAccess);
                return true;
            }

            return false;
        }

        public bool RevertTemplate(string virtualPath)
        {
            if (RegisteredViews.ContainsKey(virtualPath))
            {
                var contentRef = RegisteredViews[virtualPath];
                _contentRepo.MoveToWastebasket(contentRef);
                return true;
            }

            ResetState();
            return false;
        }

        public IEnumerable<UiTemplate> ListAllTemplates()
        {
            List<UiTemplate> allTemplates = new List<UiTemplate>();
            foreach (var template in RegisteredViews)
            {
                if (_contentRepo.Get<VirtualTemplateContent>(template.Value) is VirtualTemplateContent templateContents)
                {
                    allTemplates.Add(new UiTemplate()
                    {
                        IsVirtual = true,
                        FilePath = templateContents.VirtualPath,
                        ChangedBy = templateContents.ChangedBy
                    });
                }
            }

            return allTemplates;
        }

        public void ResetState()
        {
            lock (this)
            {
                _virtualTemplatesCache.Reset();
                _cache.Remove(_registeredViewsCacheKey);
            }
        }
    }
}
