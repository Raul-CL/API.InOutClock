using API.InOutClock.Data;
using API.InOutClock.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.InOutClock.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChecksController : ControllerBase
    {
        private readonly APIInOutClockContext _context;

        public ChecksController(APIInOutClockContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Check>>> GetChecks()
        {
            return Ok(await _context.Checks.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Check>> GetCheck(int id)
        {
            var check = await _context.Checks.SingleOrDefaultAsync(che => che.Id == id);

            if (check == null)
            {
                return NotFound();
            }

            return Ok(check);
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<Check>>> GetChecksByEmployee(int employeeId)
        {
            var checks = await _context.Checks.SingleOrDefaultAsync(chk => chk.EmployeeId == employeeId);

            if (checks == null)
            {
                return NotFound();
            }

            return Ok(checks);
        }

        [HttpGet("day/{day}")]
        public async Task<ActionResult<IEnumerable<Check>>> GetChecksByDay(DayOfWeek day)
        {
            var checks = await _context.Checks.Where(chk => chk.Record.DayOfWeek == day).ToListAsync();

            if (checks == null || checks.Count == 0)
            {
                return NotFound();
            }

            return Ok(checks);
        }

        [HttpGet("department/{departmentId}")]
        public async Task<ActionResult<IEnumerable<Check>>> GetChecksByDepartment(int departmentId)
        {
            var checks = await _context.Checks.Where(chk => chk.DepartmentId == departmentId).ToListAsync();

            if (checks == null || checks.Count == 0)
            {
                return NotFound();
            }

            return Ok(checks);
        }
        
        [HttpPost]
        public async Task<ActionResult<Check>> PostCheck(Check check)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                return BadRequest(errors);
            }
            
            if (!await _context.Employees.AnyAsync(emp => emp.Id == check.EmployeeId))
            {
                return BadRequest("El empleado no existe");
            }

            if (await _context.Checks.AnyAsync(chk => chk.EmployeeId == check.EmployeeId && chk.Record.Date == check.Record.Date && chk.TypeOfCheck == check.TypeOfCheck))
            {
                return BadRequest("El empleado ya tiene una checada de este tipo en el día");
            }

            if (check.TypeOfCheck != 0 && check.TypeOfCheck != 1)
            {
                return BadRequest("El tipo de checada no es válido");
            }

            //Obtengo empleado para obtener su turno actual, es importante evaluar siempre con el turno que se tiene al momento de checar
            var employee = await _context.Employees.SingleOrDefaultAsync(emp => emp.Id == check.EmployeeId);

            //Asignamos el valor de la evaluacion de la checada, turno y departemento por usuario
            check.DepartmentId = employee.DepartmentId;
            check.ShiftId = employee.ShiftId;

            check.RecordEvaluation = await RecordEvaluation(check.TypeOfCheck, check.Record, employee.ShiftId);

            _context.Checks.Add(check);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCheck", new { id = check.Id }, check);
        }
        /*
         El metodo RecordEvaluation recibe el tipo de checada, el check en datetime por que tambien requiero la fecha y el shiftId
         en este metodo evaluo si la checada que hace el empleado es correcta o incorrecta, de esta menera poder auditar si el empleado
         esta incurriendo en faltas o llegadas tardes 
         */
        private async Task<bool> RecordEvaluation(int typeOfCheck, DateTime record, int shiftId)
        {   //Obtengo turno yconvierto la hora de la checada a TimeOnly para comparar horarios de checada
            var shift = await _context.Shifts.SingleOrDefaultAsync(s => s.Id == shiftId);
            var timeOfCheck = TimeOnly.FromDateTime(record);

            
            //ENTRADA
            if (typeOfCheck == 0)
            {//En entrada se evalua si la checada menos el tiempo de tolerancia (15 minutos) es menor o igual a la hora de entrada
                if (timeOfCheck.AddMinutes(-15) <= shift.In)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            //SALIDA
            if (typeOfCheck == 1)
            {//En salida se evalua si la checada mas el tiempo de tolerancia (15 minutos) es mayor o igual a la hora de salida
                if (timeOfCheck.AddMinutes(15) >= shift.Out)
                {
                    return true;
                }
                else
                {                    
                    return false;
                }
            }

            return false;
        }
    }
}
