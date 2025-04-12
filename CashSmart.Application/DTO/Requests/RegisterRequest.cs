using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashSmart.Application.DTO.Requests
{
    public class RegisterRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string UserName { get; set; }
    }
}