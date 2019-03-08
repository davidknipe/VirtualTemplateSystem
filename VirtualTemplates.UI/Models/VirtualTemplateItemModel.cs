using System.Web.Mvc;

namespace VirtualTemplates.UI.Models
{
    public class VirtualTemplateItemModel
    {
        public string ConfirmMessage;

        public bool IsVirtual { get; set; }
        public string VirtualPath { get; set; }

        [AllowHtml]
        public string TemplateContents { get; set; }

        public string Button { get; set; }
    }
}
