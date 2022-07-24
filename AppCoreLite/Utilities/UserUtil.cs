using AppCoreLite.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AppCoreLite.Utilities
{
    public class UserUtil
    {
        public User? User { get; }

        public UserUtil(IHttpContextAccessor httpContextAccessor)
        {
            User = null;
            if (httpContextAccessor.HttpContext.User.Identity != null && httpContextAccessor.HttpContext.User.Claims != null && httpContextAccessor.HttpContext.User.Claims.Count() > 0)
            {
                var primarySidClaim = httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(u => u.Type == ClaimTypes.PrimarySid);
                if (primarySidClaim != null)
                {
                    User = new User()
                    {
                        UserName = httpContextAccessor.HttpContext.User.Identity.Name,
                        Roles = httpContextAccessor.HttpContext.User.Claims.Where(u => u.Type == ClaimTypes.Role).Select(u => u.Value).ToList(),
                        Guid = httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(u => u.Type == ClaimTypes.Sid)?.Value,
                        Id = Convert.ToInt32(primarySidClaim.Value)
                    };
                }
            }
        }
    }
}
