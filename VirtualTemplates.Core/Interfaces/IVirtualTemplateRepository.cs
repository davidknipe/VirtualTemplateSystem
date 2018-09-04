using System.Collections.Generic;
using EPiServer.Core;
using VirtualTemplates.Core.Impl;
using VirtualTemplates.Core.Models;

namespace VirtualTemplates.Core.Interfaces
{
    public interface IVirtualTemplateRepository
    {
        bool Exists(string virtualPath);
        VirtualTemplate GetTemplate(string virtualPath);
        bool SaveTemplate(string virtualPath, string fileContents);
        bool RevertTemplate(string virtualPath);
        IList<UiTemplate> ListAllTemplates();
        void ResetState();
        ContentReference GetContentReference(string virtualPath);
    }
}
