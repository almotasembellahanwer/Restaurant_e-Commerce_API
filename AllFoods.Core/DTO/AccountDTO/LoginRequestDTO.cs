using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.DTO.AccountDTO
{
    public class LoginRequestDTO
    {
        [Required]
        [EmailAddress(ErrorMessage = "Please write a valid email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
