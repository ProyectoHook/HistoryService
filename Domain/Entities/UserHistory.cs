using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserHistory
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<SessionHistory> SessionHistories { get; set; }
    }
}
