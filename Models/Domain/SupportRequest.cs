using Managament.Models.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Management.Models.Domain
{
    public class SupportRequest
    {
        public int CustomerId { get; set; }
        public int Id { get; set; }

        [Required(ErrorMessage = "Subject is required")]
        [StringLength(100, ErrorMessage = "Subject can't be longer than 100 characters")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [StringLength(1000, ErrorMessage = "Message can't be longer than 1000 characters")]

        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public Customer Customer { get; set; }
    }
}
