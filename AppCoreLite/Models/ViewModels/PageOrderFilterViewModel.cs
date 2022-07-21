using AppCoreLite.Configs;
using AppCoreLite.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppCoreLite.Models.ViewModels
{
	public class PageOrderFilterViewModel
    {
        public PageOrderFilter PageOrderFilter { get; set; }
        public SelectList PageNumbers { get; set; }
        public SelectList RecordsPerPageCounts { get; set; }
        public SelectList OrderExpressions { get; set; }
        public string? Filter { get; set; }

        public PageOrderFilterViewConfig Config { get; set; }

        public PageOrderFilterViewModel()
        {
            PageOrderFilter = new PageOrderFilter();
            Config = new PageOrderFilterViewConfig();
        }

        public void SetConfig(Languages language)
        {
            Config.Set(language);
        }
    }
}
