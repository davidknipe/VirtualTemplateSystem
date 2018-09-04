using System;
using EPiServer.Core;

namespace VirtualTemplates.Core.Models
{
    public class UiTemplateVersion
    {
        public ContentReference Reference { get; set; }
        public string ChangedBy { get; set; }
        public DateTime? ChangeDate { get; set; }
        public VersionStatus Status { get; set; }
        public string StatusText { get; set; }
        public string VirtualPath { get; set; }
    }
}