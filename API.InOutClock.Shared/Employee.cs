using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.InOutClock.Shared
{
    public class Employee
    {
        public int Id { get; set; }
        public string PayrollId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string NormalizedName { get; set; }
        public int ShiftId { get; set; }
        public int DepartmentId { get; set; }
        
    }
}
