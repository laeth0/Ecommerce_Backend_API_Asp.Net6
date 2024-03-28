using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace Ecommerce.Core.Models
{
    public class User : IdentityUser
    {

        [Required]
        public bool isAgree { get; set; } = false;

    }

}
