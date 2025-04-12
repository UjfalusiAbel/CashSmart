using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CashSmart.Application.Exceptions;
using CashSmart.Application.Services;
using CashSmart.Core.Models;
using CashSmart.Core.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace CashSmart.Application.Models.UserManagement
{
    public class LoginUser
    {
        public class Command : IRequest<string>
        {
            public required string UserName { get; set; }
            public required string Password { get; set; }
        }

        public class Handler : IRequestHandler<Command, string>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMediator _mediator;
            private readonly IPasswordHasher<User> _passwordHasher;
            private readonly IConfiguration _config;
            private readonly JwtTokenService _jwtTokenService;
            public Handler(ApplicationDbContext context, IMediator mediator, IPasswordHasher<User> passwordHasher, IConfiguration config, JwtTokenService jwtTokenService)
            {
                _context = context;
                _mediator = mediator;
                _passwordHasher = passwordHasher;
                _config = config;
                _jwtTokenService = jwtTokenService;
            }
            public async Task<string> Handle(Command command, CancellationToken cancellationToken)
            {
                var ExistingUser = await _mediator.Send(new GetUserByEmail.Query { Email = command.UserName });

                if (ExistingUser == null)
                {
                    throw new UserDoesNotExistException();
                }

                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(ExistingUser, ExistingUser.PasswordHash, command.Password);

                if (passwordVerificationResult == PasswordVerificationResult.Failed)
                {
                    throw new IncorrectCredentialsException();
                }

                var token = _jwtTokenService.GenerateJwtToken(ExistingUser);
                return token;
            }
        }
    }
}