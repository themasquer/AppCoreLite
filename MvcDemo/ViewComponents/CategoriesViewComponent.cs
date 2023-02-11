using AppCoreLite.Enums;
using AppCoreLite.Models;
using DataAccessDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace MvcDemo.ViewComponents
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly CategoryServiceBase _categoryService;

        public CategoriesViewComponent(CategoryServiceBase categoryService)
        {
            _categoryService = categoryService;
            _categoryService.Set(Languages.English);
        }

        public ViewViewComponentResult Invoke()
        {
            return View(_categoryService.GetList(new PageOrderFilter()));
        }
    }
}
