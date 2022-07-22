using AppCoreLite.Utilities.Bases;
using Microsoft.AspNetCore.Http;

namespace AppCoreLite.Utilities
{
    public class UserUtil : UserUtilBase
    {
        public UserUtil(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
    }
}
