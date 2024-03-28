using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Ecommerce.Core.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }


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
        public string ImageUrl { get; set; }


        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public Category Category { get; set; }


        [JsonIgnore]
        [IgnoreDataMember]  //=> we can add these two anotation to Handling JSON cycle problem in lazy loading but this soluion is not practical
        public List<Cart> CartItems { get; set; } = new List<Cart>();


    }

}
