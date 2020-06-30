using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace mr.bBall_Lib
{
    public static class Predpomnilnik
    {
        public static List<string> Keys = new List<string>();
        public static object Get(string key)
        {
            return HttpContext.Current.Cache.Get(key);
        }
        public static void Add(string key, object obj, DateTime expiration)
        {
            try
            {
                Remove(key);
                HttpContext.Current.Cache.Add(key, obj, null, expiration, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                if (!Keys.Contains(key)) Keys.Add(key);
            }
            catch { }
        }
        public static void Add(string key, object obj, int minutes)
        {
            Add(key, obj, DateTime.Now.AddMinutes(minutes));
        }
        public static void Add(string key, object obj)
        {
            Add(key, obj, 60);
        }
        public static void Add(string key, object obj, TimeSpan expiration)
        {
            Add(key, obj, DateTime.Now.Add(expiration));
        }
        public static void Remove(string key)
        {
            try
            {
                HttpContext.Current.Cache.Remove(key);
                if (Keys.Contains(key)) Keys.Remove(key);
            }
            catch { }
        }
        public static void Clear()
        {
            List<string> items = new List<string>();
            foreach (var item in Keys) items.Add(item);
            foreach (var item in items)
            {
                try
                {
                    HttpContext.Current.Cache.Remove(item);
                    if (Keys.Contains(item)) Keys.Remove(item);
                }
                catch { }
            }
        }

    }
}

