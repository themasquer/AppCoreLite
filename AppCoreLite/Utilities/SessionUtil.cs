using AppCoreLite.Utilities.Bases;
using Microsoft.AspNetCore.Http;

namespace AppCoreLite.Utilities
{
    public class SessionUtil : SessionUtilBase
    {
        public SessionUtil(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
    }
}
