using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashSmart.Core.DTO
{
    public class InvestmentDTO
    {
        public string Name { get; set; } = string.Empty;
        public DateTime BuyDate { get; set; }
        public float BuyPrice { get; set; }
        public float Fee { get; set; }
        public float Quantity { get; set; }
    }
}