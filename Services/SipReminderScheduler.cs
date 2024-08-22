using Managament.Models.Domain;
using Managament.Services;
using Management.Models.Domain;
using Management.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Management.Services
{
    // Service for checking and sending SIP payment reminders.
    public class SipReminderScheduler
    {
        private readonly EmailService _emailService;
        private readonly IInvestmentRepository _investmentRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<SipReminderScheduler> _logger;

        public SipReminderScheduler(
            EmailService emailService,
            IInvestmentRepository investmentRepository,
            ITransactionRepository transactionRepository,
            ILogger<SipReminderScheduler> logger)
        {
            _emailService = emailService;
            _investmentRepository = investmentRepository;
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        // Checks investments and sends reminders if payments are due.
        public async Task CheckAndSendRemindersAsync()
        {
            // Hardcoded date for testing
            var testDate = new DateTime(2024, 9, 7); // Sep 7th

            // Get active SIP investments
            var investments = await _investmentRepository.GetActiveSIPInvestmentsAsync();
            foreach (var investment in investments)
            {
                // Get the latest transaction for the investment
                var latestTransaction = await _transactionRepository.GetLatestTransactionAsync(investment.InvestmentId);
                if (latestTransaction == null) continue;

                // Calculate the next due date based on the frequency
                var nextDueDate = CalculateNextDueDate(latestTransaction.TransactionDate, investment.Frequency);

                // Calculate the difference in days between nextDueDate and testDate
                var daysUntilDue = (nextDueDate - testDate).TotalDays;

                // Log information
                _logger.LogInformation("Investment ID: {InvestmentId}", investment.InvestmentId);
                _logger.LogInformation("Latest Transaction Date: {LatestTransactionDate}", latestTransaction.TransactionDate);
                _logger.LogInformation("Next Due Date: {NextDueDate}", nextDueDate);
                _logger.LogInformation("Test Date: {TestDate}", testDate);
                _logger.LogInformation("Days Until Due: {DaysUntilDue}", daysUntilDue);

                // Send email if the due date is within the next 5 days from the test date
                if (daysUntilDue <= 5)
                {
                    _emailService.SendReminderEmail(investment.Customer.Email, investment.AmountInvested);
                }
            }
        }

        // Calculates the next due date for the SIP payment based on the last transaction date and frequency.
        private DateTime CalculateNextDueDate(DateTime lastTransactionDate, Frequency frequency)
        {
            return frequency switch
            {
                Frequency.Daily => lastTransactionDate.AddDays(1),
                Frequency.Monthly => lastTransactionDate.AddMonths(1),
                Frequency.Quarterly => lastTransactionDate.AddMonths(3),
                _ => lastTransactionDate
            };
        }
    }
}