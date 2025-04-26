using System.ComponentModel.DataAnnotations;

namespace AllFoods.Core.Domain.Entities
{
    public class Category
    {
        public Guid CategoryID { get; set; }
        [StringLength(50)]
        public string CategoryName { get; set; } = string.Empty;
    }
}
