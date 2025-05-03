using AllFoods.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using AllFoods.Core.ValidationAttributes;
using AllFoods.Core.Settinges;

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
        [Range(1,10000,ErrorMessage = "Price should be between 1 and 10000")]
        public decimal Price { get; set; }
        public Guid? CategoryID { get; set; }
        public string? ImageUrl { get; set; } = string.Empty;
        [Required(ErrorMessage = "{0} should not be blank")]
        [AllowedExtensions(FileSettings.AllowedExtinsions),AllowedFileSize(FileSettings.MaxFileSizeInBytes)]
        public IFormFile Cover { get; set; } = default!;

    }
}
