using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Core.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal? Price { get; set; }


        public string? Description { get; set; }

        [Required]
        public int? Amount { get; set; }


        public float? Rating { get; set; }

        public string? Color { get; set; }

        //public IEnumerable<string>? ImageUrl { get; set; }

        public string? ImageUrl { get; set; }


        [ForeignKey(nameof(Category))]
        public int? CategoryId { get; set; }


        public Category? Category { get; set; }

    }

}
