using AppCoreLite.Contexts.Bases;
using AppCoreLite.Entities;
using DataAccessDemo.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessDemo.Contexts
{
    public class Db : DbContext, IAccountContext
    {
        public DbSet<ProductEntity> ProductEntities { get; set; }
        public DbSet<CategoryEntity> CategoryEntities { get; set; }
        public DbSet<StoreEntity> StoreEntities { get; set; }
        public DbSet<ProductStoreEntity> ProductStoreEntities { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<ProductStore> ProductStores { get; set; }

        public DbSet<AccountUser> AccountUsers { get; set; }
        public DbSet<AccountRole> AccountRoles { get; set; }

        public Db(DbContextOptions<Db> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductStoreEntity>()
                .HasKey(ps => new { ps.ProductEntityId, ps.StoreEntityId });

            modelBuilder.Entity<ProductStore>()
                .HasKey(ps => new { ps.ProductId, ps.StoreId });
        }
    }
}
