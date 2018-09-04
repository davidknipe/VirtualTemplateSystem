using System;
using EPiServer.Framework.Cache;
using EPiServer.ServiceLocation;
using VirtualTemplates.Core.Interfaces;

namespace VirtualTemplates.Core.Impl
{
    [ServiceConfiguration(typeof(IVirtualTemplatesCache), Lifecycle = ServiceInstanceScope.Singleton)]
    public class VirtualTemplatesCache : IVirtualTemplatesCache
    {
        private static readonly string _versionCacheKey = "__VirtualTemplatesCacheKey";
        private readonly ISynchronizedObjectInstanceCache _cache;

        public VirtualTemplatesCache(ISynchronizedObjectInstanceCache cache)
        {
            _cache = cache;
        }

        public string VersionKey
        {
            get
            {
                if (_cache.Get(_versionCacheKey) == null)
                {
                    var cacheVal = Guid.NewGuid().ToString();
                    _cache.Insert(_versionCacheKey, cacheVal, CacheEvictionPolicy.Empty);
                    return cacheVal;
                }
                return _cache.Get(_versionCacheKey).ToString();
            }
            private set => _cache.Insert(_versionCacheKey, value, CacheEvictionPolicy.Empty);
        }

        public void Reset() => VersionKey = Guid.NewGuid().ToString();
    }
}
