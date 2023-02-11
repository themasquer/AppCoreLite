using AppCoreLite.Services;
using DataAccessDemo.Contexts;
using Microsoft.AspNetCore.Http;

namespace DataAccessDemo.Services
{
    public class UserAccountService : AccountService
    {
        public UserAccountService(Db db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        {
        }
    }
}
