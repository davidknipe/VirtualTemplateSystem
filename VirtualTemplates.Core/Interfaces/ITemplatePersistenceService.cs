using System.Collections.Generic;
using VirtualTemplates.Core.Impl;

namespace VirtualTemplates.Core.Interfaces
{
    public interface ITemplatePersistenceService
    {
        bool Exists(string virtualPath);
        VirtualTemplate GetViewFile(string virtualPath);
        bool SaveViewFile(string virtualPath, byte[] fileData);
        bool DeleteViewFile(string virtualPath);
        IEnumerable<string> ListAllRegisteredViews();
    }   
}
