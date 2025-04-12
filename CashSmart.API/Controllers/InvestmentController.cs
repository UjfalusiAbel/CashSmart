using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CashSmart.Application.Models.InvestmentManagement;
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
        public async Task<IActionResult> AddInvestment(Guid userId, string Name, DateTime buyDate, float buyPrice, float fee, float quantity, CancellationToken cancellationToken)
        {
            try
            {
                await mediator.Send(new AddInvestment.AddInvestmentRequest
                {
                    UserId = userId,
                    Name = Name,
                    BuyDate = buyDate,
                    BuyPrice = buyPrice,
                    Fee = fee,
                    Quantity = quantity
                }, cancellationToken);

                return Ok("Investment added successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("[controller]/get-by-user")]
        public async Task<IActionResult> ListInvestmentsByUser([FromForm] Guid UserId)
        {
            try
            {
                var investments = await mediator.Send(new ListInvestmentsByUser.ListInvestmentsByUserRequest { UserId = UserId });
                return Ok(investments);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}