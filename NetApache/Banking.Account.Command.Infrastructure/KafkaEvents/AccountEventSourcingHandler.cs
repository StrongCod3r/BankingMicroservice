using Banking.Account.Command.Application.Aggregates;
using Banking.Cqrs.Core.Domain;
using Banking.Cqrs.Core.Handlers;
using Banking.Cqrs.Core.Infrastructure;

namespace Banking.Account.Command.Infrastructure.KafkaEvents
{
    public class AccountEventSourcingHandler : EventSourcingHandler<AccountAggregate>
    {

        private readonly EventStore _eventStore;

        public AccountEventSourcingHandler(EventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<AccountAggregate> GetById(string id)
        {
            var aggregate = new AccountAggregate();
            var events = await _eventStore.GetEvents(id);
            if (events != null && events.Any()) 
            {
                aggregate.ReplayEvents(events);
                var latestVersion = events.Max(e => e.Version);
                aggregate.SetVersion(latestVersion);
            }

            return aggregate;
        }

        public async Task Save(AggregateRoot aggregate)
        {
            await _eventStore.SaveEvents(aggregate.Id, aggregate.GetUncommitedChanges(), aggregate.GetVersion());    
            aggregate.MarkChangesAsCommited();
        }
    }
}
