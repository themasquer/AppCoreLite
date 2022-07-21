using AppCoreLite.Configs.Bases;
using AppCoreLite.Enums;

namespace AppCoreLite.Configs
{
    public class PageOrderFilterViewConfig : IConfig
    {
        public virtual string Filter { get; set; }
        public virtual string Search { get; set; }
        public virtual string Clear { get; set; }
        public virtual string PageNumber { get; set; }
        public virtual string RecordsCount { get; set; }
        public virtual string Order { get; set; }
        public virtual string Ascending { get; set; }

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
