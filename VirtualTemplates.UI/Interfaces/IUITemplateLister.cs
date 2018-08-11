using System.Collections.Generic;
using VirtualTemplates.Core.Models;

namespace VirtualTemplates.UI.Interfaces
{
    public interface IUiTemplateLister
    {
        IEnumerable<UiTemplate> GetViewList(bool IncludePhysicalViews);
    }
}
