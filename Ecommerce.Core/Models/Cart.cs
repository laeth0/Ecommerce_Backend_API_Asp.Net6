using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Core.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Quantity { get; set; } = 1;


        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }


        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
       
    }
}
