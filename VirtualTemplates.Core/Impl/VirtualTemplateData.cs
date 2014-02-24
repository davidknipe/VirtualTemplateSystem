using EPiServer.Data.Dynamic;

namespace VirtualTemplates.Core.Impl
{
    [EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
    public class VirtualTemplateData
    {
        [EPiServerDataIndex]
        public string VirtualPath { get; set; }

        public byte[] FileData { get; set; }
    }
}
