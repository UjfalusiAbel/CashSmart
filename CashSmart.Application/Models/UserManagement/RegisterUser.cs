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

namespace CashSmart.Application.Models.UserManagement
{
    public class RegisterUser
    {
        public class Command : IRequest<string>
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
            public required string UserName { get; set; }
        }
        public class Handler : IRequestHandler<Command, string>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMediator _mediator;
            private readonly IPasswordHasher<User> _passwordHasher;
            private readonly JwtTokenService _jwtTokenService;
            public Handler(ApplicationDbContext context, IMediator mediator, IPasswordHasher<User> passwordHasher, JwtTokenService jwtTokenService)
            {
                _context = context;
                _mediator = mediator;
                _passwordHasher = passwordHasher;
                _jwtTokenService = jwtTokenService;
            }
            public async Task<string> Handle(Command command, CancellationToken cancellationToken)
            {
                var ExistingUser = await _mediator.Send(new GetUserByEmail.Query { Email = command.Email });

                if (ExistingUser != null)
                {
                    throw new UserAlreadyExistsException();
                }

                var User = new User { Email = command.Email, UserName = command.UserName };
                User.PasswordHash = _passwordHasher.HashPassword(User, command.Password);

                await _context.Users.AddAsync(User, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return _jwtTokenService.GenerateJwtToken(User);
            }
        }
    }
}