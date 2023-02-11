using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataAccessDemo.Contexts
{
    public class UnitDbFactory : IDesignTimeDbContextFactory<UnitDb>
    {
        public UnitDb CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UnitDb>();
            optionsBuilder.UseSqlServer("server=.\\SQLEXPRESS;database=ETradeAppCoreLiteDemo;user id=sa;password=sa;multipleactiveresultsets=true;trustservercertificate=true;");
            return new UnitDb(optionsBuilder.Options);
        }
    }
}
