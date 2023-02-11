using AppCoreLite.Configs;
using AppCoreLite.Configs.Bases;
using AppCoreLite.Enums;
using AppCoreLite.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppCoreLite.ViewModels
{
    public class PageOrderViewModel : IConfig
    {
        public PageOrder PageOrder { get; set; }
        public SelectList PageNumbers { get; set; }
        public SelectList RecordsPerPageCounts { get; set; }
        public SelectList OrderExpressions { get; set; }

        public PageOrderViewConfig Config { get; }

        public PageOrderViewModel()
        {
            PageOrder = new PageOrder();
            Config = new PageOrderViewConfig();
        }

        public void Set(Languages language)
        {
            Config.Set(language);
        }
    }

    public class PageOrderFilterViewModel : IConfig
    {
        public PageOrderFilter PageOrderFilter { get; set; }
        public SelectList PageNumbers { get; set; }
        public SelectList RecordsPerPageCounts { get; set; }
        public SelectList OrderExpressions { get; set; }
        public string? Filter { get; set; }

        public PageOrderFilterViewConfig Config { get; }

        public PageOrderFilterViewModel()
        {
            PageOrderFilter = new PageOrderFilter();
            Config = new PageOrderFilterViewConfig();
        }

        public void Set(Languages language)
        {
            Config.Set(language);
        }
    }
}
