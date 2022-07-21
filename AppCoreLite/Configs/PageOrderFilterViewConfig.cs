using AppCoreLite.Configs.Bases;
using AppCoreLite.Enums;

namespace AppCoreLite.Configs
{
    public class PageOrderFilterViewConfig : IConfig
    {
        public string Filter { get; set; }
        public string Search { get; set; }
        public string Clear { get; set; }
        public string PageNumber { get; set; }
        public string RecordsCount { get; set; }
        public string Order { get; set; }
        public string Ascending { get; set; }

        public PageOrderFilterViewConfig()
        {
            Filter = "Filtre";
            Search = "Ara";
            Clear = "Temizle";
            PageNumber = "Sayfa";
            RecordsCount = "Kayıt Sayısı";
            Order = "Sıra";
            Ascending = "Artan";
        }

        public void Set(Languages language)
        {
            Filter = language == Languages.Turkish ? "Filtre" : "Filter";
            Search = language == Languages.Turkish ? "Ara" : "Search";
            Clear = language == Languages.Turkish ? "Temizle" : "Clear";
            PageNumber = language == Languages.Turkish ? "Sayfa" : "Page";
            RecordsCount = language == Languages.Turkish ? "Kayıt Sayısı" : "Records Count";
            Order = language == Languages.Turkish ? "Sıra" : "Order";
            Ascending = language == Languages.Turkish ? "Artan" : "Ascending";
        }
    }
}
