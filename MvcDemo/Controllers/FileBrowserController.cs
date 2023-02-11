using AppCoreLite.Enums;
using AppCoreLite.Managers.Bases;
using Microsoft.AspNetCore.Mvc;
using MvcDemo.Settings;

namespace MvcDemo.Controllers
{
    public class FileBrowserController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly FileBrowserManagerBase _fileBrowserManager;

        public FileBrowserController(IWebHostEnvironment environment, FileBrowserManagerBase fileBrowserManager)
        {
            _environment = environment;
            _fileBrowserManager = fileBrowserManager;
            _fileBrowserManager.Set("FileBrowser", "Index", _environment.WebRootPath, AppSettings.TextFilesRootPath);
        }

        public IActionResult Index(string? path = null)
        {
            var contentsViewModel = _fileBrowserManager.GetContents(path);
            if (contentsViewModel.FileType == FileTypes.Compressed)
                return File(contentsViewModel.FileBinaryContent, contentsViewModel.FileContentType, contentsViewModel.Title);
            return View(contentsViewModel);
        }
    }
}
