using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CashSmart.Application.Exceptions;
using CashSmart.Application.Models.InvestmentManagement;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashSmart.API.Controllers
{
    [Route("investments")]
    public class InvestmentController : ControllerBase
    {
        private IMediator mediator;
        public InvestmentController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddInvestment([FromForm] string Name, [FromForm] DateTime buyDate, [FromForm] float buyPrice, [FromForm] float fee, [FromForm] float quantity, CancellationToken cancellationToken)
        {
            try
            {
                var identifierClaim = User.FindFirst("Identifier")?.Value;
                if (string.IsNullOrEmpty(identifierClaim) || !Guid.TryParse(identifierClaim, out var userId))
                {
                    return Unauthorized("User identifier not found or invalid.");
                }


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

        [HttpGet("user/{id}/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListInvestmentsByUser(Guid UserId)
        {

            try
            {
                var investments = await mediator.Send(new ListInvestmentsByUser.ListInvestmentsByUserRequest { UserId = UserId });
                return Ok(investments);
            }
            catch (NotExistingObjectExceptions ne)
            {
                return NotFound(ne.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListInvestmentsByUser()
        {
            Guid UserId;
            if (!Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out UserId))
            {
                return Unauthorized("User identifier not found.");
            }

            try
            {
                var investments = await mediator.Send(new ListInvestmentsByUser.ListInvestmentsByUserRequest { UserId = UserId });
                return Ok(investments);
            }
            catch (NotExistingObjectExceptions ne)
            {
                return NotFound(ne.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetInvestment(Guid id)
        {
            try
            {
                Guid UserId;
                if (!Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "Identifier")?.Value, out UserId))
                {
                    return Unauthorized("User identifier not found.");
                }

                var investment = await mediator.Send(new GetInvestmentByID.Request { Id = id });

                if (investment.UserId != UserId)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "You don't have access to this investment");
                }
                return Ok(investment);
            }
            catch (NotExistingObjectExceptions ne)
            {
                return NotFound(ne.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}