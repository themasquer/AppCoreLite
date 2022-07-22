using AppCoreLite.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AppCoreLite.Utilities.Bases
{
    public abstract class UserUtilBase
    {
        public User User { get; set; }

        protected UserUtilBase(IHttpContextAccessor httpContextAccessor)
        {
            User = new User()
            {
                UserName = httpContextAccessor.HttpContext.User.Identity?.Name,
                Roles = httpContextAccessor.HttpContext.User.Claims.Where(u => u.Type == ClaimTypes.Role).Select(u => u.Value).ToList(),
                Guid = httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(u => u.Type == ClaimTypes.Sid)?.Value,
                Id = Convert.ToInt32(httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(u => u.Type == ClaimTypes.PrimarySid)?.Value)
            };
        }
    }
}
