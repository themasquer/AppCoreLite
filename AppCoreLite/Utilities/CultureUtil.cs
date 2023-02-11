using AppCoreLite.Configs.Bases;
using AppCoreLite.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace AppCoreLite.Utilities
{
    public class CultureUtil : IConfig
    {
        private readonly List<CultureInfo> _cultures;

        public CultureUtil()
        {
            _cultures = new List<CultureInfo>() { new CultureInfo("tr-TR") };
        }

        public void Set(Languages language)
        {
            _cultures.Clear();
            if (language == Languages.Turkish)
                _cultures.Add(new CultureInfo("tr-TR"));
            else
                _cultures.Add(new CultureInfo("en-US"));
        }

        public Action<RequestLocalizationOptions> AddCulture()
        {
            Action<RequestLocalizationOptions> action = options =>
            {
                options.DefaultRequestCulture = new RequestCulture(_cultures?.FirstOrDefault()?.Name);
                options.SupportedCultures = _cultures;
                options.SupportedUICultures = _cultures;
            };
            return action;
        }

        public RequestLocalizationOptions UseCulture()
        {
            RequestLocalizationOptions options = new RequestLocalizationOptions()
            {
                DefaultRequestCulture = new RequestCulture(_cultures?.FirstOrDefault()?.Name),
                SupportedCultures = _cultures,
                SupportedUICultures = _cultures
            };
            return options;
        }
    }
}
