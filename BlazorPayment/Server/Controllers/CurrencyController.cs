using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorPayment.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorPayment.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        [HttpGet]
        public CurrencyList GetCurrencies()
        {
            return new CurrencyList
            {
                Currencies = CurrencyManager.Currencies
            };
        }
    }
}
