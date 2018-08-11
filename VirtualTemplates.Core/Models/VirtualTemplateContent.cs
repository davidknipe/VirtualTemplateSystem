using System.ComponentModel.DataAnnotations;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Web;

namespace VirtualTemplates.Core.Models
{
    [ContentType(
        DisplayName = "Virtual template",
        Description = "Saved the definition of the template into the content repository",
        GUID = "7D4648CD-043D-4C57-815C-418CD885A5CF")]
    [AvailableContentTypes(Availability = Availability.None,
        IncludeOn = new[] {typeof(VirtualTemplateRoot)})]
    public class VirtualTemplateContent : VirtualTemplateContentBase
    {
        [Display(
            Order = 1, 
            GroupName = TabNames.VirtualTemplates)]
        [Required]
        public virtual string VirtualPath { get; set; }

        [Display(
            Order = 2, 
            GroupName = TabNames.VirtualTemplates)]
        [Required]
        [UIHint(UIHint.Textarea)]
        public virtual string TemplateContents { get; set; }
    }
}