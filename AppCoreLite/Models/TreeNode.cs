using AppCoreLite.Entities;
using AppCoreLite.Enums;

namespace AppCoreLite.Models
{
    public class TreeNodeFilter
    {
        public Languages Language { get; set; }
        public bool ShowDetailTexts { get; set; }
        public bool ShowAbbreviations { get; set; }
        public bool ShowOnlyActive { get; set; }

        public TreeNodeFilter()
        {
            Language = Languages.Turkish;
            ShowDetailTexts = false;
            ShowAbbreviations = false;
            ShowOnlyActive = true;
        }
    }

    public class TreeNodeRecursive : TreeNode
    {
        public List<TreeNodeRecursive>? Nodes { get; set; }
    }
}
