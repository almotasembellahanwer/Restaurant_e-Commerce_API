using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllFoods.Core.Domain.Entities
{
    public class Product
    {
        public Guid ProductID { get; set; }
        [MaxLength(50)]
        public string ProductName { get; set; } = string.Empty;
        [Required(ErrorMessage = "{0} should not be blank")]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        public Guid? CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public Category? Category { get; set; } = default!;


    }
}
