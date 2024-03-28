using Ecommerce.Core.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.PL.DTO
{
    public class ProductResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public string Color { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }

    }
}
