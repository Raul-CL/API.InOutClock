using API.InOutClock.Data;
using API.InOutClock.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.InOutClock.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly APIInOutClockContext _context;

        public EmployeesController(APIInOutClockContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        [HttpGet("{payrollId}")]
        public async Task<ActionResult<Employee>> GetEmployeeByPayrollId(string payrollId)
        {
            var employee = await _context.Employees.SingleOrDefaultAsync(emp => emp.PayrollId == payrollId);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeeByName(string name)
        {
            var employees = await _context.Employees.Where(emp => emp.NormalizedName.Contains(name)).ToListAsync();

            if (employees == null || employees.Count == 0)
            {
                return NotFound();
            }

            return employees;
        }

        [HttpGet("department/{departmentId}")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesByDepartment(int departmentId)
        {
            var employees = await _context.Employees.Where(emp => emp.DepartmentId == departmentId).ToListAsync();
            
            if (employees == null)
            {
                   return NotFound();
            }

            return employees;
        }

        [HttpGet("shift/{shiftId}")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesByShift(int shiftId)
        {
            var employees = await _context.Employees.Where(emp => emp.ShiftId == shiftId).ToListAsync();
            
            if (employees == null)
            {
                   return NotFound();
            }

            return employees;
        }
    }
}
