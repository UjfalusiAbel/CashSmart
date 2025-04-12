using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CashSmart.Core.Models
{
    public class Investment
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime BuyDate { get; set; }
        public float BuyPrice { get; set; }
        public float Fee { get; set; }
        public float Quantity { get; set; }
        public User User { get; set; } = null!;
    }
}