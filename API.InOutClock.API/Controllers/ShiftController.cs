using API.InOutClock.Data;
using API.InOutClock.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.InOutClock.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private readonly APIInOutClockContext _context;

        public ShiftController(APIInOutClockContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shift>>> GetShifts()
        {
            return Ok(await _context.Shifts.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Shift>> GetShift(int id)
        {
            var shift = await _context.Shifts.SingleOrDefaultAsync(dep => dep.Id == id);

            if (shift == null)
            {
                return NotFound();
            }

            return Ok(shift);
        }

        [HttpGet("description/{description}")]
        public async Task<ActionResult<IEnumerable<Shift>>> GetShiftByDescription(string description)
        {
            var shifts = await _context.Shifts.Where(dep => dep.NormalizedDescription.Contains(description)).ToListAsync();

            if (shifts == null || shifts.Count == 0)
            {
                return NotFound();
            }

            return Ok(shifts);
        }

        [HttpPost]
        public async Task<ActionResult<Shift>> PostShift(Shift shift)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                return BadRequest(errors);
            }

            if (await _context.Shifts.AnyAsync(dep => dep.NormalizedDescription == shift.NormalizedDescription))
            {
                return BadRequest("Ya existe un turno con la misma descripción");
            }

            if(await _context.Shifts.AnyAsync(dep => dep.In == shift.In && dep.Out == shift.Out))
            {
                return BadRequest("Ya existe un turno con el mismo horario");
            }

            _context.Shifts.Add(shift);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetShift", new { id = shift.Id }, shift);
        }

        [HttpPut]
        public async Task<IActionResult> PutShift(int id, Shift shift)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                return BadRequest(errors);
            }

            if (await _context.Shifts.AnyAsync(shi => shi.Id == shift.Id))
            {
                _context.Shifts.Update(shift);
                await _context.SaveChangesAsync();
                return Ok(shift);
            }
            else
            {
                return NotFound("El turno no existe");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Shift>> DeleteShift(int id)
        {
            var shift = await _context.Shifts.SingleOrDefaultAsync(dep => dep.Id == id);

            if (shift == null)
            {
                return NotFound();
            }

            _context.Shifts.Remove(shift);
            await _context.SaveChangesAsync();

            return Ok(shift);
        }
    }
}
