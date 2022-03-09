using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroceryManagement.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(300)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(50)]
        public string Barcode { get; set; } = string.Empty;
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;
        public double? Weight { get; set; }
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;
        public int? CategoryID { get; set; }
    }
}
