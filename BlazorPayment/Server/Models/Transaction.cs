using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPayment.Server.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid SourceWalletId { get; set; }
        public Guid DestinationWalletId { get; set; }
        public decimal  Amount { get; set; }
        public DateTime Date { get; set; }

        

    }
}
