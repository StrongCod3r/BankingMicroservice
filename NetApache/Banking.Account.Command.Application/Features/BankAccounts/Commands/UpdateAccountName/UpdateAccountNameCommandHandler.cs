using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Account.Command.Application.Aggregates;
using Banking.Account.Command.Application.Features.BankAccounts.Commands.DepositFund;
using Banking.Cqrs.Core.Handlers;
using MediatR;

namespace Banking.Account.Command.Application.Features.BankAccounts.Commands.UpdateAccountName
{
    public class UpdateAccountNameCommandHandler : IRequestHandler<UpdateAccountNameCommand, bool>
    {
        private readonly EventSourcingHandler<AccountAggregate> _eventSourcingHandler;

        public UpdateAccountNameCommandHandler(EventSourcingHandler<AccountAggregate> eventSourcingHandler)
        {
            _eventSourcingHandler = eventSourcingHandler;
        }

        public async Task<bool> Handle(UpdateAccountNameCommand request, CancellationToken cancellationToken)
        {
            var aggregate = await _eventSourcingHandler.GetById(request.Id);
            aggregate.UpdateAccountName(request.Name);
            await _eventSourcingHandler.Save(aggregate);
            return true;
        }
    }
}
