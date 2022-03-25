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
    public class LawyersController : ControllerBase
    {
        private readonly LawyerHelperContext _context;

        public LawyersController(LawyerHelperContext context)
        {
            _context = context;
        }

        // GET: api/Lawyers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lawyers>>> GetLawyers()
        {
            return await _context.Lawyers.ToListAsync();
        }

        // GET: api/Lawyers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Lawyers>> GetLawyers(int id)
        {
            var lawyers = await _context.Lawyers.FindAsync(id);

            if (lawyers == null)
            {
                return NotFound();
            }

            return lawyers;
        }

        // PUT: api/Lawyers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLawyers(int id, Lawyers lawyers)
        {
            if (id != lawyers.Id)
            {
                return BadRequest();
            }

            _context.Entry(lawyers).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LawyersExists(id))
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

        // POST: api/Lawyers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Lawyers>> PostLawyers(Lawyers lawyers)
        {
            _context.Lawyers.Add(lawyers);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLawyers", new { id = lawyers.Id }, lawyers);
        }

        // DELETE: api/Lawyers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLawyers(int id)
        {
            var lawyers = await _context.Lawyers.FindAsync(id);
            if (lawyers == null)
            {
                return NotFound();
            }

            _context.Database.BeginTransaction();
            _context.Database.ExecuteSqlRaw($"DELETE FROM process_relation WHERE LawyerId={id}");
            _context.Lawyers.Remove(lawyers);
            _context.Database.CommitTransaction();

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LawyersExists(int id)
        {
            return _context.Lawyers.Any(e => e.Id == id);
        }
    }
}
