using Microsoft.Extensions.Caching.Memory;
using Web2FA.Backend.Shared.Services.Base;

namespace Web2FA.Backend.Shared.Services.Derived
{
    /// <summary>
    /// Author: Can DOĞU (CENTECH)
    /// </summary>
    public sealed class CacheService<TObject> : ServiceSingularBase<CacheService<TObject>>
    {
        private readonly object operationLock = new();
        private readonly MemoryCache memCache;

        public CacheService()
        {
            memCache = new MemoryCache(new MemoryCacheOptions());
        }

        public bool SetObject(string key, TObject? data, int durationMinutes)
        {
            try
            {
                lock (operationLock)
                {
                    if (data == null) return false;

                    var objectOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(durationMinutes)
                    };

                    return memCache.Set(key, data, objectOptions) != null;
                }
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);
            }

            return false;
        }

        public TObject? GetObject(string key)
        {
            try
            {
                lock (operationLock)
                {
                    if (memCache.TryGetValue(key, out TObject? data))
                    {
                        return data;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);
            }

            return default;
        }

        public bool DeleteObject(string key)
        {
            try
            {
                if (!memCache.TryGetValue(key, out _)) return true;

                lock (operationLock)
                {
                    memCache.Remove(key);

                    return true;
                }
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);
            }

            return false;
        }
    }
}
