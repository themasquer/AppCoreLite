using AppCoreLite.Records.Bases;
using AutoMapper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccessDemo.Entities
{
    public class Category : Record, ISoftDelete, IModifiedBy
    {
        [Required(ErrorMessage = "{0} zorunludur!")]
        [StringLength(100)]
        [DisplayName("Kategori Adı")]
        public string? Name { get; set; }

        [DisplayName("Açıklaması")]
        [StringLength(500, ErrorMessage = "{0} maksimum {1} karakter olmalıdır!")]
        public string? Description { get; set; }

        [JsonIgnore]
        public List<Product>? Products { get; set; }

        [JsonIgnore]
        public bool? IsDeleted { get; set; }

        [JsonIgnore]
        public DateTime? CreateDate { get; set; }

        [DisplayName("Oluşturan")]
        [JsonIgnore]
        public string? CreatedBy { get; set; }

        [JsonIgnore]
        public DateTime? UpdateDate { get; set; }

        [DisplayName("Güncelleyen")]
        [JsonIgnore]
        public string? UpdatedBy { get; set; }
    }

    public class CategoryModel : Category
    {
        [DisplayName("")]
        [JsonIgnore]
        public string? OperationLinks { get; set; }

        [JsonIgnore]
        public int? ProductCount { get; set; }

        [DisplayName("Ürün Sayısı")]
        [JsonIgnore]
        public string? ProductCountDisplay { get; set; }

        [DisplayName("Oluşturulma Tarihi")]
        [JsonIgnore]
        public string? CreateDateDisplay { get; set; }

        [DisplayName("Güncellenme Tarihi")]
        [JsonIgnore]
        public string? UpdateDateDisplay { get; set; }
    }

    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryModel>()
                .ForMember(d => d.ProductCount, o => o.MapFrom(s => s.Products.Count))
                .ForMember(d => d.ProductCountDisplay, o => o.MapFrom(s => s.Products.Count.ToString()))
                .ForMember(d => d.CreateDateDisplay, o => o.MapFrom(s => s.CreateDate.HasValue ? s.CreateDate.Value.ToString("MM/dd/yyyy") : ""))
                .ForMember(d => d.UpdateDateDisplay, o => o.MapFrom(s => s.UpdateDate.HasValue ? s.UpdateDate.Value.ToString("MM/dd/yyyy") : ""));
        }
    }
}
