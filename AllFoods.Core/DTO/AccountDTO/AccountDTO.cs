using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.DTO.AccountDTO
{
    public class AccountDTO
    {
        public AccountDTO()
        {
            ErrorMassages = new List<string>();
        }
        public string ID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> ErrorMassages { get; set; } = default!;
    }
}
