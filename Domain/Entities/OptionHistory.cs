using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OptionHistory
    {
        public int Id { get; set; }
        public string OptionText { get; set; }

        public int SlideHistoryId { get; set; }
        public SlideHistory SlideHistory { get; set; }
    }
}
