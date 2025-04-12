using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashSmart.Application.DTO.Requests
{
    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}