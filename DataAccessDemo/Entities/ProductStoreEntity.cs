using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessDemo.Entities
{
    public partial class ProductStoreEntity
    {
        [Key]
        [Column(Order = 0)]
        public int ProductEntityId { get; set; }

        public ProductEntity? ProductEntity { get; set; }

        [Key]
        [Column(Order = 1)]
        public int StoreEntityId { get; set; }

        public StoreEntity? StoreEntity { get; set; }
    }
}
