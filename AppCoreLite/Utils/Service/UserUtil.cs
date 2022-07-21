using AppCoreLite.Utils.Bases.Service;
using Microsoft.AspNetCore.Http;

namespace AppCoreLite.Utils.Service
{
    public class UserUtil : UserUtilBase
    {
        public UserUtil(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
    }
}
