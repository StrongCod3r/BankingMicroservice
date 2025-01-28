using Banking.Cqrs.Core.Events;

namespace Banking.Cqrs.Core.Domain
{
    public abstract class AggregateRoot
    {
        public string Id { get; set; } = string.Empty;

        private int version = -1;

        List<BaseEvent> changes = new List<BaseEvent>();


        public int GetVersion()
        {
            return version;
        }

        public void SetVersion(int version)
        {
            this.version = version;
        }

        public List<BaseEvent> GetUncommitedChanges()
        {
            return changes;
        }

        public void MarkChangesAsCommited()
        { 
            changes.Clear();
        }

        public void ApplyChange(BaseEvent @event, bool isNewEvent)
        {
            try
            {
                var ClaseDeEvento = @event.GetType();
                var method = GetType().GetMethod("Apply", new[] { ClaseDeEvento });
                method.Invoke(this, new object[] { @event });
            }
            catch (Exception ex)
            {

            }
            finally {
                if (isNewEvent) 
                {
                    changes.Add(@event);
                }
            }

        }

        public void RaiseEvent(BaseEvent @event)
        {
            ApplyChange(@event, true);
        }

        public void ReplayEvents(IEnumerable<BaseEvent> events)
        {
            foreach (var evt in events)
            { 
                ApplyChange(evt, false);
            }
        }


    }
}
