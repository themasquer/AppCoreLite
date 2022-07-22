using AppCoreLite.ViewModels;
using DataAccessDemo.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MvcDemo.Models
{
    public class StoresEntityIndexViewModel : PageOrderFilterViewModel
    {
        public List<StoreEntity> Stores { get; set; }
        public SelectList Products { get; set; }
    }
}
