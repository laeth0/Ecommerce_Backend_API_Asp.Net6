using Ecommerce.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.PL.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

    }
}
