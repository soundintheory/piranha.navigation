using Piranha;
using Piranha.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Extensions
{
    internal static class PiranhaCacheExtensions
    {
        internal sealed class CacheEntry<T>
        {
            public T Value { get; set; }

            public CacheEntry() { }

            public CacheEntry(T value)
            {
                Value = value;
            }
        }

        internal static bool TryGet<T>(this ICache cache, string key, out T value)
        {
            value = default;

            if (cache == null)
            {
                return false;
            }

            var entry = GetValue<CacheEntry<T>>(cache, key);
            
            if (entry != null)
            {
                value = entry.Value;
                return true;
            }

            return false;
        }

        internal static T GetOrAdd<T>(this ICache cache, string key, Func<T> factory)
            => GetOrAdd<T>(cache, key, cacheNullValues: false, factory);

        internal static T GetOrAdd<T>(this ICache cache, string key, bool cacheNullValues, Func<T> factory)
        {
            if (cache == null)
            {
                return factory.Invoke();
            }

            if (!TryGet<T>(cache, key, out var value))
            {
                value = factory.Invoke();

                if (cacheNullValues || value != null)
                {
                    SetValue(cache, key, value);
                }
            }

            return value;
        }

        internal static Task<T> GetOrAddAsync<T>(this ICache cache, string key, Func<Task<T>> factory)
            => GetOrAddAsync(cache, key, cacheNullValues: false, factory);

        internal async static Task<T> GetOrAddAsync<T>(this ICache cache, string key, bool cacheNullValues, Func<Task<T>> factory)
        {
            if (cache == null)
            {
                return await factory.Invoke();
            }

            if (!TryGet<T>(cache, key, out var value))
            {
                value = await factory.Invoke();

                if (cacheNullValues || value != null)
                {
                    SetValue(cache, key, value);
                }
            }

            return value;
        }

        internal static T GetValue<T>(this ICache cache, string key)
        {
            if (cache == null)
            {
                return default;
            }

#if NET8_0_OR_GREATER
            var entry = cache.GetAsync<CacheEntry<T>>(key).GetAwaiter().GetResult();
#else
            var entry = cache.Get<CacheEntry<T>>(key);
#endif
            return entry != null ? entry.Value : default;
        }

        internal static void SetValue<T>(this ICache cache, string key, T value)
        {
            if (cache == null)
            {
                return;
            }

#if NET8_0_OR_GREATER
            cache.SetAsync<CacheEntry<T>>(key, new CacheEntry<T>(value)).GetAwaiter().GetResult();
#else
            cache.Set(key, new CacheEntry<T>(value));
#endif
        }

        internal static void RemoveValue(this ICache cache, string key)
        {
            if (cache == null)
            {
                return;
            }

#if NET8_0_OR_GREATER
            cache.RemoveAsync(key).GetAwaiter().GetResult();
#else
            cache.Remove(key);
#endif
        }
    }
}
