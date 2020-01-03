using System.Linq;
using System.Web;
using EPiServer.ServiceLocation;
using VirtualTemplates.Core.Interfaces;

namespace VirtualTemplates.Core.Impl
{
    [ServiceConfiguration(typeof(ITemplateComparer))]
    public class TemplateComparer : ITemplateComparer
    {
        private readonly IPhysicalFileReader _fileReader;
        private readonly HttpContextBase _httpContext;

        public TemplateComparer(IPhysicalFileReader fileReader, HttpContextBase httpContext)
        {
            _fileReader = fileReader;
            _httpContext = httpContext;
        }

        public bool TemplateIsChanged(string currentFileContents, string originalPath)
        {
            try
            {
                var currentContent = TrimWhiteSpace(currentFileContents);
                var originalContent =
                    TrimWhiteSpace(_fileReader.ReadFile(_httpContext.Server.MapPath("~" + originalPath)));
                return currentContent != originalContent;
            }
            catch 
            {
                return true;
            }
        }

        private string TrimWhiteSpace(string original)
        {
            return new string
            (original
                .Where
                (
                    c => !char.IsWhiteSpace(c)
                )
                .ToArray<char>()
            );
        }
    }
}