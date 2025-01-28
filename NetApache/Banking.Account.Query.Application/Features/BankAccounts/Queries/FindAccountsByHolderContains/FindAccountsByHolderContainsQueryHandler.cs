using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Account.Query.Application.Contracts.Persistence;
using Banking.Account.Query.Application.Features.BankAccounts.Queries.FindAccountById;
using Banking.Account.Query.Application.Features.BankAccounts.Queries.FindAllAccounts;
using Banking.Account.Query.Domain;
using MediatR;

namespace Banking.Account.Query.Application.Features.BankAccounts.Queries.FindAccountsByNameContains
{
    public class FindAccountsByHolderContainsQueryHandler : IRequestHandler<FindAccountsByHolderContainsQuery, IEnumerable<BankAccount>>
    {
        private readonly IBankAccountRepository _bankAccountRepository;

        public FindAccountsByHolderContainsQueryHandler(IBankAccountRepository bankAccountRepository)
        {
            _bankAccountRepository = bankAccountRepository;
        }

        public async Task<IEnumerable<BankAccount>> Handle(FindAccountsByHolderContainsQuery request, CancellationToken cancellationToken)
        {
            return await _bankAccountRepository.FindByHolderContains(request.Name);
        }
    }
}
