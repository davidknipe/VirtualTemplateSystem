using EPiServer.ServiceLocation;
using System.Collections.Specialized;
using System.Web.Hosting;
using VirtualTemplates.Core.Impl;
using VirtualTemplates.Core.Interfaces;

namespace VirtualTemplates.Core.Providers
{
    public class VirtualTemplatesVirtualPathProvider : VirtualPathProvider
    {
#pragma warning disable 649
        private readonly Injected<ITemplatePersistenceService> _viewPersistenceService;
#pragma warning restore 649
#pragma warning disable 169
        private readonly Injected<IVirtualTemplatesCache> _virtualTemplatesCache;
#pragma warning restore 169


        /// <inheritdoc />
        public VirtualTemplatesVirtualPathProvider() { }

        /// <inheritdoc />
        public VirtualTemplatesVirtualPathProvider(ITemplatePersistenceService viewPersistenceService) { }

        /// <inheritdoc />
        public VirtualTemplatesVirtualPathProvider(string providerName, NameValueCollection configParameters) { }

        /// <inheritdoc />
        public override bool FileExists(string virtualPath) 
        {
            if (_viewPersistenceService.Service.Exists(virtualPath))  
            {
                return true;
            }
            else
            {
                return base.FileExists(virtualPath);
            }
        }

        /// <inheritdoc />
        public override VirtualFile GetFile(string virtualPath)
        {
            if (_viewPersistenceService.Service.Exists(virtualPath))
            {
                VirtualFile vf = _viewPersistenceService.Service.GetViewFile(virtualPath);
                return vf;
            }

            return base.GetFile(virtualPath);
        }

        /// <inheritdoc />
        public override string GetCacheKey(string virtualPath)
        {
            //Need to use a custom cache key implementation to avoid clashes with cachekeys generated 
            //for existing files on disk that are in the same physical location as a virtual template
            if (_viewPersistenceService.Service.Exists(virtualPath))
            {
                return _virtualTemplatesCache.Service.VersionKey + virtualPath.Replace(@"\", "_").Replace(@"/", "_");
            }

            return base.GetCacheKey(virtualPath);
        }

        /// <inheritdoc />
        public override string GetFileHash(string virtualPath, System.Collections.IEnumerable virtualPathDependencies)
        {
            // Ensure the hash contains the version key as the view in the Repo may have been changed when the app is offline
            // this forces the runtime to recompile each view stored in the Repo each time the application starts and/or the 
            // the version key is updated
            if (_viewPersistenceService.Service.Exists(virtualPath))
            {
                return virtualPath + _virtualTemplatesCache.Service.VersionKey;
            }

            return base.GetFileHash(virtualPath, virtualPathDependencies);
        }
    }
}
