using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataAccessDemo.Contexts
{
    public class UnitDbFactory : IDesignTimeDbContextFactory<UnitDb>
    {
        public UnitDb CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UnitDb>();
            optionsBuilder.UseSqlServer("server=(localdb)\\mssqllocaldb;database=AppCoreLiteETradeDemo;trusted_connection=true;multipleactiveresultsets=true;trustservercertificate=true;");
            return new UnitDb(optionsBuilder.Options);
        }
    }
}
