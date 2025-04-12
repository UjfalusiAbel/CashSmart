using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CashSmart.Core.Models;
using CashSmart.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CashSmart.Application.Models.UserManagement
{
    public class GetUserByEmail
    {
        public class Query : IRequest<User>
        {
            public required string Email { get; set; }
        }

        public class Handler : IRequestHandler<Query, User?>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<User?> Handle(Query query, CancellationToken cancellationToken)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == query.Email, cancellationToken);

                if (user == null)
                {
                    return null;
                }

                return user;
            }
        }
    }
}