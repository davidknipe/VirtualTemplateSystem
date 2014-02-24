using System.Collections.Generic;
using System.Linq;
using VirtualTemplates.Core.Interfaces;
using VirtualTemplates.UI.DTO;
using VirtualTemplates.UI.Interfaces;

namespace VirtualTemplates.UI.Impl
{
    public class UITemplateLister : IUITemplateLister
    {
        private IPhysicalFileLister _physicalFileLister;
        private ITemplatePersistenceService _persistenceService;

        public UITemplateLister(IPhysicalFileLister physicalFileLister, ITemplatePersistenceService persistenceService) 
        {
            _persistenceService = persistenceService;
            _physicalFileLister = physicalFileLister;
        }

        public IEnumerable<UITemplate> GetViewList(bool IncludePhysicalViews)
        {
            if (IncludePhysicalViews)
            {
                //Get both listings and work out whats physical and whats not
                var physicalFiles = _physicalFileLister.ListPhysicalFiles();
                var virtualFiles = _persistenceService.ListAllRegisteredViews();

                var returnData = from p in physicalFiles
                                 join v in virtualFiles on p equals v into result
                                 from r in result.DefaultIfEmpty()
                                 select new UITemplate() { IsVirtual = (r != null), FilePath = p };

                return returnData.OrderBy(x => x.FilePath);

            }
            else
            {
                IList<UITemplate> returnList = new List<UITemplate>();
                _persistenceService.ListAllRegisteredViews().ToList().ForEach(x => returnList.Add(new UITemplate() { IsVirtual = true, FilePath = x }));
                return returnList.OrderBy(x => x.FilePath);
            }
        }
    }
}
