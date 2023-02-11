using AppCoreLite.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppCoreLite.Contexts.Bases
{
    public interface ITreeNodeContext
    {
        DbSet<TreeNode> TreeNodes { get; set; }
        DbSet<TreeNodeDetail> TreeNodeDetails { get; set; }
    }
}
