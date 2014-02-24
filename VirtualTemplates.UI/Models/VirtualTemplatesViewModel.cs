using System.Collections.Generic;
using VirtualTemplates.UI.DTO;

namespace VirtualTemplates.UI.Models
{
    public class VirtualTemplatesViewModel
    {
        public string ConfirmMessage;
        public string ErrorMessage;
        public string LastActionPath;
        public bool ShowAllTemplates;
        public IEnumerable<UITemplate> TemplateList; 
    }
}
