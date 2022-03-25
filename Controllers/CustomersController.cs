#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LawyerHelper.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly LawyerHelperContext _context;

        public CustomersController(LawyerHelperContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customers>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customers>> GetCustomers(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            customer.Info = _context.Info
                .FromSqlRaw($"SELECT * FROM info WHERE CustomersId={id}")
                .ToList();

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomers(int id, Customers customers)
        {
            if (id != customers.Id)
            {
                return BadRequest();
            }

            _context.Entry(customers).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomersExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customers>> PostCustomers(Customers customers)
        {
            _context.Customers.Add(customers);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomers", new { id = customers.Id }, customers);
        }

        [HttpPost("{id}/info")]
        public async Task<ActionResult<Customers>> PostInfo(int id, Info info)
        {
            _context.Info.Add(info);
            await _context.SaveChangesAsync();
            var customer = await _context.Customers.FindAsync(id);

            customer.Info = _context.Info
                .FromSqlRaw($"SELECT * FROM info WHERE CustomersId={id}")
                .ToList();

            return customer;
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomers(int id)
        {
            var customers = await _context.Customers.FindAsync(id);
            if (customers == null)
            {
                return NotFound();
            }
            
            _context.Database.BeginTransaction();
            _context.Database.ExecuteSqlRaw($"DELETE FROM info WHERE CustomersId={id}");
            _context.Database.ExecuteSqlRaw($"DELETE FROM process_relation WHERE CustomerId={id}");

            _context.Customers.Remove(customers);
            _context.Database.CommitTransaction();
            await _context.SaveChangesAsync();

            return NoContent();
        }
       
        [HttpDelete("{id}/info/{infoId}")]
        public async Task<IActionResult> DeleteInfo(int id, int infoId)
        {
            var history = await _context.History.FindAsync(infoId);
            if (history == null)
            {
                return NotFound();
            }
            
            _context.Database.ExecuteSqlRaw($"DELETE FROM info WHERE id={infoId} AND CustomersId={id}");
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomersExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
