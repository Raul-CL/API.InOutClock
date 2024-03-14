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

            if (check.TypeOfCheck != 0 || check.TypeOfCheck != 1)
            {
                return BadRequest("El tipo de checada no es válido");
            }
            
            //Asignamos el valor de la evaluacion de la checada
            check.RecordEvaluation = await RecordEvaluation(check.TypeOfCheck, check.Record, check.ShiftId);


            _context.Checks.Add(check);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCheck", new { id = check.Id }, check);
        }

        private async Task<bool> RecordEvaluation(int typeOfCheck, DateTime record, int shiftId)
        {   /*
            Aqui evaluo la checada, primero obtengo el turno del empleado y posterior ajusto el tiempo de la checada aumentando
            15 minutos por tolerancia, para evaluar si la checada es correcta o incorrecta obtengo el tipo de checada (entrada o salida)
            y con este dato evaluo que la diferencia en la hora de entrada y la hora de checada sea igual o que la hora de checada sea menor
            y para la salida evaluo que la checada sea mayor o igual a la que tiene en su turno.

            Esto forma parte de la logica de negocio de mi proyecto, algunos valores o logica podrian cambiar dependiendo de los requerimientos.
             */

            var shift = await _context.Shifts.SingleOrDefaultAsync(s => s.Id == shiftId);
            var timeOfCheck = record.TimeOfDay.Subtract(TimeSpan.FromMinutes(15));
                

            //0 == entrada y 1 == salida
            if (typeOfCheck == 0)
            {
                if (timeOfCheck.CompareTo(shift.In) >= 0 || timeOfCheck.CompareTo(shift.In) >= -1)
                {//0 == incorrecto y 1 == correcto
                    return true;
                }
                else
                {
                    return false;
                }
            }

            //0 == entrada y 1 == salida
            if (typeOfCheck == 1)
            {
                if (timeOfCheck.CompareTo(shift.Out) >= 0 || timeOfCheck.CompareTo(shift.Out) >= 1)
                {//0 == incorrecto y 1 == correcto
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
