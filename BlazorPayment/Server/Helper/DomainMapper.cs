using BlazorPayment.Server.Models;
using BlazorPayment.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPayment.Server.Helper
{
    public static class DomainMapper
    {
        public static TransactionDto ToDto(Transaction transaction)
        {
            return transaction == null ? null : new TransactionDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                DestinationWalletId = transaction.DestinationWalletId,
                SourceWalletId = transaction.SourceWalletId,
                Date = transaction.Date
            };
        }
    }
}
