using AppCoreLite.Records.Bases;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessDemo.Entities
{
    public partial class StoreEntity : Record, ISoftDelete, IModifiedBy
    {
        [Required(ErrorMessage = "{0} is required!")]
        [StringLength(150, ErrorMessage = "{0} must have maximum {1} characters!")]
        [DisplayName("Store Name")]
        public string? Name { get; set; }

        [DisplayName("Virtual")]
        public bool IsVirtual { get; set; }

        public List<ProductStoreEntity>? ProductStoreEntities { get; set; }

        public bool? IsDeleted { get; set; }

        [DisplayName("Create Date")]
        public DateTime? CreateDate { get; set; }

        [DisplayName("Created By")]
        public string? CreatedBy { get; set; }

        [DisplayName("Update Date")]
        public DateTime? UpdateDate { get; set; }

        [DisplayName("Updated By")]
        public string? UpdatedBy { get; set; }
    }

    public partial class StoreEntity
    {
        [NotMapped]
        [DisplayName("Virtual")]
        public string? IsVirtualDisplay { get; set; }

        [NotMapped]
        [DisplayName("Products")]
        public string? ProductsEntityDisplay { get; set; }

        [NotMapped]
        [DisplayName("Products")]
        public List<int>? ProductEntityIds { get; set; }

        [NotMapped]
        [DisplayName("Create Date")]
        public string? CreateDateDisplay { get; set; }

        [NotMapped]
        [DisplayName("Update Date")]
        public string? UpdateDateDisplay { get; set; }
    }
}
