using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CashSmart.Application.Exceptions;
using CashSmart.Core.DTO;
using CashSmart.Core.Models;
using CashSmart.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CashSmart.Application.Models.InvestmentManagement
{
    public class ListInvestmentsByUser
    {
        public class ListInvestmentsByUserRequest : IRequest<List<InvestmentDTO>>
        {
            public Guid UserId { get; set; }
        }

        public class ListInvestmentsByUserRequestHandler : IRequestHandler<ListInvestmentsByUserRequest, List<InvestmentDTO>>
        {
            private readonly ApplicationDbContext dbContext;
            public ListInvestmentsByUserRequestHandler(ApplicationDbContext dbContext)
            {
                this.dbContext = dbContext;
            }

            public async Task<List<InvestmentDTO>> Handle(ListInvestmentsByUserRequest request, CancellationToken cancellationToken)
            {
                var userExists = await dbContext.Users.AnyAsync(s => s.Id == request.UserId);
                if (!userExists)
                {
                    throw new NotExistingObjectExceptions("User");
                }
                var investmentDTOs = new List<InvestmentDTO>();
                var investments = await dbContext.Investments.Where(x => x.UserId == request.UserId).ToListAsync();
                foreach(var investment in investments)
                {
                    investmentDTOs.Add(new InvestmentDTO{
                        Name = investment.Name,
                        BuyDate = investment.BuyDate,
                        BuyPrice = investment.BuyPrice,
                        Fee = investment.Fee,
                        Quantity = investment.Quantity
                    });
                }
                return investmentDTOs;
            }
        }
    }
}