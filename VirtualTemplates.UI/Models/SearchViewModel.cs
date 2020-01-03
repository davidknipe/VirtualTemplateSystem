using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace VirtualTemplates.UI.Models
{
    public class SearchViewModel
    {
        [Required]
        [AllowHtml]
        public string searchString { get; set; }
        public bool searchFileNamesOnly { get; set; }
    }
}