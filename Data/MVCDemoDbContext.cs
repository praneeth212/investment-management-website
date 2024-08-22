using Managament.Models.Domain;
using Management.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System;

namespace Managament.Data
{
    public class MVCDemoDbContext : DbContext
    {
        public MVCDemoDbContext(DbContextOptions<MVCDemoDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<SupportRequest> SupportRequests { get; set; }
        public DbSet<MutualFund> MutualFunds { get; set; }
        public DbSet<NAV> NAVs { get; set; }
        public DbSet<Investment> Investments { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure conversions for enums
            modelBuilder.Entity<MutualFund>()
                .Property(mf => mf.Type)
                .HasConversion(
                    v => v.ToString(),
                    v => (MutualFundType)Enum.Parse(typeof(MutualFundType), v));

            modelBuilder.Entity<Investment>()
                .Property(i => i.InvestmentType)
                .HasConversion(
                    v => v.ToString(),
                    v => (InvestmentType)Enum.Parse(typeof(InvestmentType), v));

            modelBuilder.Entity<Investment>()
                .Property(i => i.Frequency)
                .HasConversion(
                    v => v.ToString(),
                    v => (Frequency)Enum.Parse(typeof(Frequency), v));

            // Configure relationships
            modelBuilder.Entity<Investment>()
                .HasOne(i => i.Customer)
                .WithMany(c => c.Investments)
                .HasForeignKey(i => i.CustomerId);

            modelBuilder.Entity<Investment>()
                .HasOne(i => i.MutualFund)
                .WithMany(mf => mf.Investments)
                .HasForeignKey(i => i.MutualFundId);

            modelBuilder.Entity<NAV>()
                .HasOne(n => n.MutualFund)
                .WithMany(mf => mf.NAVs)
                .HasForeignKey(n => n.MutualFundId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Investment)
                .WithMany(i => i.Transactions)
                .HasForeignKey(t => t.InvestmentId);

            // Configure decimal columns with precision and scale
            modelBuilder.Entity<Investment>()
                .Property(i => i.AmountInvested)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Investment>()
                .Property(i => i.UnitsOwned)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<NAV>()
                .Property(n => n.NAVValue)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Units)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<MutualFund>()
                .Property(mf => mf.ExitLoad)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Investment>()
                .Property(i => i.AmountRedeemed)
                .HasColumnType("decimal(18, 2)");
        }
    }
}
