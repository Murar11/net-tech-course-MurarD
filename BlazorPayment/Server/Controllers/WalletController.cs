using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorPayment.Server.Aplication.Wallets.Commands;
using BlazorPayment.Server.Aplication.Wallets.Queries;
using BlazorPayment.Server.Data;
using BlazorPayment.Server.Helper;
using BlazorPayment.Server.Models;
using BlazorPayment.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wallet = BlazorPayment.Server.Models.Wallet;


namespace BlazorPayment.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMediator mediator;

        public WalletController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMediator mediator)
        {
            this.context = context;
            this.userManager = userManager;
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<List<Wallet>> GetWallets()
        {
            var query = new GetWalletsQuery
            {
                UserId = userManager.GetUserId(User),
            };    
           var wallets = await  mediator.Send(query);
            return wallets;
        }
        [HttpGet]
        [Route("{id}")]
        public Wallet GetWallet(Guid id)
        {
            var userId = userManager.GetUserId(User);
            var wallet = context.Users.Include(x => x.Wallets).FirstOrDefault(x => x.Id == userId).
                           Wallets.FirstOrDefault(x => x.Id == id);
            return wallet;
        }
        [HttpPost]
        public async Task<IActionResult> CreateWallet([FromQuery] string currency)
        {
            var createWalletCommand = new CreateWalletCommand
            {
                UserId = userManager.GetUserId(User),
                Currency = currency

            };
            var createWalletResult = await mediator.Send(createWalletCommand);
           if(!createWalletResult.IsSuccessful)
            {
                return BadRequest();
            }

            return Ok();
        }


        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteWallet([FromRoute] Guid id)
        {
            var userId = userManager.GetUserId(User);
            var user = context.Users.Include(x => x.Wallets).FirstOrDefault(x => x.Id == userId);

            if (!user.Wallets.Any(x => x.Id == id))
            {
                return BadRequest();
            }

            var wallet = context.Wallets.Find(id);
            context.Wallets.Remove(wallet);
            context.SaveChanges();

            return Ok();
        }
        [HttpPost]
        [Route("transfer")]
        public ActionResult MakeTransfer([FromBody] TransferDto data)
        {
            var userId = userManager.GetUserId(User);
            var user = context.Users.Include(x => x.Wallets).FirstOrDefault(x => x.Id == userId);


            if (!user.Wallets.Any(x => x.Currency == data.Currency))
            {
                return BadRequest();
            }
            var source = user.Wallets.FirstOrDefault(x => x.Currency == data.Currency);

            if (source.Amount < data.Amount)
            {
                return BadRequest();
            }

            var destinationUser = context.Users.Include(x => x.Wallets).FirstOrDefault(x => x.UserName == data.Username);
            if(destinationUser == null)
            {
                throw new NotFoundException();
            }

            var destination = destinationUser.Wallets.FirstOrDefault(x => x.Currency == data.Currency);

            if (destination == null)
            {
                destination = new Wallet
                {
                    Amount = 0,
                    Currency = data.Currency
                };

                destinationUser.Wallets.Add(destination);
            }

            
            source.Amount -= data.Amount;
            destination.Amount += data.Amount;

            var transaction = new Transaction
            {
                Amount = data.Amount,
                Date = DateTime.Now,
                DestinationWalletId = destination.Id,
                SourceWalletId = source.Id
            };
            context.Add(transaction);

            context.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [Route("transfers/{itemsPerPage}/{pageNumber}")]
        public TransactionsHistoryData GetTransaction(int itemsPerPage, int pageNumber, [FromQuery] Direction direction)
        {
            var userId = userManager.GetUserId(User);

            var walletIds = context.Wallets.Where(w => w.ApplicationUserId == userId).Select(w => w.Id ).ToList();

            var query = context.Transactions.Where(t =>
             walletIds.Contains(t.DestinationWalletId) || walletIds.Contains(t.SourceWalletId));

            Transaction[] transactions;
            switch (direction)
            {
                case Direction.Inbound:
                 query = context.Transactions.Where(t => walletIds.Contains(t.SourceWalletId));
                    transactions  = query.OrderBy(x => x.Date)
                        .Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage).ToArray();
                    break;

                case Direction.Outbound:
                    query = context.Transactions.Where(t => walletIds.Contains(t.DestinationWalletId));
                    transactions = query.OrderByDescending(x => x.Date)
                        .Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage).ToArray();
                    break;
                case Direction.None:
                default:
                    query = context.Transactions.Where(t =>
                        walletIds.Contains(t.DestinationWalletId) || walletIds.Contains(t.SourceWalletId));
                    transactions = query.OrderByDescending(x => x.Date)
                        .Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage).ToArray();
                    break;

            }
            query.OrderByDescending(x=> x.Date)
                .Skip((pageNumber-1) * itemsPerPage).Take(itemsPerPage).ToArray();

            var transactionData = new TransactionsHistoryData
            {
                Transactions = transactions.Select(DomainMapper.ToDto).ToArray(),
                ItemCount = query.Count()
        };

            return transactionData;
        }
    }
}
