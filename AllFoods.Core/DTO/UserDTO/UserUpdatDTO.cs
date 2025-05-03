using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.DTO.UserDTO
{
    public class UserUpdatDTO
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;


        public string StreetAddress { get; set; } = string.Empty;


        public string City { get; set; } = string.Empty;


        public string State { get; set; } = string.Empty;


        public string PostalCode { get; set; } = string.Empty;
    }
}
