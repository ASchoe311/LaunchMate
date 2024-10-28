using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LaunchMate.Utilities;

namespace LaunchMate.Models
{
    //public class ProcessCache
    //{
    //    //private List<Tuple<string, bool>> _cache;
    //    private Dictionary<string, string> _cacheDict;
    //    private HashSet<string> _cachePaths;

    //    public ProcessCache()
    //    {
    //        //_cacheDict = new Dictionary<string, string>();
    //        _cachePaths = new HashSet<string>();
    //    }

    //    public void Clear()
    //    {
    //        //_cacheDict.Clear();
    //        _cachePaths.Clear();
    //    }

    //    public void Add(string key, string value)
    //    {
    //        //if (key.ToLow)
    //        //_cacheDict.Add(key.ToLowerInvariant(), value);
    //        _cachePaths.Add(value.ToLowerInvariant());
    //    }

    //    public bool TryAccess(string key, out string value)
    //    {
    //        //foreach (var k in _cacheDict.Keys)
    //        //{
    //        //    if (k.Contains(key, StringComparison.InvariantCultureIgnoreCase))
    //        //    {
    //        //        value = _cacheDict[k];
    //        //        return true;
    //        //    }
    //        //}
    //        //value = string.Empty;
    //        //return false;
    //        value = _cachePaths.FirstOrDefault((x) =>
    //        {
    //            return 
    //            (
    //                x == key.ToLowerInvariant() ||
    //                Path.GetFileName(x) == key.ToLowerInvariant() ||
    //                Path.GetFileNameWithoutExtension(x) == key.ToLowerInvariant()
    //            );
    //        });
    //        return value != string.Empty;
    //    }

    //}

    public class Cache
    {
        public class CacheType
        {
            private Dictionary<string, bool> _cache;

            public CacheType()
            {
                _cache = new Dictionary<string, bool>();
            }

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

        public void Clear()
        {
            ExeCache.Clear();
            ProcessCache.Clear();
            ServiceCache.Clear();
        }

    }


}
