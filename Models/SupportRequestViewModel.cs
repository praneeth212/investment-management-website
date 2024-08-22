using System.ComponentModel.DataAnnotations;

namespace Managament.Models
{
    public class SupportRequestViewModel
     {
        public int CustomerId { get; set; }
        [Required(ErrorMessage = "Subject is required")]
        [StringLength(100, ErrorMessage = "Subject can't be longer than 100 characters")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [StringLength(1000, ErrorMessage = "Message can't be longer than 1000 characters")]
        public string Message { get; set; }
    }
}
