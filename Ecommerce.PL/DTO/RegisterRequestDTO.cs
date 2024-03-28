using System.ComponentModel.DataAnnotations;

namespace Ecommerce.PL.DTO
{
    public class RegisterRequestDTO
    {
        [Required]
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public bool isAgree { get; set; }
    }
}
