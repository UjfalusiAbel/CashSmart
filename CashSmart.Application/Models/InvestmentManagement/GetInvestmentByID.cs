using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using CashSmart.Application.Exceptions;
using CashSmart.Core.Models;
using CashSmart.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CashSmart.Application.Models.InvestmentManagement
{
    public class GetInvestmentByID
    {
        public class Request : IRequest<Investment>
        {
            public Guid Id { get; set; }
        }
        public class Handler : IRequestHandler<Request, Investment>
        {
            private readonly ApplicationDbContext _context;
            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<Investment> Handle(Request request, CancellationToken cancellationToken)
            {
                var investment = await _context.Investments.Where(i => i.Id == request.Id).FirstOrDefaultAsync();

                if (investment == null)
                {
                    throw new NotExistingObjectExceptions("Investment");
                }

                return investment;
            }
        }
    }
}