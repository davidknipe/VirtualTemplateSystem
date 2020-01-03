using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using EPiServer.ServiceLocation;
using VirtualTemplates.Core.Interfaces;

namespace VirtualTemplates.Core.Impl
{
    [ServiceConfiguration(typeof(IPhysicalFileLister))]
    public class PhysicalFileLister : IPhysicalFileLister
    {
        /// <inheritdoc />
        public IEnumerable<string> ListPhysicalFiles() 
            => ListPhysicalFilesInternal(HostingEnvironment.ApplicationPhysicalPath,
                new List<string>() {"*.cshtml", "*.css", "*.js"}, false);

        public IEnumerable<string> ListPhysicalFiles(bool allLowerCase)
            => ListPhysicalFilesInternal(HostingEnvironment.ApplicationPhysicalPath,
                new List<string>() {"*.cshtml", "*.css", "*.js"}, allLowerCase);

        /// <inheritdoc />
        public IEnumerable<string> ListPhysicalFiles(string path, IList<string> searchPattern)
        {
            return ListPhysicalFilesInternal(path, searchPattern, false);
        }

        public IEnumerable<string> ListPhysicalFiles(string path, string searchPattern) 
            => ListPhysicalFilesInternal(path, new List<string> { searchPattern }, false);

        private IEnumerable<string> ListPhysicalFilesInternal(string path, IList<string> searchPattern, bool allLowerCase)
        {
            var results = new List<string>();

            foreach (var s in searchPattern)
            {
                foreach (var f in Directory.GetFiles(path, s, SearchOption.AllDirectories))
                {
                    if (allLowerCase)
                    {
                        results.Add(f.Remove(0, path.Length - 1).Replace(@"\", @"/").ToLower());
                    }
                    else
                    {
                        results.Add(f.Remove(0, path.Length - 1).Replace(@"\", @"/"));
                    }
                }
            }
            return results.OrderBy(x => x);
        }

    }
}
