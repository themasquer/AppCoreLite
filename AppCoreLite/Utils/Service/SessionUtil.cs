using AppCoreLite.Utils.Bases.Service;
using Microsoft.AspNetCore.Http;

namespace AppCoreLite.Utils.Service
{
    public class SessionUtil : SessionUtilBase
    {
        public SessionUtil(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
    }
}
