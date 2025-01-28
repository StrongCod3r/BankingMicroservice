using Banking.Account.Command.Application.Features.BankAccounts.Commands.OpenAccount;
using Banking.Cqrs.Core.Domain;
using Banking.Cqrs.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Banking.Account.Command.Application.Aggregates
{
    public class AccountAggregate : AggregateRoot
    {
        public bool Active { get; set; }
        public double Balance { get; set; }
        public string Name { get; set; }

        public AccountAggregate()
        {
        }

        public AccountAggregate(OpenAccountCommand command) 
        {
            var accountOpenedEvent = new AccountOpenedEvent(
                            command.Id,
                            command.AccountHolder,
                            command.AccounType,
                            DateTime.Now,
                            command.OpeningBalance
                );

            RaiseEvent(accountOpenedEvent);
        }

        public void Apply(AccountOpenedEvent @event)
        { 
            Id = @event.Id;
            Active = true;
            Balance = @event.OpeningBalance;
        }


        public void DepositFunds(double amount)
        {
            if (!Active)
            {
                throw new Exception("Los fondos no pueden ser depositados en una cuenta cancelada");
            }

            if (amount <= 0)
            { 
                throw new Exception("El deposito de dinero debe ser mayor que cero");
            }

            var fundsDepositEvent = new FundsDepositedEvent(Id)
            {
                Id = Id,
                Amount = amount
            };

            RaiseEvent(fundsDepositEvent);
        
        }

        public void Apply(FundsDepositedEvent @event) 
        {
            Id = @event.Id;
            Balance += @event.Amount;
        }

        public void WithdrawFunds(double amount)
        {
            if (!Active)
            {
                throw new Exception("La cuenta bancaria esta cerrada");
            }

            var fundsWithDrawnEvent = new FundsWithdrawnEvent(Id)
            { 
                Id = Id,
                Amount = amount
            };

            RaiseEvent(fundsWithDrawnEvent);
        }

        public void Apply(FundsWithdrawnEvent @event)
        {
            Id = @event.Id;
            Balance -= @event.Amount;
        
        }

        public void CloseAccount()
        {
            if (!Active)
            {
                throw new Exception("La cuenta esta cerrada");
            }

            var accountClosedEvent = new AccountClosedEvent(Id);
            RaiseEvent(accountClosedEvent);
        }

        public void Apply(AccountClosedEvent @event)
        { 
            Id = @event.Id;
            Active = false;
        }

        public void UpdateAccountName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("La cuenta no puede tener un nombre vacío");
            }

            var updateAccountNameEvent = new UpdateAccountNameEvent(Id)
            {
                Id = Id,
                Name = name
            };

            RaiseEvent(updateAccountNameEvent);

        }

        public void Apply(UpdateAccountNameEvent @event)
        {
            Id = @event.Id;
            Name = @event.Name;
        }
    }
}
