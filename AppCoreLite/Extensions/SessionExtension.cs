using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;

namespace AppCoreLite.Extensions
{
    public static class SessionExtension
    {
        public static void SetObject<T>(this ISession session, string key, T? value) where T : class
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                string serializedValue = JsonConvert.SerializeObject(value);
                byte[] bytes = Encoding.UTF8.GetBytes(serializedValue);
                session.Set(key, bytes);
            }
        }

        public static T? GetObject<T>(this ISession session, string key) where T : class
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            byte[] bytes;
            session.TryGetValue(key, out bytes);
            if (bytes == null)
                return null;
            string deserializedValue = Encoding.UTF8.GetString(bytes);
            T? value = JsonConvert.DeserializeObject<T>(deserializedValue);
            return value;
        }
    }
}
