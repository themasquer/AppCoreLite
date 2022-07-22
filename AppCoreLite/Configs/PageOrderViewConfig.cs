using AppCoreLite.Configs.Bases;
using AppCoreLite.Enums;

namespace AppCoreLite.Configs
{
    public class PageOrderViewConfig : IConfig
    {
        public string PageNumber { get; set; }
        public string RecordsCount { get; set; }
        public string Order { get; set; }
        public string Ascending { get; set; }

        public PageOrderViewConfig()
        {
            PageNumber = "Sayfa";
            RecordsCount = "Kayıt Sayısı";
            Order = "Sıra";
            Ascending = "Artan";
        }

        public virtual void Set(Languages language)
        {
            PageNumber = language == Languages.Turkish ? "Sayfa" : "Page";
            RecordsCount = language == Languages.Turkish ? "Kayıt Sayısı" : "Records Count";
            Order = language == Languages.Turkish ? "Sıra" : "Order";
            Ascending = language == Languages.Turkish ? "Artan" : "Ascending";
        }
    }
}
