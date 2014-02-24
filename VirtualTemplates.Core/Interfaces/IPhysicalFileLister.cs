using System.Collections.Generic;
namespace VirtualTemplates.Core.Interfaces
{
    public interface IPhysicalFileLister
    {
        IEnumerable<string> ListPhysicalFiles();
        IEnumerable<string> ListPhysicalFiles(string path, string searchPattern);
        IEnumerable<string> ListPhysicalFiles(string path, IList<string> searchPattern);
    }
}
