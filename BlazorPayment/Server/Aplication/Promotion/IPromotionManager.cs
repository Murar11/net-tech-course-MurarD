using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPayment.Server.Aplication.Promotion
{
    public interface IPromotionManager
    {
        decimal GetDefaultAmount(string currency);
    }
}
