using AppCoreLite.Contexts.Bases;
using AppCoreLite.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppCoreLite.Contexts
{
    public class TreeNodeContext : DbContext, ITreeNodeContext
    {
        public DbSet<TreeNode> TreeNodes { get; set; }
        public DbSet<TreeNodeDetail> TreeNodeDetails { get; set; }

        public TreeNodeContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TreeNode>().Property(n => n.Id).ValueGeneratedNever();
        }
    }
}
