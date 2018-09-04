using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using VirtualTemplates.Core.Interfaces;
using VirtualTemplates.Core.Models;

namespace VirtualTemplates.Core.Impl
{
    [ServiceConfiguration(typeof(IVirtualTemplateVersionRepository))]
    public class VirtualTemplateVersionRepository : IVirtualTemplateVersionRepository
    {
        private readonly IContentVersionRepository _contentVersionRepo;
        private readonly LocalizationService _localization;
        private readonly IContentRepository _contentRepo;
        private readonly ITemplateKeyConverter _keyConverter;

        public VirtualTemplateVersionRepository(IContentVersionRepository contentVersionRepo,
            LocalizationService localization, IContentRepository contentRepo, ITemplateKeyConverter keyConverter)
        {
            _contentVersionRepo = contentVersionRepo;
            _localization = localization;
            _contentRepo = contentRepo;
            _keyConverter = keyConverter;
        }

        public IList<UiTemplateVersion> GetAllVersions(ContentReference contentRef, string virtualPath)
        {
            return _contentVersionRepo.List(contentRef)
                .OrderByDescending(x => x.Saved)
                .Take(5)
                .Select(x => new UiTemplateVersion()
                {
                    VirtualPath = virtualPath,
                    Reference = x.ContentLink,
                    ChangedBy = x.SavedBy,
                    ChangeDate = x.Saved,
                    Status = x.Status,
                    StatusText = _localization.GetString("/versionstatus/" + x.Status.ToString(), x.Status.ToString())
                }).ToList();
        }

        public VirtualTemplate GetVersion(ContentReference contentReference)
        {
            var template = _contentRepo.Get<VirtualTemplateContent>(contentReference);
            if (template != null)
            {
                return new VirtualTemplate(_keyConverter.GetTemplateKey(template.VirtualPath),
                    template.TemplateContents)
                {
                    ChangedBy = template.ChangedBy,
                    ChangedDate = template.Saved,
                    StatusText =
                        _localization.GetString("/versionstatus/" + template.Status, template.Status.ToString())
                };
            }

            return null;
        }
    }
}