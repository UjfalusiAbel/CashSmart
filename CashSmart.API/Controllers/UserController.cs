using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CashSmart.Application.DTO.Requests;
using CashSmart.Application.Exceptions;
using CashSmart.Application.Models.UserManagement;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CashSmart.API.Controllers
{
    [Route("user")]
    public class UserController : ControllerBase
    {
        private IMediator mediator;
        public UserController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromForm] RegisterRequest request, CancellationToken cancellationToken)
        {
            if (!IsValidEmail(request.Email))
            {
                return BadRequest("Invalid email address.");
            }

            try
            {
                string token = await mediator.Send(new RegisterUser.Command
                {
                    Email = request.Email,
                    Password = request.Password,
                    UserName = request.UserName
                });

                return Ok(token);
            }
            catch (UserAlreadyExistsException ue)
            {
                return Conflict(ue.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private bool IsValidEmail(string email)
        {
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!regex.IsMatch(email))
            {
                return false;
            }

            try
            {
                var domain = email.Split('@')[1];
                var mxRecords = Dns.GetHostEntry(domain).AddressList;
                return mxRecords.Length > 0;
            }
            catch
            {
                return false;
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromForm] LoginRequest request, CancellationToken cancellationToken)
        {

            try
            {
                string token = await mediator.Send(new LoginUser.Command { UserName = request.Email, Password = request.Password });
                return Ok(token);
            }
            catch (UserDoesNotExistException udne)
            {
                return NotFound(udne.Message);
            }
            catch (IncorrectCredentialsException invCred)
            {
                return Unauthorized(invCred.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}