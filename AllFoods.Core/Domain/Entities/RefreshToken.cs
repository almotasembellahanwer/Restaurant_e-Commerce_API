using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string UserID { get; set; } = string.Empty;
        public string JwtTokenID { get; set; } = string.Empty;
        public string Refresh_Token { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public DateTime ExpiredAt { get; set; }

    }
}
