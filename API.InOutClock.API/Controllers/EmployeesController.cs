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
            return Ok(await _context.Employees.ToListAsync());
        }

        [HttpGet("payroll/{payrollId}")]
        public async Task<ActionResult<Employee>> GetEmployeeByPayrollId(string payrollId)
        {
            var employee = await _context.Employees.SingleOrDefaultAsync(emp => emp.PayrollId == payrollId);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeeByName(string name)
        {
            var employees = await _context.Employees.Where(emp => emp.NormalizedName.Contains(name)).ToListAsync();

            if (employees == null || employees.Count == 0)
            {
                return NotFound();
            }

            return Ok(employees);
        }

        [HttpGet("department/{departmentId}")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesByDepartment(int departmentId)
        {
            var employees = await _context.Employees.Where(emp => emp.DepartmentId == departmentId).ToListAsync();
            
            if (employees == null)
            {
                   return NotFound();
            }

            return Ok(employees);
        }

        [HttpGet("shift/{shiftId}")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesByShift(int shiftId)
        {
            var employees = await _context.Employees.Where(emp => emp.ShiftId == shiftId).ToListAsync();
            
            if (employees == null)
            {
                   return NotFound();
            }

            return Ok(employees);
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {   //Es importante que el PayrollId sea único
            if (await _context.Employees.AnyAsync(emp => emp.PayrollId == employee.PayrollId))
            {
                return BadRequest("El payrollId ya existe");
            }
            //El turno y el departamento deben existir en la base de datos
            if (!await _context.Departments.AnyAsync(dep => dep.Id == employee.DepartmentId))
            {
                return BadRequest("El departamento no existe");
            }

            if (!await _context.Shifts.AnyAsync(shift => shift.Id == employee.ShiftId))
            {
                return BadRequest("El turno no existe");
            }

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            //Retorna 201 Created, con la ruta del recurso creado
            return CreatedAtAction("GetEmployeeByPayrollId", new { payrollId = employee.PayrollId }, employee);
        }

        [HttpPut]
        public async Task<ActionResult<Employee>> PutEmployee(Employee employee)
        {   //Al actualizar el empleado debe de existir, el nombre debe de ser único tambien dep y turno deben existir

            if (!await _context.Employees.AnyAsync(emp => emp.PayrollId == employee.PayrollId))
            {
                return NotFound("El empleado no existe");
            }

            if (await _context.Employees.AnyAsync(emp => emp.NormalizedName == employee.NormalizedName))
            {
                return NotFound("El nombre del empleado ya existe");
            }

            if (!await _context.Departments.AnyAsync(dep => dep.Id == employee.DepartmentId))
            {
                return NotFound("El departamento no existe");
            }

            if (!await _context.Shifts.AnyAsync(shift => shift.Id == employee.ShiftId))
            {
                return NotFound("El turno no existe");
            }

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{payrollId}")]
        public async Task<ActionResult<Employee>> PatchShiftEmployee(string payrollId, int shiftId, int departmentId)
        {                                
            var employee = await _context.Employees.SingleOrDefaultAsync(emp => emp.PayrollId == payrollId);

            if (employee == null)
            {
                   return NotFound("El empleado no existe");
            }
            
            if (!await _context.Departments.AnyAsync(dep => dep.Id == departmentId))
            {
                return NotFound("El departamento no existe");
            }

            if (!await _context.Shifts.AnyAsync(shift => shift.Id == shiftId))
            {
                return NotFound("El turno no existe");
            }
            
            employee.ShiftId = shiftId;
            employee.DepartmentId = departmentId;
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{payrollId}")]
        public async Task<ActionResult<Employee>> DeleteEmployee(string payrollId)
        {
            var employee = await _context.Employees.SingleOrDefaultAsync(emp => emp.PayrollId == payrollId);

            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
