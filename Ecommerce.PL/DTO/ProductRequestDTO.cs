using Ecommerce.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.PL.DTO
{
    public class ProductRequestDTO
    {

        [Required]
        public string Name { get; set; }

        [Required]
        public float Price { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Amount { get; set; } = 1;

        [Required]
        public string Color { get; set; }

        [Required]
        public IFormFile image { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }

}
