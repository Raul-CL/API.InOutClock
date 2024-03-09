using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.InOutClock.Shared
{
    public class Check
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime Record { get; set; }        
        public int TypeOfCheck { get; set; }
        public int DepartmentId { get; set; }
        public int ShiftId { get; set; }        
    }
}
