using AppCoreLite.Managers.Bases;
using Microsoft.AspNetCore.Http;

namespace DataAccessDemo.Managers
{
    public abstract class StoreExportManagerBase : ExportManagerBase
    {
        protected StoreExportManagerBase(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
    }

    public class StoreExportManager : StoreExportManagerBase
    {
        public StoreExportManager(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
    }
}
