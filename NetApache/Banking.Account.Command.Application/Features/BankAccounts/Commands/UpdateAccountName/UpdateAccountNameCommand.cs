using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Banking.Account.Command.Application.Features.BankAccounts.Commands.UpdateAccountName
{
    public class UpdateAccountNameCommand : IRequest<bool>
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; }
    }
}
