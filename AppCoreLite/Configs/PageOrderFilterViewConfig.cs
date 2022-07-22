using AppCoreLite.Enums;

namespace AppCoreLite.Configs
{
    public class PageOrderFilterViewConfig : PageOrderViewConfig
    {
        public string Filter { get; set; }
        public string Search { get; set; }
        public string Clear { get; set; }

        public PageOrderFilterViewConfig()
        {
            Filter = "Filtre";
            Search = "Ara";
            Clear = "Temizle";
        }

        public override void Set(Languages language)
        {
            base.Set(language);
            Filter = language == Languages.Turkish ? "Filtre" : "Filter";
            Search = language == Languages.Turkish ? "Ara" : "Search";
            Clear = language == Languages.Turkish ? "Temizle" : "Clear";
        }
    }
}
