using AllFoods.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AllFoods.Core.DTO.ProductDTO
{
    public class ProductResponse
    {
        public Guid ProductID { get; set; }
        [StringLength(50)]
        public string ProductName { get; set; } = string.Empty;
        [Required(ErrorMessage = "{0} should not be blank")]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        [Range(1, 1000, ErrorMessage = "Quantity should be between 1 and 1000")]
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public Guid? CategoryID { get; set; }

        public string Category { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            ProductResponse productResponse = (ProductResponse)obj;
            return productResponse.ProductID == ProductID
                && productResponse.ProductName == ProductName
                && productResponse.Description == Description
                && productResponse.Price == Price
                && productResponse.ImageUrl == ImageUrl
                && productResponse.CategoryID == CategoryID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
