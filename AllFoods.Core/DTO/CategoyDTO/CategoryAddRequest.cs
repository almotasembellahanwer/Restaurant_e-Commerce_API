using AllFoods.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations;


namespace AllFoods.Core.DTO.CategoyDTO
{
    public class CategoryAddRequest
    {
        [StringLength(50)]
        public string CategoryName { get; set; } = string.Empty;



        public Category ToCategory()
        {
            return new Category { CategoryName = CategoryName };
        }
    }
}
