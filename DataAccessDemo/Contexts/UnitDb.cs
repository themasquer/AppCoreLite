using AppCoreLite.Contexts;
using Microsoft.EntityFrameworkCore;

namespace DataAccessDemo.Contexts
{
    public class UnitDb : TreeNodeContext
    {
        public UnitDb(DbContextOptions<UnitDb> options) : base(options)
        {
        }
    }
}
