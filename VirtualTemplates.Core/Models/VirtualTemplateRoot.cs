using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace VirtualTemplates.Core.Models
{
    [ContentType(
        DisplayName = "Virtual template root",
        Description = "The root where all virtual templates are saved",
        GUID = "2C25988A-AB55-49DB-A248-EDE870DB2D9D")]
    [AvailableContentTypes(
        Availability = Availability.Specific,
        IncludeOn = new[] { typeof(VirtualTemplateContent) })]
    public class VirtualTemplateRoot : ContentFolder { }
}