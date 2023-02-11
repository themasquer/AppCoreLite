using Microsoft.Extensions.Configuration;

namespace AppCoreLite.Utilities
{
    public class AppSettingsUtil
    {
        private readonly IConfiguration _configuration;
        private readonly string _sectionKey;

        public AppSettingsUtil(IConfiguration configuration, string sectionKey = "AppSettings")
        {
            _configuration = configuration;
            _sectionKey = sectionKey;
        }

        public virtual void Bind<T>() where T : class, new()
        {
            T? t = null;
            IConfigurationSection section = _configuration.GetSection(_sectionKey);
            if (section != null)
            {
                t = new T();
                section.Bind(t);
            }
        }
    }
}
