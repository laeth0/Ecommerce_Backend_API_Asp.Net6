using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Ecommerce.Core.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }


        [MaxLength(50)]
        [MinLength(2)]
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }


        [JsonIgnore]
        [IgnoreDataMember]  //=> we can add these two anotation to Handling JSON cycle problem in lazy loading but this soluion is not practical
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
    }
}
