using System;
using System.Collections.Generic;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Framework.Cache;
using EPiServer.Framework.Localization;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using VirtualTemplates.Core.Init;
using VirtualTemplates.Core.Interfaces;
using VirtualTemplates.Core.Models;

namespace VirtualTemplates.Core.Impl
{
    [ServiceConfiguration(typeof(IVirtualTemplateRepository))]
    public class VirtualTemplateRepository : IVirtualTemplateRepository
    {
        private static readonly string _registeredViewsCacheKey = "__TemplateRepo_Templates";
        private readonly IVirtualTemplatesCache _virtualTemplatesCache;
        private readonly ISynchronizedObjectInstanceCache _cache;
        private readonly IContentRepository _contentRepo;
        private readonly IVirtualTemplateVersionRepository _templateVersionRepo;
        private readonly ITemplateKeyConverter _keyConverter;
        private readonly LocalizationService _localization;

        public VirtualTemplateRepository(IVirtualTemplatesCache virtualTemplatesCache,
            ISynchronizedObjectInstanceCache cache, IContentRepository contentRepo, 
            IVirtualTemplateVersionRepository templateVersionRepo, ITemplateKeyConverter keyConverter,
            LocalizationService localization)
        {
            _virtualTemplatesCache = virtualTemplatesCache;
            _cache = cache;
            _contentRepo = contentRepo;
            _templateVersionRepo = templateVersionRepo;
            _keyConverter = keyConverter;
            _localization = localization;
        }

        private Dictionary<string, ContentReference> RegisteredViews
        {
            get
            {
                try
                {
                    //Backed onto Episerver cache manager to ensure muliple instances share the same content
                    return _cache.ReadThrough(
                        _registeredViewsCacheKey,
                        () =>
                        {
                            var cacheVal =
                                new Dictionary<string, ContentReference>(StringComparer.InvariantCultureIgnoreCase);
                            var allTemplates =
                                _contentRepo.GetChildren<VirtualTemplateContent>(
                                    VirtualTemplateRootInit.VirtualTemplateRoot);
                            foreach (var template in allTemplates)
                            {
                                if (!cacheVal.ContainsKey(_keyConverter.GetTemplateKey(template.VirtualPath)))
                                {
                                    cacheVal.Add(_keyConverter.GetTemplateKey(template.VirtualPath), template.ContentLink);
                                }
                            }

                            return cacheVal;
                        },
                        ReadStrategy.Wait);
                }
                catch (Exception ex)
                {
                    // If application is trying to access a file during Application_Start(), for instance
                    // by calling BundleConfig.RegisterBundles() then this could throw an exception, as
                    // somewhere in the Episerver stack there is a depedency on System.Web.HttpContext.Request
                    // when using the content repo (seems that it's ProjectResolver in the internal namespace). 
                    // So being defensive here to prevent the application throwing an exception on startup
                    return new Dictionary<string, ContentReference>(StringComparer.InvariantCultureIgnoreCase);
                }
            }
        }

        public bool Exists(string virtualPath)
        {
            //Optimise for performance by using a Dictionary
            return RegisteredViews.ContainsKey(_keyConverter.GetTemplateKey(virtualPath));
        }

        public VirtualTemplate GetTemplate(string virtualPath)
        {
            if (!RegisteredViews.ContainsKey(_keyConverter.GetTemplateKey(virtualPath)))
                return null;

            var contentRef = RegisteredViews[_keyConverter.GetTemplateKey(virtualPath)];
            var template = _contentRepo.Get<VirtualTemplateContent>(contentRef);
            if (template != null)
            {
                return new VirtualTemplate(_keyConverter.GetTemplateKey(virtualPath), template.TemplateContents)
                {
                    ChangedBy = template.ChangedBy,
                    ChangedDate = template.Saved,
                    StatusText = _localization.GetString("/versionstatus/" + template.Status, template.Status.ToString())
                };
            }

            return null;
        }

        public bool SaveTemplate(string virtualPath, string fileContents)
        {
            VirtualTemplateContent template;
            if (RegisteredViews.ContainsKey(_keyConverter.GetTemplateKey(virtualPath)))
            {
                var contentRef = RegisteredViews[_keyConverter.GetTemplateKey(virtualPath)];
                template =
                    _contentRepo.Get<VirtualTemplateContent>(contentRef)
                        .CreateWritableClone() as VirtualTemplateContent;
            }
            else
            {
                template = _contentRepo.GetDefault<VirtualTemplateContent>(VirtualTemplateRootInit.VirtualTemplateRoot);
                template.Name = _keyConverter.GetTemplateKey(virtualPath);
            }

            if (template != null)
            {
                template.VirtualPath = _keyConverter.GetTemplateKey(virtualPath);
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
                var contentRef = RegisteredViews[_keyConverter.GetTemplateKey(virtualPath)];
                _contentRepo.MoveToWastebasket(contentRef);
                return true;
            }

            ResetState();
            return false;
        }

        public IList<UiTemplate> ListAllTemplates()
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
                        ChangedBy = templateContents.ChangedBy,
                        Versions = _templateVersionRepo.GetAllVersions(templateContents.ContentLink, templateContents.VirtualPath)
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

        public ContentReference GetContentReference(string virtualPath)
        {
            var key = _keyConverter.GetTemplateKey(virtualPath);
            if (RegisteredViews.ContainsKey(key))
            {
                return RegisteredViews[key];
            }

            return null;
        }
    }
}