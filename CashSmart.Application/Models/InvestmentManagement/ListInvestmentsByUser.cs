using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CashSmart.Core.Models;
using CashSmart.Core.Persistence;
using MediatR;

namespace CashSmart.Application.Models.InvestmentManagement
{
    public class ListInvestmentsByUser
    {
        public class ListInvestmentsByUserRequest : IRequest<List<Investment>>
        {
            public Guid UserId { get; set; }
        }

        public class ListInvestmentsByUserRequestHandler : IRequestHandler<ListInvestmentsByUserRequest, List<Investment>>
        {
            private readonly ApplicationDbContext dbContext;
            public async Task<List<Investment>> Handle(ListInvestmentsByUserRequest request, CancellationToken cancellationToken)
            {
                var investments = 1;
                throw new NotImplementedException();
            }
        }
    }
}