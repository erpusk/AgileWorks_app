using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;

namespace WebApplication.Controllers {
    public class ResolvedTicketsController(WebApplicationContext context) : Controller {
        private readonly WebApplicationContext _context = context;

        public async Task<IActionResult> Index() {
            return View(await _context.Ticket.Where(t => t.IsResolved).ToListAsync());
        }
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) return NotFound();

            var ticket = await _context.Ticket
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null) return NotFound();

            return View(ticket);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket != null) {
                _context.Ticket.Remove(ticket);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
