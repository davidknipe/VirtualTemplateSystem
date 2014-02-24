using System.Collections.Generic;
using VirtualTemplates.UI.DTO;

namespace VirtualTemplates.UI.Interfaces
{
    public interface IUITemplateLister
    {
        IEnumerable<UITemplate> GetViewList(bool IncludePhysicalViews);
    }
}
