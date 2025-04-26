using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.DTO.AccountDTO
{
    public class RegistrationRequestDTO
    {
        [Required]
        [EmailAddress(ErrorMessage = "Please write a valid email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string StreetAddress { get; set; } = string.Empty;
        [Required]

        public string City { get; set; } = string.Empty;
        [Required]

        public string State { get; set; } = string.Empty;
        [Required]

        public string PostalCode { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = string.Empty;

    }
}
