﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorPayment.Shared
{
  public   class Wallet
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }

    }
}
