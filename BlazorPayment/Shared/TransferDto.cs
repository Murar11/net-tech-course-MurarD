using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorPayment.Shared
{
   public  class TransferDto
    {
        public string Username { get; set; }
        public decimal  Amount { get; set; }
        public string Currency { get; set; }
    }
}
