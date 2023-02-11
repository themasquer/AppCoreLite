using AppCoreLite.Records.Bases;

namespace AppCoreLite.Entities
{
    public class TreeNode : Record, ISoftDelete, IModifiedBy
    {
        public int ParentId { get; set; }
        public string? NameTurkish { get; set; }
        public string? NameEnglish { get; set; }
        public string? TextTurkish { get; set; }
        public string? TextEnglish { get; set; }
        public string? AbbreviationTurkish { get; set; }
        public string? AbbreviationEnglish { get; set; }
        public bool IsActive { get; set; }
        public int TreeNodeDetailId { get; set; }
        public TreeNodeDetail? TreeNodeDetail { get; set; }

        public bool? IsDeleted { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class TreeNodeDetail : Record, ISoftDelete, IModifiedBy
    {
        public string? TextTurkish { get; set; }
        public string? TextEnglish { get; set; }
        public int Level { get; set; }
        public List<TreeNode>? TreeNodes { get; set; }

        public bool? IsDeleted { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
