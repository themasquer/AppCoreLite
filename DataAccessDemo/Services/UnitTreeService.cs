using AppCoreLite.Enums;
using AppCoreLite.Services;
using DataAccessDemo.Configs;
using DataAccessDemo.Contexts;
using Microsoft.AspNetCore.Http;

namespace DataAccessDemo.Services
{
    public class UnitTreeService : TreeNodeService
    {
        public UnitTreeService(UnitDb db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        {
            TreeNodeServiceConfig = new UnitTreeServiceConfig();
        }

        public override void Set(Languages language)
        {
            TreeNodeServiceConfig.Set(language);
            Config.Set(language);
        }
    }
}
