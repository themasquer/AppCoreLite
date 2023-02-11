using AppCoreLite.Configs;
using AppCoreLite.Entities;
using AppCoreLite.Enums;
using AppCoreLite.Models;
using AppCoreLite.Results.Bases;
using AppCoreLite.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppCoreLite.Services
{
    public class AccountService : EntityService<AccountUser> 
    {
        protected readonly JwtUtil _jwtUtil;

        public AccountServiceConfig AccountServiceConfig { get; }

        public AccountService(DbContext db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        {
            _jwtUtil = new JwtUtil();
            AccountServiceConfig = new AccountServiceConfig();
        }

        public override void Set(Languages language)
        {
            AccountServiceConfig.Set(language);
            base.Set(language);
        }

        public override IQueryable<AccountUser> Query(params Expression<Func<AccountUser, object?>>[] navigationPropertyPaths)
        {
            return base.Query(q => q.AccountRole).Where(q => (q.IsDeleted ?? false) == false && (q.AccountRole.IsDeleted ?? false) == false);
        }

        public virtual Result<AccountUser> Login(AccountLogin accountUser)
        {
            var userEntity = GetItem(u => u.UserName == accountUser.UserName.Trim() && u.Password == accountUser.Password.Trim() && u.IsActive);
            if (userEntity == null)
                return Error<AccountUser>(AccountServiceConfig.UserNotFound);
            userEntity.Password = "";
            return Success(userEntity);
        }

        public virtual Result<Jwt> CreateJwt(AccountLogin accountUser, bool includeBearer = true)
        {
            var result = Login(accountUser);
            if (result.IsSuccessful)
            {
                var jwt = _jwtUtil.CreateJwt(_userUtil.GetUser(result.Data), includeBearer);
                if (jwt != null)
                    return Success(jwt);
                return Error<Jwt>(Config.OperationFailed);
            }
            return Error<Jwt>(result.Message);
        }

        public virtual Result Register(AccountRegister accountUser)
        {
            var result = base.ItemExists(u => u.UserName == accountUser.UserName.Trim());
            if (result.IsSuccessful)
                return Error(AccountServiceConfig.UserFound + " " + Config.OperationFailed);
            var userEntity = new AccountUser()
            {
                AccountRoleId = (int)Roles.User,
                CreateDate = DateTime.Now,
                CreatedBy = Roles.System.ToString(),
                IsActive = true,
                Password = accountUser.Password,
                UserName = accountUser.UserName
            };
            result = base.Add(userEntity);
            if (result.IsSuccessful)
                result.Message = AccountServiceConfig.UserRegistered;
            return result;
        }
    }
}
