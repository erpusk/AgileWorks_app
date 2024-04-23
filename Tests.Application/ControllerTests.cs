using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using WebApplication.Controllers;
using WebApplication.Data;
using WebApplication.Models;

namespace Tests.Application {
    [TestClass]
    public class ControllerTests {

        private ActiveTicketsController? _controller;
        private WebApplicationContext? _context;
        private List<Ticket>? tickets;
        [TestInitialize] public void Initialize() {
            var options = new DbContextOptionsBuilder<WebApplicationContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new WebApplicationContext(options);
            _context.Ticket.RemoveRange(_context.Ticket);
            _context.SaveChanges();

            if (tickets == null) {
                tickets = new List<Ticket> {
                    new() { Id = 1, Description = "Test ticket 1", SubmissionTime = DateTime.Now, DeadlineTime = DateTime.Now.AddDays(7), IsResolved = false },
                    new() { Id = 2, Description = "Test ticket 2", SubmissionTime = DateTime.Now, DeadlineTime = DateTime.Now.AddDays(5), IsResolved = false }
                };
                _context.AddRange(tickets);
                _context.SaveChanges();
            }

            _controller = new ActiveTicketsController(_context);
            }
        [TestMethod] public async Task ActiveTicketsController_IndexView_Test() {
            var result = await _controller.Index() as ViewResult;
            var model = result?.Model as List<Ticket>;

            Assert.IsNotNull(result);
            Assert.AreEqual(tickets.Count, model?.Count);
            Assert.AreNotEqual(tickets[0].SubmissionTime, tickets[1].SubmissionTime); //two tickets can't have the same submission time
        }
        [TestMethod] public async Task ActiveTicketsController_DetailsView_Test() {
            var ticketId = tickets[0].Id;

            var result = await _controller.Details(ticketId) as ViewResult;
            var model = result?.Model as Ticket;

            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.AreEqual(ticketId, model.Id);
            Assert.AreEqual("Test ticket 1", model.Description);
        }
        [TestMethod] public async Task ActiveTicketsController_Delete_Test() {
            var result = await _controller.DeleteConfirmed(tickets[0].Id) as RedirectToActionResult;

            var deletedTicket = await _context.Ticket.FindAsync(tickets[0].Id);
            Assert.IsNull(deletedTicket);
            Assert.AreEqual("Index", result?.ActionName);
        }
        [TestMethod] public async Task MarkAsResolved_RemovesFromActive_AddsToResolved_Test() {
            _ = await _controller.MarkAsResolved(tickets[0].Id);

            var activeTickets = await _context.Ticket.Where(t => !t.IsResolved).ToListAsync();
            var resolvedTickets = await _context.Ticket.Where(t => t.IsResolved).ToListAsync();

            Assert.IsFalse(activeTickets.Any(t => t.Id == tickets[0].Id));
            Assert.IsTrue(resolvedTickets.Any(t => t.Id == tickets[0].Id));
        }
    }
}