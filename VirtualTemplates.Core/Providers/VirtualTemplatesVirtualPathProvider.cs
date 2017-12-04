using EPiServer.ServiceLocation;
using System.Collections.Specialized;
using System.Web.Hosting;
using VirtualTemplates.Core.Impl;
using VirtualTemplates.Core.Interfaces;

namespace VirtualTemplates.Core.Providers
{
    public class VirtualTemplatesVirtualPathProvider : VirtualPathProvider
    {
        private readonly ITemplatePersistenceService _viewPersistenceService;

        public VirtualTemplatesVirtualPathProvider()
        {
            _viewPersistenceService = ServiceLocator.Current.GetInstance<ITemplatePersistenceService>();
        }

        public VirtualTemplatesVirtualPathProvider(ITemplatePersistenceService viewPersistenceService)
        {
            _viewPersistenceService = viewPersistenceService;
        }

        public VirtualTemplatesVirtualPathProvider(string providerName, NameValueCollection configParameters)
        {
            _viewPersistenceService = ServiceLocator.Current.GetInstance<ITemplatePersistenceService>();
        }

        public override bool FileExists(string virtualPath) 
        {
            if (_viewPersistenceService.Exists(virtualPath))  
            {
                return true;
            }
            else
            {
                return base.FileExists(virtualPath);
            }
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            if (_viewPersistenceService.Exists(virtualPath))
            {
                VirtualFile vf = _viewPersistenceService.GetViewFile(virtualPath);
                return vf;
            }

            return base.GetFile(virtualPath);
        }

        public override string GetCacheKey(string virtualPath)
        {
            //Need to use a custom cache key implementation to avoid clashes with cachekeys generated 
            //for existing files on disk that are in the same physical location as a virtual template
            if (_viewPersistenceService.Exists(virtualPath))
            {
                return VirtualTemplatesCache.VersionKey + virtualPath.Replace(@"\", "_").Replace(@"/", "_");
            }

            return base.GetCacheKey(virtualPath);
        }

        public override string GetFileHash(string virtualPath, System.Collections.IEnumerable virtualPathDependencies)
        {
            // Ensure the hash contains the version key as the view in the Repo may have been changed when the app is offline
            // this forces the runtime to recompile each view stored in the Repo each time the application starts and/or the 
            // the version key is updated
            if (_viewPersistenceService.Exists(virtualPath))
            {
                return virtualPath + VirtualTemplatesCache.VersionKey;
            }

            return base.GetFileHash(virtualPath, virtualPathDependencies);
        }
    }
}
