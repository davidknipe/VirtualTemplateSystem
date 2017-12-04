using EPiServer.Data.Dynamic;
using System.Collections.Generic;
using System.Linq;
using VirtualTemplates.Core.Interfaces;

namespace VirtualTemplates.Core.Impl
{
    public class TemplatePersistenceService : ITemplatePersistenceService
    {
        private static readonly string _registeredViewsCacheKey = "__TemplatePersistenceService_RegisteredViews";

        private HashSet<string> RegisteredViews
        {
            get
            {
                //Backed onto EPiServer cache manager to ensure muliple instances share the same HashSet
                lock (this)
                {
                    if (EPiServer.CacheManager.Get(_registeredViewsCacheKey) == null)
                    {
                        lock (this)
                        {
                            var cacheVal = new HashSet<string>();
                            this.ListAllRegisteredViews().ToList().ForEach(x => cacheVal.Add(x.ToLower()));
                            EPiServer.CacheManager.Insert(_registeredViewsCacheKey, cacheVal);
                            return cacheVal;
                        }
                    }
                }
                lock (this)
                {
                    return (HashSet<string>)EPiServer.CacheManager.Get(_registeredViewsCacheKey);
                }
            }
            set
            {
                EPiServer.CacheManager.Insert(_registeredViewsCacheKey, value);
            }
        }

        public bool Exists(string virtualPath)
        {
            //Optimise for performance by using a HashSet
            return RegisteredViews.Contains(virtualPath.ToLower());
        }

        public VirtualTemplate GetViewFile(string virtualPath)
        {
            VirtualTemplateData data = GetFileData(virtualPath);
            if (data != null)
            {
                return new VirtualTemplate(virtualPath, data.FileData);
            }
            else
            {
                return null;
            }
        }

        public bool SaveViewFile(string virtualPath, byte[] fileData)
        {
            var result = SaveFileData(new VirtualTemplateData() { VirtualPath = virtualPath, FileData = fileData });
            if (result)
            {
                ResetState();
            }
            return result;
        }

        public bool DeleteViewFile(string virtualPath)
        {
            using (var fileDataStore = typeof(VirtualTemplateData).GetStore())
            {
                // Look up the content in the DDS
                var result = fileDataStore.Find<VirtualTemplateData>("VirtualPath", virtualPath).FirstOrDefault();
                if (result != null)
                {
                    fileDataStore.Delete(result);
                    this.ResetState();
                    return true;
                }
                return false;
            }
        }

        public IEnumerable<string> ListAllRegisteredViews()
        {
            IList<string> viewList = new List<string>();
            using (var fileDataStore = typeof(VirtualTemplateData).GetStore())
            {
                // Look up the content in the DDS
                foreach(var view in fileDataStore.LoadAll<VirtualTemplateData>())
                {
                    viewList.Add(view.VirtualPath);
                }
            }
            return viewList;
        }

        private VirtualTemplateData GetFileData(string virtualPath)
        {
            using (var fileDataStore = typeof(VirtualTemplateData).GetStore())
            {
                // Look up the content in the DDS
                var result = fileDataStore.Find<VirtualTemplateData>("VirtualPath", virtualPath).FirstOrDefault();
                return result;
            }
        }

        private bool SaveFileData(VirtualTemplateData data)
        {
            using (var fileDataStore = typeof(VirtualTemplateData).GetStore())
            {
                // Look up the content in the DDS
                var result = fileDataStore.Find<VirtualTemplateData>("VirtualPath", data.VirtualPath).FirstOrDefault();
                if (result == null)
                {
                    fileDataStore.Save(data);
                }
                else
                {
                    result.FileData = data.FileData;
                    fileDataStore.Save(result);
                }
                return true;
            }
        }

        private void ResetState()
        {
            lock (this)
            {
                VirtualTemplatesCache.Reset();
                EPiServer.CacheManager.Remove(_registeredViewsCacheKey);
            }
        }
    }
}
