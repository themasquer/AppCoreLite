using AppCoreLite.Attributes;
using AppCoreLite.Records.Bases;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DataAccessDemo.Entities
{
    public partial class ProductEntity : Record, IRecordFile
    {
        [Required(ErrorMessage = "{0} is required!")]
        [MinLength(3, ErrorMessage = "{0} must have minimum {1} characters!")]
        [MaxLength(200, ErrorMessage = "{0} must have maximum {1} characters!")]
        [DisplayName("Product Name")]
        [OrderTag]
        [StringFilterTag]
        public string? Name { get; set; }

        [DisplayName("Product Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [Range(0, double.MaxValue, ErrorMessage = "{0} must be {1} or positive!")]
        [DisplayName("Unit Price")]
        [OrderTag]
        public double? UnitPrice { get; set; }

        [Required(ErrorMessage = "{0} is required!")]
        [DisplayName("Category")]
        public int? CategoryEntityId { get; set; }

        public CategoryEntity? CategoryEntity { get; set; }

        [Range(0, 1000000, ErrorMessage = "{0} must be between {1} and {2}!")]
        [DisplayName("Stock Amount")]
        [OrderTag]
        public int? StockAmount { get; set; }

        [DisplayName("Expiration Date")]
        [OrderTag]
        public DateTime? ExpirationDate { get; set; }

        [Column(TypeName = "image")]
        [DisplayName("Image")]
        public byte[]? FileData { get; set; }

        [DisplayName("Image")]
        public string? FileContent { get; set; }

        public string? FilePath { get; set; }

        public List<ProductStoreEntity>? ProductStoreEntities { get; set; }
    }

    public partial class ProductEntity
    {
        [NotMapped]
        [DisplayName("Category")]
        [OrderTag]
        [StringFilterTag]
        public string? CategoryEntityDisplay { get; set; }

        [NotMapped]
        [DisplayName("Stores")]
        public List<int>? StoreEntityIds { get; set; }

        [NotMapped]
        [DisplayName("Stores")]
        public List<StoreEntity>? StoreEntities { get; set; }

        [NotMapped]
        [DisplayName("Unit Price")]
        public string? UnitPriceDisplay { get; set; }

        [NotMapped]
        [DisplayName("Expiration Date")]
        public string? ExpirationDateDisplay { get; set; }
    }
}
