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
    public class ProcessController : ControllerBase
    {
        private readonly LawyerHelperContext _context;

        public ProcessController(LawyerHelperContext context)
        {
            _context = context;
        }

        // GET: api/Process
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Process>>> GetProcess()
        {
            return await _context.Process.ToListAsync();
        }

        // GET: api/Process/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Process>> GetProcess(int id)
        {
            var process = await _context.Process.FindAsync(id);

            if (process == null)
            {
                return NotFound();
            }

            process.Customers = _context.Customers
                .FromSqlRaw($"SELECT pr.ProcessId, c.* FROM process_relation pr JOIN customers c ON c.id=pr.CustomersId WHERE pr.ProcessId={id}")
                .ToList();

            process.Lawyers = _context.Lawyers
                .FromSqlRaw($"SELECT pr.ProcessId, c.* FROM process_relation pr JOIN lawyers c ON c.id=pr.LawyerId WHERE pr.ProcessId={id}")
                .ToList();

            process.Info = _context.Info
                .FromSqlRaw($"SELECT * FROM info WHERE ProcessId={id}")
                .ToList();

            process.History = _context.History
                .FromSqlRaw($"SELECT * FROM history WHERE Processid={id}")
                .ToList();

            return process;
        }

        // PUT: api/Process/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProcess(int id, Process process)
        {
            if (id != process.Id)
            {
                return BadRequest();
            }
            _context.Database.BeginTransaction();

            foreach (int customerId in process.CustomersIds ?? Enumerable.Empty<int>())
            {
                Process_Relation pr = new Process_Relation();
                pr.ProcessId = process.Id;
                pr.CustomersId = customerId;
                _context.Process_Relation.Add(pr);                
            }

            foreach (int lawyerId in process.LawyersIds ?? Enumerable.Empty<int>())
            {
                Process_Relation pr = new Process_Relation();
                pr.ProcessId = process.Id;
                pr.LawyerId = lawyerId;
                _context.Process_Relation.Add(pr);                
            }            

            _context.Database.CommitTransaction();
            _context.Entry(process).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProcessExists(id))
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

        // POST: api/Process
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Process>> PostProcess(Process process)
        {
            _context.Database.BeginTransaction();
            _context.Process.Add(process);
            await _context.SaveChangesAsync();

            foreach (int customerId in process.CustomersIds ?? Enumerable.Empty<int>())
            {
                Process_Relation pr = new Process_Relation();
                pr.ProcessId = process.Id;
                pr.CustomersId = customerId;
                _context.Process_Relation.Add(pr);                
            }

            foreach (int lawyerId in process.LawyersIds ?? Enumerable.Empty<int>())
            {
                Process_Relation pr = new Process_Relation();
                pr.ProcessId = process.Id;
                pr.LawyerId = lawyerId;
                _context.Process_Relation.Add(pr);                
            }

            _context.Database.CommitTransaction();
            await _context.SaveChangesAsync();


            return CreatedAtAction("GetProcess", new { id = process.Id }, process);
        }

        [HttpPost("{id}/history")]
        public async Task<ActionResult<Process>> PostHistory(int id, History history)
        {
            _context.History.Add(history);
            await _context.SaveChangesAsync();
            
            var process = await _context.Process.FindAsync(id);

            process.History = _context.History
                .FromSqlRaw($"SELECT * FROM history WHERE ProcessId={id}")
                .ToList();

            return process;
        }

        [HttpPost("{id}/info")]
        public async Task<ActionResult<Process>> PostInfo(int id, Info info)
        {
            _context.Info.Add(info);
            
            var process = await _context.Process.FindAsync(id);
            await _context.SaveChangesAsync();

            process.Info = _context.Info
                .FromSqlRaw($"SELECT * FROM info WHERE ProcessId={id}")
                .ToList();

            return process;
        }

        // DELETE: api/Process/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProcess(int id)
        {
            _context.Database.BeginTransaction();

            var process = await _context.Process.FindAsync(id);
            if (process == null)
            {
                return NotFound();
            }

            _context.Database
                .ExecuteSqlRaw($"DELETE FROM info WHERE ProcessId={id}");

            _context.Database
                .ExecuteSqlRaw($"DELETE FROM history WHERE ProcessId={id}");

            _context.Database
                .ExecuteSqlRaw($"DELETE FROM Process_Relation WHERE ProcessId={id}");

            _context.Process.Remove(process);
            _context.Database.CommitTransaction();

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}/info/{infoId}")]
        public async Task<IActionResult> DeleteInfo(int id, int infoId)
        {
            _context.Database.ExecuteSqlRaw($"DELETE FROM info WHERE id={infoId} AND ProcessId={id}");
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}/history/{historyId}")]
        public async Task<IActionResult> DeleteHistory(int id, int historyId)
        {
            _context.Database.ExecuteSqlRaw($"DELETE FROM history WHERE id={historyId} AND ProcessId={id}");
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}/customer/{customersId}")]
        public async Task<IActionResult> DeleteCustomerRelation(int id, int customersId)
        {
            _context.Database.ExecuteSqlRaw($"DELETE FROM process_relation WHERE ProcessId={id} AND CustomersId={customersId}");
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}/lawyer/{LawyerId}")]
        public async Task<IActionResult> DeleteLawyerRelation(int id, int lawyerId)
        {
            _context.Database.ExecuteSqlRaw($"DELETE FROM process_relation WHERE ProcessId={id} AND LawyerId={lawyerId}");
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProcessExists(int id)
        {
            return _context.Process.Any(e => e.Id == id);
        }
    }
}
