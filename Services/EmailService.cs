using System.Globalization;
using System.Net;
using System.Net.Mail;

namespace Managament.Services
{
    public class EmailService
    {
        // Sender's email and password used for authentication with SMTP server
        private readonly string _fromEmail = "vinaysriramtummidi01@gmail.com";
        private readonly string _fromPassword = "drjm pkfi bvuy gbsi";

        public void SendReminderEmail(string email, decimal amount)
        {
            // Mail address object for sender
            var fromAddress = new MailAddress(_fromEmail, "ManagementApp");

            // Mail address object for recipient
            var toAddress = new MailAddress(email);

            // Configuring SMTP client to use Gmail's SMTP server
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, _fromPassword)
            };

            // Formatting the amount in currency format and replace the '$' with 'rupee'
            string amountFormatted = amount.ToString("C", CultureInfo.CurrentCulture).Replace('$', '₹');

            // Configuring email message
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "SIP Payment Reminder",
                Body = $"Dear Customer, your SIP payment of amount {amountFormatted} is due within 4 days. Please ensure sufficient funds in your account.",
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }
    }
}