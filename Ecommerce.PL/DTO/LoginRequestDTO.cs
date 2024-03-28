using System.ComponentModel.DataAnnotations;

namespace Ecommerce.PL.DTO
{
    public class LoginRequestDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
