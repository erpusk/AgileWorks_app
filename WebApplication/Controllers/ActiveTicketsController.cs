using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers {
    public class ActiveTicketsController(WebApplicationContext context) : Controller {
        private readonly WebApplicationContext _context = context;
        public async Task<IActionResult> Index() {
            var activeTickets = await _context.Ticket.Where(t => !t.IsResolved).OrderBy(t => t.DeadlineTime).ToListAsync();
            return View(activeTickets);
        }
        public async Task<IActionResult> Details(int? id) {
            if (id == null) return NotFound();
            var ticket = await _context.Ticket.FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null) return NotFound();

            return View(ticket);
        }
        public async Task<IActionResult> ResolvedTickets() {
            return View(await _context.Ticket.Where(t => t.IsResolved).ToListAsync());
        }
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,SubmissionTime,DeadlineTime,IsResolved")] Ticket ticket) {
            if (ModelState.IsValid) {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) return NotFound();
            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket == null) return NotFound();
            return View(ticket);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,SubmissionTime,DeadlineTime,IsResolved")] Ticket ticket) {
            if (id != ticket.Id) return NotFound();
            if (ModelState.IsValid) {
                try {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) {
                    if (!TicketExists(ticket.Id)) {
                        return NotFound();
                    }
                    else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) return NotFound();
            var ticket = await _context.Ticket.FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null) return NotFound();

            return View(ticket);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket != null) {
                _context.Ticket.Remove(ticket);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsResolved(int id) {
            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket != null) {
                ticket.IsResolved = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        private bool TicketExists(int id) => _context.Ticket.Any(e => e.Id == id);
    }
}
