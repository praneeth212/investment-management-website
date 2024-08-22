using Managament.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Managament.Data
{
    public static class DbInitializer
    {
        public static void Initialize(MVCDemoDbContext context)
        {
            // Ensure the database is created
            context.Database.EnsureCreated();

            // Check if the database has any data
            if (context.MutualFunds.Any() && context.NAVs.Any())
            {
                // Database has been seeded
                return;
            }

            // Seed Mutual Funds
            SeedMutualFunds(context);
            context.SaveChanges(); // Save Mutual Funds

            // Verify Mutual Funds are Saved
            Console.WriteLine($"Mutual Funds Count: {context.MutualFunds.Count()}");

            // Seed NAVs
            SeedNAVs(context);
            context.SaveChanges(); // Save NAVs

            // Verify NAVs are Saved
            Console.WriteLine($"NAVs Count: {context.NAVs.Count()}");
        }

        private static void SeedMutualFunds(MVCDemoDbContext context)
        {
            var mutualFunds = new List<MutualFund>
            {
                new MutualFund
                {
                    Name = "Equity Growth Fund",
                    Description = "An equity mutual fund aiming for long-term capital growth through investments in equity stocks.",
                    Type = MutualFundType.Equity,
                    MinimumInvestment = 5000f,
                    ExitLoad = 0.5m,
                    Risk = RiskAssessment.High
                },
                new MutualFund
                {
                    Name = "Balanced Income Fund",
                    Description = "A hybrid mutual fund that invests in a mix of equities and fixed-income securities to balance risk and return.",
                    Type = MutualFundType.Hybrid,
                    MinimumInvestment = 2000f,
                    ExitLoad = 0.8m,
                    Risk = RiskAssessment.Moderate
                },
                new MutualFund
                {
                    Name = "Conservative Debt Fund",
                    Description = "A debt mutual fund focused on providing stable returns through investments in high-quality debt instruments.",
                    Type = MutualFundType.Debt,
                    MinimumInvestment = 1000f,
                    ExitLoad = 0.5m,
                    Risk = RiskAssessment.Low
                },
                new MutualFund
                {
                    Name = "Aggressive Equity Fund",
                    Description = "An equity mutual fund targeting aggressive growth through investments in high-risk, high-reward stocks.",
                    Type = MutualFundType.Equity,
                    MinimumInvestment = 7000f,
                    ExitLoad = 1.0m,
                    Risk = RiskAssessment.High
                },
                new MutualFund
                {
                    Name = "Moderate Growth Hybrid Fund",
                    Description = "A hybrid mutual fund that aims for moderate growth by investing in both equities and fixed-income securities.",
                    Type = MutualFundType.Hybrid,
                    MinimumInvestment = 3000f,
                    ExitLoad = 1.2m,
                    Risk = RiskAssessment.Moderate
                },
                new MutualFund
                {
                    Name = "Short-Term Debt Fund",
                    Description = "A debt mutual fund focusing on short-term debt instruments to provide liquidity and lower interest rate risk.",
                    Type = MutualFundType.Debt,
                    MinimumInvestment = 2000f,
                    ExitLoad = 0.7m,
                    Risk = RiskAssessment.Low
                },
                new MutualFund
                {
                    Name = "Balanced Growth Fund",
                    Description = "A hybrid mutual fund that seeks to provide both capital appreciation and current income by investing in a mix of equities and debt instruments.",
                    Type = MutualFundType.Hybrid,
                    MinimumInvestment = 4000f,
                    ExitLoad = 0.02m,
                    Risk = RiskAssessment.Moderate
                },
                new MutualFund
                {
                    Name = "Emerging Markets Equity Fund",
                    Description = "An equity mutual fund that invests in stocks from emerging markets to capitalize on growth opportunities in developing economies.",
                    Type = MutualFundType.Equity,
                    MinimumInvestment = 8000f,
                    ExitLoad = 0.9m,
                    Risk = RiskAssessment.High
                }
            };

            context.MutualFunds.AddRange(mutualFunds);
        }

        private static void SeedNAVs(MVCDemoDbContext context)
        {
            var mutualFunds = context.MutualFunds.ToList();

            if (!mutualFunds.Any())
            {
                throw new InvalidOperationException("No mutual funds found. Ensure MutualFunds are seeded before NAVs.");
            }

            var random = new Random();
            var navs = new List<NAV>();
            var today = DateTime.Today;
            var startDate = today.AddYears(-1); // 1 year back from today
            var endDate = today; // Up to the current date

            foreach (var fund in mutualFunds)
            {
                for (var date = startDate; date <= endDate; date = date.AddDays(random.Next(1, 7))) // Random interval between 1 and 7 days
                {
                    decimal navValue = 100m + (decimal)(random.NextDouble() * 50); // NAV between 100 and 150
                    navs.Add(new NAV
                    {
                        MutualFundId = fund.MutualFundId,
                        NAVDate = date,
                        NAVValue = navValue
                    });
                }
            }

            context.NAVs.AddRange(navs);

            // Log NAV data count for debugging
            Console.WriteLine($"Seeding NAVs: Count = {navs.Count}");
        }
    }
}
