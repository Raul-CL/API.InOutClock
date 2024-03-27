using API.InOutClock.Data;
using API.InOutClock.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace API.InOutClock.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly APIInOutClockContext _context;

        public DepartmentsController(APIInOutClockContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            return Ok(await _context.Departments.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {   
            var department = await _context.Departments.SingleOrDefaultAsync(dep => dep.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            return Ok(department);
        }

        [HttpGet("description/{description}")]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartmentByDescription(string description)
        {
            var departments = await _context.Departments.Where(dep => dep.NormalizedDescription.Contains(description)).ToListAsync();

            if (departments == null || departments.Count == 0)
            {
                return NotFound();
            }

            return Ok(departments);
        }

        [HttpPost]
        public async Task<ActionResult<Department>> PostDepartment(Department department)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));

                return BadRequest(errors);
            }

            if (await _context.Departments.AnyAsync(dep => dep.NormalizedDescription == department.NormalizedDescription))
            {
                return BadRequest("La descripción ya existe");
            }            

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDepartment", new { id = department.Id }, department);
        }

        [HttpPut]
        public async Task<IActionResult> PutDepartment(Department department)
        {
            if (!ModelState.IsValid)
            {                
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                
                return BadRequest(errors);
            }

            if(!await _context.Departments.AnyAsync(dep => dep.Id == department.Id))
            {
                return NotFound("El departamento no existe");
            }

            if (await _context.Departments.AnyAsync(dep => dep.NormalizedDescription == department.NormalizedDescription))
            {
                return BadRequest("La descripción ya existe");
            }
            
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
            return Ok(department);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<Department>> DeleteDepartment(int id)
        {
            var department = await _context.Departments.SingleOrDefaultAsync(dep => dep.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return NoContent();
        }
  
    }
}
