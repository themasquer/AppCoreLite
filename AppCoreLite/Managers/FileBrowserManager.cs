using AppCoreLite.Managers.Bases;
using Microsoft.AspNetCore.Http;

namespace AppCoreLite.Managers
{
    public class FileBrowserManager : FileBrowserManagerBase
    {
        public FileBrowserManager(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
    }
}
