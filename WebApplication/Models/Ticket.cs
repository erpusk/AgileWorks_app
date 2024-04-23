using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models {
    public class Ticket {
        public Ticket() => SubmissionTime = DateTime.Now;
        public int Id { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Display(Name = "Submission time")]
        public DateTime SubmissionTime { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Deadline time")]
        [Required(ErrorMessage = "Deadline time is required")]
        public DateTime DeadlineTime { get; set; }
        [Display(Name = "Is resolved?")] public bool IsResolved { get; set; }
    }
}
