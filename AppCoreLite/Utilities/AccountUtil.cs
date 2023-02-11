using AppCoreLite.Entities;
using AppCoreLite.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AppCoreLite.Utilities
{
    public class AccountUtil
    {
        private User? _user;

        public AccountUtil(IHttpContextAccessor httpContextAccessor)
        {
            _user = null;
            if (httpContextAccessor.HttpContext.User.Identity != null && httpContextAccessor.HttpContext.User.Claims != null && httpContextAccessor.HttpContext.User.Claims.Count() > 0)
            {
                var primarySidClaim = httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(u => u.Type == ClaimTypes.PrimarySid);
                if (primarySidClaim != null)
                {
                    _user = new User()
                    {
                        UserName = httpContextAccessor.HttpContext.User.Identity.Name,
                        Roles = httpContextAccessor.HttpContext.User.Claims.Where(u => u.Type == ClaimTypes.Role).Select(u => u.Value).ToList(),
                        Guid = httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(u => u.Type == ClaimTypes.Sid)?.Value,
                        Id = Convert.ToInt32(primarySidClaim.Value)
                    };
                }
            }
        }

        public AccountUtil()
        {
            _user = null;
        }

        public User? GetUser()
        {
            return _user;
        }

        public User? GetUser(AccountUser accountUser)
        {
            _user = null;
            if (accountUser != null && !string.IsNullOrWhiteSpace(accountUser.UserName) && accountUser.AccountRole != null && !string.IsNullOrWhiteSpace(accountUser.AccountRole.RoleName))
            {
                _user = new User()
                {
                    UserName = accountUser.UserName,
                    Guid = accountUser.Guid,
                    Id = accountUser.Id,
                    Roles = new List<string>() { accountUser.AccountRole.RoleName }
                };
            }
            return _user;
        }
    }
}
