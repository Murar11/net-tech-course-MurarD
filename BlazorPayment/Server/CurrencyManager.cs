using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPayment.Server
{
    public static class CurrencyManager
    {
        public static List<string> Currencies { get; }

        static CurrencyManager()
        {
            Currencies = new List<string>
                {
                  "EUR",
                  "USD",
                  "MDL",
                  "EC"
                };
        }
    }
}
