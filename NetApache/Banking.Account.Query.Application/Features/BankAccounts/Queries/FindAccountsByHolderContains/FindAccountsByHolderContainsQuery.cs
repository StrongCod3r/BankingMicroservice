using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Account.Query.Domain;
using MediatR;

namespace Banking.Account.Query.Application.Features.BankAccounts.Queries.FindAccountsByNameContains
{
    public class FindAccountsByHolderContainsQuery : IRequest<IEnumerable<BankAccount>>
    {
        public string Name { get; set; } = string.Empty;
    }
}
