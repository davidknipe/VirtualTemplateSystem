using System.Collections.Generic;
using VirtualTemplates.Core.Models;

namespace VirtualTemplates.UI.Models
{
    public class VirtualTemplatesListViewModel
    {
        public string ConfirmMessage;
        public string ErrorMessage;
        public string LastActionPath;
        public bool ShowAllTemplates;
        public IEnumerable<UiTemplate> TemplateList;
        public string LastSearch;
    }
}
