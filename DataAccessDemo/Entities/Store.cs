using AppCoreLite.Attributes;
using AppCoreLite.Records.Bases;
using AutoMapper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataAccessDemo.Entities
{
    public class Store : Record, ISoftDelete, IModifiedBy
    {
        [Required(ErrorMessage = "{0} zorunludur!")]
        [StringLength(150, ErrorMessage = "{0} maksimum {1} karakter olmalıdır!")]
        [DisplayName("Mağaza Adı")]
        [ExportTag]
        public string? Name { get; set; }

        [DisplayName("Sanal")]
        public bool IsVirtual { get; set; }

        [JsonIgnore]
        public List<ProductStore>? ProductStores { get; set; }

        [JsonIgnore]
        public bool? IsDeleted { get; set; }

        [JsonIgnore]
        public DateTime? CreateDate { get; set; }

        [DisplayName("Oluşturan")]
        [ExportTag]
        [JsonIgnore]
        public string? CreatedBy { get; set; }

        [JsonIgnore]
        public DateTime? UpdateDate { get; set; }

        [DisplayName("Güncelleyen")]
        [ExportTag]
        [JsonIgnore]
        public string? UpdatedBy { get; set; }
    }

    public class StoreModel : Store
    {
        [DisplayName("Sanal")]
        [ExportTag]
        [JsonIgnore]
        public string? IsVirtualDisplay { get; set; }

        [DisplayName("Ürünler")]
        [ExportTag]
        public string? ProductsDisplay { get; set; }

        [DisplayName("Ürünler")]
        public List<int>? ProductIds { get; set; }

        [DisplayName("Oluşturulma Tarihi")]
        [ExportTag]
        [JsonIgnore]
        public string? CreateDateDisplay { get; set; }

        [DisplayName("Güncellenme Tarihi")]
        [ExportTag]
        [JsonIgnore]
        public string? UpdateDateDisplay { get; set; }
    }

    public class StoreProfile : Profile
    {
        public StoreProfile()
        {
            CreateMap<Store, StoreModel>()
                .ForMember(d => d.IsVirtualDisplay, o => o.MapFrom(s => s.IsVirtual ? "Evet" : "Hayır"))
                .ForMember(d => d.CreateDateDisplay, o => o.MapFrom(s => s.CreateDate.HasValue ? s.CreateDate.Value.ToString("MM/dd/yyyy HH:mm:ss") : ""))
                .ForMember(d => d.UpdateDateDisplay, o => o.MapFrom(s => s.UpdateDate.HasValue ? s.UpdateDate.Value.ToString("MM/dd/yyyy HH:mm:ss") : ""))
                .ForMember(d => d.ProductsDisplay, o => o.MapFrom(s => string.Join("<br />", s.ProductStores.Select(ps => ps.Product.Name))))
                .ForMember(d => d.ProductIds, o => o.MapFrom(s => s.ProductStores.Select(ps => ps.ProductId).ToList()));
        }
    }
}
