using System.ComponentModel.DataAnnotations;

namespace Managament.Models.Domain
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string Address { get; set; }
        public string Pan_no { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }

        public ICollection<Investment> Investments { get; set; } = new List<Investment>();

    }
}
