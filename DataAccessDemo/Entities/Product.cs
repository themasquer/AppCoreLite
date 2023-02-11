using AppCoreLite.Attributes;
using AppCoreLite.Records.Bases;
using AppCoreLite.Utilities;
using AutoMapper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.Json.Serialization;

namespace DataAccessDemo.Entities
{
    public class Product : Record, IRecordFile
    {
        [Required(ErrorMessage = "{0} zorunludur!")]
        [MinLength(3, ErrorMessage = "{0} minimum {1} karakter olmalıdır!")]
        [MaxLength(200, ErrorMessage = "{0} maksimum {1} karakter olmalıdır!")]
        [DisplayName("Ürün Adı")]
        [OrderTag]
        [StringFilterTag]
        public string? Name { get; set; }

        [DisplayName("Açıklaması")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "{0} zorunludur!")]
        [Range(0, double.MaxValue, ErrorMessage = "{0} minimum {1} veya pozitif olmalıdır!")]
        [DisplayName("Birim Fiyatı")]
        [OrderTag]
        public double? UnitPrice { get; set; }

        [Required(ErrorMessage = "{0} zorunludur!")]
        [DisplayName("Kategori")]
        public int? CategoryId { get; set; }

        [JsonIgnore]
        public Category? Category { get; set; }

        [Range(0, 1000000, ErrorMessage = "{0} {1} ile {2} arasında olmalıdır!")]
        [DisplayName("Stok Miktarı")]
        [OrderTag]
        public int? StockAmount { get; set; }

        [DisplayName("Son Kullanma Tarihi")]
        [OrderTag]
        public DateTime? ExpirationDate { get; set; }

        [Column(TypeName = "image")]
        [DisplayName("İmaj")]
        [JsonIgnore]
        public byte[]? FileData { get; set; }

        [DisplayName("İmaj")]
        [JsonIgnore]
        public string? FileContent { get; set; }

        [JsonIgnore]
        public string? FilePath { get; set; }

        [JsonIgnore]
        public List<ProductStore>? ProductStores { get; set; }
    }

    public class ProductModel : Product
    {
        [DisplayName("Kategori")]
        [OrderTag]
        [StringFilterTag]
        public string? CategoryDisplay { get; set; }

        [DisplayName("Mağazalar")]
        public List<int>? StoreIds { get; set; }

        [DisplayName("Mağazalar")]
        public string? StoresDisplay { get; set; }

        [DisplayName("Birim Fiyatı")]
        [JsonIgnore]
        public string? UnitPriceDisplay { get; set; }

        [DisplayName("Son Kullanma Tarihi")]
        [JsonIgnore]
        public string? ExpirationDateDisplay { get; set; }

        [DisplayName("Birim Fiyatı (Yazı)")]
        [JsonIgnore]
        public string? UnitPriceTextDisplay { get; set; }
    }

    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductModel>()
                .ForMember(d => d.CategoryDisplay, o => o.MapFrom(s => s.Category.Name))
                .ForMember(d => d.StoreIds, o => o.MapFrom(s => s.ProductStores.Select(ps => ps.StoreId).ToList()))
                .ForMember(d => d.StoresDisplay, o => o.MapFrom(s => string.Join("<br />", s.ProductStores.Select(ps => ps.Store.Name))))
                .ForMember(d => d.UnitPriceDisplay, o => o.MapFrom(s => s.UnitPrice.HasValue ? s.UnitPrice.Value.ToString("C2", new CultureInfo("tr-TR")) : ""))
                .ForMember(d => d.ExpirationDateDisplay, o => o.MapFrom(s => s.ExpirationDate.HasValue ? s.ExpirationDate.Value.ToString("dd.MM.yyyy") : ""))
                .ForMember(d => d.FileContent, o => o.MapFrom(s => RecordFileUtil.GetImgSrc(s, s.Id)));
        }
    }
}
