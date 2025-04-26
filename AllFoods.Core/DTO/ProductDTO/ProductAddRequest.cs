using AllFoods.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AllFoods.Core.DTO.ProductDTO
{
    public class ProductAddRequest
    {
        [Required(ErrorMessage = "{0} should not be blank")]
        [StringLength(50)]
        public string ProductName { get; set; } = string.Empty;
        [Required(ErrorMessage = "{0} should not be blank")]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "{0} should not be blank")]
        public decimal Price { get; set; }
        public Guid? CategoryID { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        [Required(ErrorMessage = "{0} should not be blank")]

        public IFormFile Cover { get; set; } = default!;

    }
}
