using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CashSmart.Core.Models;
using CashSmart.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CashSmart.Application.Models.InvestmentManagement
{
    public class AddInvestment
    {
        public class AddInvestmentRequest : IRequest<Unit>
        {
            public Guid UserId;
            public string Name { get; set; } = string.Empty;
            public DateTime BuyDate { get; set; }
            public float BuyPrice { get; set; }
            public float Fee { get; set; }
            public float Quantity { get; set; }
        }

        public class AddInvestmentRequestHandler : IRequestHandler<AddInvestmentRequest, Unit>
        {
            private ApplicationDbContext dbContext;
            public AddInvestmentRequestHandler(ApplicationDbContext dbContext)
            {
                this.dbContext = dbContext;
            }

            public async Task<Unit> Handle(AddInvestmentRequest request, CancellationToken cancellationToken)
            {
                var userExists = await dbContext.Users.AnyAsync(s => s.Id == request.UserId);
                if (!userExists)
                {
                    throw new Exception("User does not exist!");
                }

                var investment = new Investment
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    BuyDate = request.BuyDate,
                    BuyPrice = request.BuyPrice,
                    Fee = request.Fee,
                    Quantity = request.Quantity,
                };

                return Unit.Value;
            }
        }
    }
}