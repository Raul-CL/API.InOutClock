using API.InOutClock.Data;
using API.InOutClock.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace API.InOutClock.API.Controllers
{
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
                return BadRequest();
            }

            if (await _context.Departments.AnyAsync(dep => dep.NormalizedDescription == department.NormalizedDescription))
            {
                return BadRequest();
            }

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDepartment", new { id = department.Id }, department);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(Department department)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (await _context.Departments.AnyAsync(dep => dep.Id == department.Id))
            {
                _context.Departments.Update(department);
                await _context.SaveChangesAsync();
                return Ok(department);
            }
            else
            {
                return NotFound("El departamento no existe");
            }
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

            return Ok(department);
        }
  
    }
}
