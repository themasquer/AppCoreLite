using AppCoreLite.Extensions;
using Microsoft.AspNetCore.Http;

namespace AppCoreLite.Utilities
{
    public class SessionUtil
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionUtil(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void ClearSession(string key)
        {
            _httpContextAccessor.HttpContext.Session.Remove(key);
        }

        public T? GetSessionObject<T>(string key) where T : class
        {
            return _httpContextAccessor.HttpContext.Session.GetObject<T>(key);
        }

        public void SetSessionObject<T>(string key, T? sessionObject) where T : class
        {
            _httpContextAccessor.HttpContext.Session.SetObject(key, sessionObject);
        }

        public int? GetSessionInt(string key)
        {
            return _httpContextAccessor.HttpContext.Session.GetInt32(key);
        }

        public void SetSessionInt(string key, int value)
        {
            _httpContextAccessor.HttpContext.Session.SetInt32(key, value);
        }

        public string GetSessionString(string key)
        {
            return _httpContextAccessor.HttpContext.Session.GetString(key);
        }

        public void SetSessionString(string key, string value)
        {
            _httpContextAccessor.HttpContext.Session.SetString(key, value);
        }
    }
}
