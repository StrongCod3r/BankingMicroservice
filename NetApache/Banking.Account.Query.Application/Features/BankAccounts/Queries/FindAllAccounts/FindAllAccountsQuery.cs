using Banking.Account.Query.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Account.Query.Application.Features.BankAccounts.Queries.FindAllAccounts
{
    public class FindAllAccountsQuery : IRequest<IEnumerable<BankAccount>>
    {
    }
}
