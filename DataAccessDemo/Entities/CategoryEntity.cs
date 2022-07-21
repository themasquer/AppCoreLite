using AppCoreLite.Records.Bases;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DataAccessDemo.Entities
{
    public partial class CategoryEntity : Record, ISoftDelete, IModifiedBy
    {
        [Required(ErrorMessage = "{0} is required!")]
        [StringLength(100)]
        [DisplayName("Category Name")]
        public string? Name { get; set; }

        [DisplayName("Category Description")]
        [StringLength(500, ErrorMessage = "{0} must be maximum {1} characters!")]
        public string? Description { get; set; }

        public List<ProductEntity>? ProductEntities { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? CreateDate { get; set; }

        [DisplayName("Created By")]
        public string? CreatedBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        [DisplayName("Updated By")]
        public string? UpdatedBy { get; set; }
    }

    public partial class CategoryEntity
    {
        [NotMapped]
        [DisplayName("")]
        public string? OperationLinks { get; set; }

        [NotMapped]
        public int? ProductCount { get; set; }

        [NotMapped]
        [DisplayName("Product Count")]
        public string? ProductCountDisplay { get; set; }

        [NotMapped]
        [DisplayName("Create Date")]
        public string? CreateDateDisplay { get; set; }

        [NotMapped]
        [DisplayName("Update Date")]
        public string? UpdateDateDisplay { get; set; }
    }
}
