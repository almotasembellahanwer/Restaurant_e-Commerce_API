using System.ComponentModel.DataAnnotations;
namespace AllFoods.Core.DTO.CategoyDTO
{
    public class CategoryResponse
    {
        public Guid CategoryID { get; set; }
        [StringLength(50)]
        public string CategoryName { get; set; } = string.Empty;

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
            CategoryResponse categoryResponse = (CategoryResponse)obj;
            return categoryResponse.CategoryID == CategoryID
                && categoryResponse.CategoryName == CategoryName;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
