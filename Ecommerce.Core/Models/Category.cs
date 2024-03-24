using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Core.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }


        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string Name { get; set; }


        public string? Description { get; set; }


        public IEnumerable<Product>? Products { get; set; }=new List<Product>();
    }
}
