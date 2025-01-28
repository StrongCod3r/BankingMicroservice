using Banking.Account.Query.Application.Features.BankAccounts.Queries.FindAccountByHolder;
using Banking.Account.Query.Application.Features.BankAccounts.Queries.FindAccountById;
using Banking.Account.Query.Application.Features.BankAccounts.Queries.FindAccountsByNameContains;
using Banking.Account.Query.Application.Features.BankAccounts.Queries.FindAccountWithBalance;
using Banking.Account.Query.Application.Features.BankAccounts.Queries.FindAllAccounts;
using Banking.Account.Query.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Banking.Account.Query.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AccountLookupController : ControllerBase
    {
        private IMediator _mediator;

        public AccountLookupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("AllAccounts", Name = "GetAllAccounts")]
        [ProducesResponseType(typeof(IEnumerable<BankAccount>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<BankAccount>>> GetAllAccounts()
        {
            return Ok(await _mediator.Send(new FindAllAccountsQuery()));        
        }

        [HttpGet("AccountByIdentifier/{id}", Name = "GetAccountByIdentifier")]
        [ProducesResponseType(typeof(IEnumerable<BankAccount>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<BankAccount>>> GetAccountByIdentifier(string id)
        {
            return Ok(await _mediator.Send(new FindAccountByIdQuery { Identifier = id }));
        }

        [HttpGet("AccountByBalance", Name = "GetAccountByBalance")]
        [ProducesResponseType(typeof(IEnumerable<BankAccount>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<BankAccount>>> GetAccountByBalance([FromBody] FindAccountWithBalanceQuery query)
        {
            return Ok(await _mediator.Send(query));
        }


        [HttpGet("AccountByAccountHolder/{name}", Name = "GetAccountByAccountHolder")]
        [ProducesResponseType(typeof(IEnumerable<BankAccount>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<BankAccount>>> GetAccountByAccountHolder(string name)
        {
            return Ok(await _mediator.Send(new FindAccountByHolderQuery { AccountHolder = name}));
        }

        [HttpGet("GetAccountsByAccountHolderContains", Name = "GetAccountsByAccountHolderContains")]
        [ProducesResponseType(typeof(IEnumerable<BankAccount>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<BankAccount>>> GetAccountsByAccountHolderContains(string name)
        {
            return Ok(await _mediator.Send(new FindAccountsByHolderContainsQuery() { Name = name }));
        }

    }
}
