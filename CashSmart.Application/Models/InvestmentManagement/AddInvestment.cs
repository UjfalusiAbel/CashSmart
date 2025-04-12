using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

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
            public async Task<Unit> Handle(AddInvestmentRequest request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}