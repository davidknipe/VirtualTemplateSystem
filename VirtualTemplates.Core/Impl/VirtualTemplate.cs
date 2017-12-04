using System.IO;
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
                using (var sr = new StreamReader(this.Open()))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public VirtualTemplate(string virtualPath, byte[] FileData)
            : base(virtualPath)
        {
            this._data = FileData;
        }

        public override System.IO.Stream Open()
        {
            return new MemoryStream(_data);
        }
    }

}
