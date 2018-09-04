using System.Collections.Generic;
using EPiServer.Core;
using VirtualTemplates.Core.Impl;
using VirtualTemplates.Core.Models;

namespace VirtualTemplates.Core.Interfaces
{
    public interface IVirtualTemplateVersionRepository
    {
        IList<UiTemplateVersion> GetAllVersions(ContentReference contentRef, string virtualPath);
        VirtualTemplate GetVersion(ContentReference contentReference);
    }
}