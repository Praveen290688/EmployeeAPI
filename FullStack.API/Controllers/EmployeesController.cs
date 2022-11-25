using FullStack.API.Data;
using FullStack.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FullStack.API.Controllers
{
    [ApiController]
    
    [Route("api/[controller]")]
  
    //[Route("[controller]")]
    public class EmployeesController : Controller
    {
        private readonly FullStackDbContext _fullStackDbContext;
        public EmployeesController(FullStackDbContext fullStackDbContext)
        {
            _fullStackDbContext = fullStackDbContext;
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult>GetAllEmployees()
        {
            var employees = await _fullStackDbContext.Employees.OrderByDescending(d => d.CreatedDate).ToListAsync();

            return Ok(employees);
        }

        [HttpPost]
        public async Task<IActionResult>AddEmployee([FromBody]Employee employeeRequest)
        {
            employeeRequest.Id = Guid.NewGuid();
            await _fullStackDbContext.Employees.AddAsync(employeeRequest);
            await _fullStackDbContext.SaveChangesAsync();
            return Ok(employeeRequest);
        }


        [HttpGet]
        [Route("{id:Guid}")]

        public async Task<IActionResult> GetEmployee([FromRoute] Guid id)
        {
            var employee = await _fullStackDbContext.Employees.FirstOrDefaultAsync(x => x.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }


        [HttpPut]
        [Route("{id:Guid}")]

        public async Task<IActionResult>UpdateEmployee([FromRoute] Guid id, Employee updateEmployeeRequest)
        {
            var employee = await _fullStackDbContext.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            employee.FirstName = updateEmployeeRequest.FirstName;
            employee.LastName = updateEmployeeRequest.LastName;
            employee.Email = updateEmployeeRequest.Email;
            employee.PhoneNumber = updateEmployeeRequest.PhoneNumber;
            employee.Address = updateEmployeeRequest.Address;
            employee.City = updateEmployeeRequest.City;
            employee.State = updateEmployeeRequest.State;
            employee.Country = updateEmployeeRequest.Country;
            employee.PostalCode = updateEmployeeRequest.PostalCode;
            employee.Password = updateEmployeeRequest.Password;

            await _fullStackDbContext.SaveChangesAsync();

            return Ok(employee);
        }


        [HttpDelete]
        [Route("{id:Guid}")]

        public async Task<IActionResult> DeleteEmployee([FromRoute] Guid id)
        {
            var employee = await _fullStackDbContext.Employees.FindAsync(id);

            if(employee == null)
            {
                return NotFound();
            }

            _fullStackDbContext.Employees.Remove(employee);
            await _fullStackDbContext.SaveChangesAsync();
            return Ok(employee);
        }





    }
}
