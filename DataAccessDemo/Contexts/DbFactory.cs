using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataAccessDemo.Contexts
{
    public class DbFactory : IDesignTimeDbContextFactory<Db>
    {
        public Db CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Db>();
            optionsBuilder.UseSqlServer("server=.\\SQLEXPRESS;database=ETradeAppCoreLiteDemo;user id=sa;password=sa;multipleactiveresultsets=true;trustservercertificate=true;");
            return new Db(optionsBuilder.Options);
        }
    }
}
