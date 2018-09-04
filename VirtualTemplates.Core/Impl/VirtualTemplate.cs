using System;
using System.IO;
using System.Text;
using System.Web.Hosting;

namespace VirtualTemplates.Core.Impl
{
    public class VirtualTemplate : VirtualFile
    {
        private readonly byte[] _data;

        public string FileContents
        {
            get
            {
                using (var sr = new StreamReader(Open()))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        public VirtualTemplate(string virtualPath, string fileData)
            : base(virtualPath)
        {
            _data = Encoding.Default.GetBytes(fileData);
        } 

        public VirtualTemplate(string virtualPath, byte[] fileData)
            : base(virtualPath) => _data = fileData;

        public override Stream Open() => new MemoryStream(_data);

        public string ChangedBy { get; set; }

        public DateTime ChangedDate { get; set; }

        public string StatusText { get; set; }
    }

}
