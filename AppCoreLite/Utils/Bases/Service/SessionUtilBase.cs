using Microsoft.AspNetCore.Http;
using AppCoreLite.Extensions;

namespace AppCoreLite.Utils.Bases.Service
{
    public abstract class SessionUtilBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected SessionUtilBase(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public virtual void ClearSession(string key)
        {
            _httpContextAccessor.HttpContext.Session.Remove(key);
        }

        public virtual T? GetSessionObject<T>(string key) where T : class
        {
            return _httpContextAccessor.HttpContext.Session.GetObject<T>(key);
        }

        public virtual void SetSessionObject<T>(string key, T? sessionObject) where T : class
        {
            _httpContextAccessor.HttpContext.Session.SetObject(key, sessionObject);
        }

        public virtual int? GetSessionInt(string key)
        {
            return _httpContextAccessor.HttpContext.Session.GetInt32(key);
        }

        public virtual void SetSessionInt(string key, int value)
        {
            _httpContextAccessor.HttpContext.Session.SetInt32(key, value);
        }

        public virtual string GetSessionString(string key)
        {
            return _httpContextAccessor.HttpContext.Session.GetString(key);
        }

        public virtual void SetSessionString(string key, string value)
        {
            _httpContextAccessor.HttpContext.Session.SetString(key, value);
        }
    }
}
