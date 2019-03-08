using System.Collections.Generic;
using System.Web.Mvc;
using VirtualTemplates.Core.Models;

namespace VirtualTemplates.UI.Models
{
    public class VirtualTemplatesCompareModel
    {
        public string ConfirmMessage;

        public bool IsVirtual { get; set; }

        public string VirtualPath { get; set; }

        [AllowHtml]
        public string LeftContents { get; set; }

        [AllowHtml]
        public string RightContents { get; set; }

        public string Button { get; set; }

        public IList<UiTemplateVersion> Versions { get; set; }

        public string LeftVersionText { get; set; }

        public string RightVersionText { get; set; }
    }
}