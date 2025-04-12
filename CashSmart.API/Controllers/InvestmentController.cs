using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashSmart.API.Controllers
{
    public class InvestmentController : ControllerBase
    {
        private IMediator mediator;
        public InvestmentController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Authorize]
        [HttpPost("[controller]/add")]
        public async Task<IActionResult> AddInvestment(string userEmail, string Name, DateTime buyDate, float buyPrice, float fee, float quantity)
        {
            return Ok();
        }

        [HttpGet("[controller]/get-by-user")]
        public async Task<IActionResult> ListInvestmentsByUser([FromForm] string userEmail)
        {
            return Ok();
        }
    }
}