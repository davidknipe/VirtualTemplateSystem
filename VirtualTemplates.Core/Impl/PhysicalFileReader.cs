using System.IO;
using EPiServer.ServiceLocation;
using VirtualTemplates.Core.Interfaces;

namespace VirtualTemplates.Core.Impl
{
    [ServiceConfiguration(typeof(IPhysicalFileReader))]
    public class PhysicalFileReader : IPhysicalFileReader
    {
        public string ReadFile(string path)
        {
            return File.ReadAllText(path);
        }
    }
}