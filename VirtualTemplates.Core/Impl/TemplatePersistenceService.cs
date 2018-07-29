using EPiServer.Data.Dynamic;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Framework.Cache;
using VirtualTemplates.Core.Interfaces;

namespace VirtualTemplates.Core.Impl
{
    public class TemplatePersistenceService : ITemplatePersistenceService
    {
        private static readonly string _registeredViewsCacheKey = "__TemplatePersistenceService_RegisteredViews";
        private readonly IVirtualTemplatesCache _virtualTemplatesCache;
        private readonly ISynchronizedObjectInstanceCache _cache;


        public TemplatePersistenceService(IVirtualTemplatesCache virtualTemplatesCache, ISynchronizedObjectInstanceCache cache)
        {
            _virtualTemplatesCache = virtualTemplatesCache;
            _cache = cache;
        }

        private HashSet<string> RegisteredViews
        {
            get
            {
                //Backed onto Episerver cache manager to ensure muliple instances share the same HashSet
                return _cache.ReadThrough(
                    _registeredViewsCacheKey,
                    () =>
                    {
                        var cacheVal = new HashSet<string>();
                        ListAllRegisteredViews().ToList().ForEach(x => cacheVal.Add(x.ToLower()));
                        return cacheVal;
                    },
                    ReadStrategy.Wait);
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

            return null;
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
                    ResetState();
                    return true;
                }
                return false;
            }
        }

        public IEnumerable<string> ListAllRegisteredViews()
        {
            var viewList = new List<string>();
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
                _virtualTemplatesCache.Reset();
                _cache.Remove(_registeredViewsCacheKey);
            }
        }
    }
}
