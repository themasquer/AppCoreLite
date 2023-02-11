using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessDemo.Entities
{
    public class ProductStore
    {
        [Key]
        [Column(Order = 0)]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        [Key]
        [Column(Order = 1)]
        public int StoreId { get; set; }

        public Store? Store { get; set; }
    }
}
