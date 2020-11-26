using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPayment.Server.Models
{
    public class Wallet
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
