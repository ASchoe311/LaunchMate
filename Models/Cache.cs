using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LaunchMate.Utilities;

namespace LaunchMate.Models
{
    /// <summary>
    /// Provides a simple cache for non-playnite conditions
    /// Used to speed up checking all matched games
    /// </summary>
    public class Cache
    {
        public class CacheType
        {
            private Dictionary<string, bool> _cache;

            public CacheType()
            {
                _cache = new Dictionary<string, bool>();
            }

            /// <summary>
            /// Checks if the cached item is currently running
            /// </summary>
            /// <param name="key">Key to check</param>
            /// <returns>The running status of the cache item with key <paramref name="key"/>, null if <paramref name="key"/> does not exist</returns>
            public bool? IsRunning(string key)
            {
                key = key.ToLowerInvariant();
                //return _found;
                if (_cache.ContainsKey(key))
                {
                    return _cache[key];
                }
                return null;
            }

            /// <summary>
            /// Sets the running status in the cache for an item with given key
            /// </summary>
            /// <param name="key">Name of the item</param>
            /// <param name="running">Running status</param>
            public void SetRunning(string key, bool running)
            {
                key = key.ToLowerInvariant();
                if (!_cache.ContainsKey(key))
                {
                    _cache.Add(key, running);
                }
                else
                {
                    _cache[key] = running;
                }
            }

            /// <summary>
            /// Clears the cache
            /// </summary>
            public void Clear()
            {
                _cache.Clear();
            }

        }

        public CacheType ExeCache;
        public CacheType ProcessCache;
        public CacheType ServiceCache;

        public Cache()
        {
            ExeCache = new CacheType();
            ProcessCache = new CacheType();
            ServiceCache = new CacheType();
        }

        /// <summary>
        /// Clears all portions of cache
        /// </summary>
        public void Clear()
        {
            ExeCache.Clear();
            ProcessCache.Clear();
            ServiceCache.Clear();
        }

    }


}
