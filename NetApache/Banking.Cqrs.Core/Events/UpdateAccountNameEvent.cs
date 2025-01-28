using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Cqrs.Core.Events
{
    public class UpdateAccountNameEvent : BaseEvent
    {
        public string Name { get; set; } = string.Empty;
        public UpdateAccountNameEvent(string id) : base(id)
        {
        }
    }
}
