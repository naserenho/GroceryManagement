using System.ComponentModel.DataAnnotations;

namespace GroceryManagement.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;
    }
}
