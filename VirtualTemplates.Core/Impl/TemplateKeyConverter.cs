using EPiServer.ServiceLocation;
using VirtualTemplates.Core.Interfaces;

namespace VirtualTemplates.Core.Impl
{
    /// <inheritdoc />
    [ServiceConfiguration(typeof(ITemplateKeyConverter))]
    public class TemplateKeyConverter : ITemplateKeyConverter
    {
        /// <inheritdoc />
        public string GetTemplateKey(string virtualPath) => virtualPath.TrimStart('~');
    }
}