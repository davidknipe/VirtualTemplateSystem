using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.ServiceLocation;
using VirtualTemplates.Core.Interfaces;

namespace VirtualTemplates.Core.Impl
{
    /// <inheritdoc />
    [ServiceConfiguration(typeof(IFileSearcher))]
    public class FileSearcher : IFileSearcher
    {
        private readonly IVirtualTemplateRepository _templateRepo;
        private readonly IPhysicalFileReader _fileReader;
        private readonly IPhysicalFileLister _fileLister;
        private readonly HttpContextBase _httpContext;

        public FileSearcher(IVirtualTemplateRepository templateRepo, IPhysicalFileReader fileReader,
            IPhysicalFileLister fileLister, HttpContextBase httpContext)
        {
            _templateRepo = templateRepo;
            _fileReader = fileReader;
            _fileLister = fileLister;
            _httpContext = httpContext;
        }

        /// <inheritdoc />
        public List<string> SearchFiles(string searchString, bool searchInFileNamesOnly)
        {
            var results = new List<string>();
            var allFiles = _fileLister.ListPhysicalFiles();
            searchString = searchString.ToLower();

            foreach (var file in allFiles)
            {

                if (searchInFileNamesOnly)
                {
                    if (file.ToLower().Contains(searchString))
                        results.Add(file);
                }
                else
                {
                    if (file.ToLower().Contains(searchString))
                    {
                        results.Add(file);
                    }
                    else
                    {
                        var isVirtualFile = _templateRepo.Exists(file);
                        var fileContents = isVirtualFile
                            ? _templateRepo.GetTemplate(file).FileContents.ToLower()
                            : _fileReader.ReadFile(_httpContext.Server.MapPath("~" + file)).ToLower();

                        if (fileContents.Contains(searchString))
                            results.Add(file);
                    }
                }
            }

            results.Sort();
            return results;
        }
    }
}