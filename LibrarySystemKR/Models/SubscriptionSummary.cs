using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystemKR.Models
{
    public class SubscriptionSummary
    {
        public string BookTitle { get; set; }
        public string Author { get; set; }
        public string LibraryName { get; set; }
        public int SubscriptionCount { get; set; }
        public DateTime FirstIssued { get; set; }
        public DateTime LastReturned { get; set; }
    }
}
