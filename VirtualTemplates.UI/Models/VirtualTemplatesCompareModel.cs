using System.Web.Mvc;

namespace VirtualTemplates.UI.Models
{
    public class VirtualTemplatesCompareModel
    {
        public bool IsVirtual { get; set; }
        public string VirtualPath { get; set; }

        [AllowHtml]
        public string TemplateContents { get; set; }

        [AllowHtml]
        public string OriginalContents { get; set; }

    }
}