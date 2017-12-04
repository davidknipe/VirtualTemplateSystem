using System;

namespace VirtualTemplates.Core.Impl
{
    public class VirtualTemplatesCache
    {
        private static readonly string _versionCacheKey = "__FastViewsCacheKey";

        public static string VersionKey
        {
            get
            {
                if (EPiServer.CacheManager.Get(_versionCacheKey) == null)
                {
                    var cacheVal = Guid.NewGuid().ToString();
                    EPiServer.CacheManager.Insert(_versionCacheKey, cacheVal);
                    return cacheVal;
                }
                return EPiServer.CacheManager.Get(_versionCacheKey).ToString();
            }
            private set
            {
                EPiServer.CacheManager.Insert(_versionCacheKey, value);
            }
        }

        public static void Reset()
        {
            VersionKey = Guid.NewGuid().ToString();
        }
    }
}
