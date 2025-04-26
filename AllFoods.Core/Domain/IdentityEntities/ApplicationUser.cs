using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
namespace AllFoods.Core.Domain.IdentityEntities
{
    public class ApplicationUser : IdentityUser
    {

        public string Name { get; set; } = string.Empty;

        public string StreetAddress { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string PostalCode { get; set; } = string.Empty;
        [NotMapped]
        public string? Role { get; set; }
    }
}
