using System.Collections.Generic;
using System.Linq;
using EPiServer.ServiceLocation;
using VirtualTemplates.Core.Interfaces;
using VirtualTemplates.Core.Models;
using VirtualTemplates.UI.Interfaces;

namespace VirtualTemplates.UI.Impl
{
    [ServiceConfiguration(typeof(IUiTemplateLister))]
    public class UiTemplateLister : IUiTemplateLister
    {
        private readonly IPhysicalFileLister _physicalFileLister;
        private readonly IVirtualTemplateRepository _persistenceService;

        public UiTemplateLister(IPhysicalFileLister physicalFileLister, IVirtualTemplateRepository persistenceService)
        {
            _persistenceService = persistenceService;
            _physicalFileLister = physicalFileLister;
        }

        public IEnumerable<UiTemplate> GetViewList(bool includePhysicalViews)
        {
            if (includePhysicalViews)
            {
                //Get both listings and work out whats physical and whats not
                var physicalFiles = _physicalFileLister.ListPhysicalFiles();
                var virtualFiles = _persistenceService.ListAllTemplates();

                var returnData = physicalFiles
                    .GroupJoin(virtualFiles, p => p, v => v.FilePath, (p, result) => new {p, result})
                    .SelectMany(@t => @t.result.DefaultIfEmpty(),
                        (@t, r) => new UiTemplate()
                        {
                            IsVirtual = (r != null),
                            FilePath = @t.p,
                            ChangedBy = r?.ChangedBy,
                            Versions = r?.Versions,
                            TemplateIsChanged = r?.TemplateIsChanged != null && (r?.TemplateIsChanged).Value
                        });

                return returnData.OrderBy(x => x.FilePath);

            }
            else
            {
                return _persistenceService.ListAllTemplates().OrderBy(x => x.FilePath);
            }
        }
    }
}
