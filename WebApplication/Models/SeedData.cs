using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using WebApplication.Data;

namespace WebApplication.Models;

public static class SeedData {
    public static void Initialize(IServiceProvider serviceProvider) {
        using (var context = new WebApplicationContext(
            serviceProvider.GetRequiredService<
                DbContextOptions<WebApplicationContext>>())) {

            if (!context.Ticket.Any(t => t.IsResolved) && context.Ticket.Any()) {
                return;
            }
            context.Ticket.AddRange(
                new Ticket {
                    Description = "Mousepad not working (deadline 1 day from submission, white ticket)",
                    DeadlineTime = DateTime.Now.AddDays(1),
                    SubmissionTime = DateTime.Now,
                    IsResolved = false
                },
                new Ticket {
                    Description = "Website is down (deadline 30 minutes from submission, red ticket)",
                    DeadlineTime = DateTime.Now.AddMinutes(30),
                    SubmissionTime = DateTime.Now,
                    IsResolved = false
                },
                new Ticket {
                    Description = "Faulty login system (deadline in the past, red ticket)",
                    DeadlineTime = DateTime.Now.AddDays(-5),
                    SubmissionTime = DateTime.Now,
                    IsResolved = false
                }
            );
            context.SaveChanges();
        }
    }
}