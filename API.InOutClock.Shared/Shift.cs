using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.InOutClock.Shared
{
    internal class Shift
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string NormalizedDescription { get; set; }
        public DateTime In { get; set; }
        public DateTime Out { get; set; }
    }
}
