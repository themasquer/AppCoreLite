using AppCoreLite.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppCoreLite.Contexts.Bases
{
    public interface IAccountContext
    {
        DbSet<AccountUser> AccountUsers { get; set; }
        DbSet<AccountRole> AccountRoles { get; set; }
    }
}
