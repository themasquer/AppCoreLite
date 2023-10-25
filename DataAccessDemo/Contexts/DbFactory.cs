using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataAccessDemo.Contexts
{
    public class DbFactory : IDesignTimeDbContextFactory<Db>
    {
        public Db CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Db>();
            optionsBuilder.UseSqlServer("server=(localdb)\\mssqllocaldb;database=AppCoreLiteETradeDemo;trusted_connection=true;multipleactiveresultsets=true;trustservercertificate=true;");
            return new Db(optionsBuilder.Options);
        }
    }
}
