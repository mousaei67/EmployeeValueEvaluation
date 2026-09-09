using EmployeeValueEvaluation.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeValueEvaluation.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<TaskItem> Tasks { get; set; }

        public DbSet<TaskInstruction> TaskInstructions { get; set; }

        public DbSet<TaskEvaluation> TaskEvaluations { get; set; }

        public DbSet<TaskCost> TaskCosts { get; set; }

        public DbSet<TaskBenefit> TaskBenefits { get; set; }

        public DbSet<TaskRisk> TaskRisks { get; set; }

        public DbSet<MonthlyEvaluation> MonthlyEvaluations { get; set; }

        public DbSet<CorrectionPlan> CorrectionPlans { get; set; }

        public DbSet<SalaryHistory> SalaryHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Employee
            modelBuilder.Entity<Employee>()
                .Property(x => x.BaseSalary)
                .HasPrecision(18, 2);

            // Task
            modelBuilder.Entity<TaskItem>()
                .Property(x => x.EstimatedHours)
                .HasPrecision(18, 2);

            modelBuilder.Entity<TaskItem>()
                .Property(x => x.ActualHours)
                .HasPrecision(18, 2);

            // Task Evaluation
            modelBuilder.Entity<TaskEvaluation>()
                .HasOne(x => x.TaskItem)
                .WithOne(x => x.Evaluation)
                .HasForeignKey<TaskEvaluation>(x => x.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Task Instruction
            modelBuilder.Entity<TaskInstruction>()
                .HasOne(x => x.TaskItem)
                .WithOne(x => x.Instruction)
                .HasForeignKey<TaskInstruction>(x => x.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Task Cost
            modelBuilder.Entity<TaskCost>()
                .HasOne(x => x.TaskItem)
                .WithOne(x => x.Cost)
                .HasForeignKey<TaskCost>(x => x.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Task Benefit
            modelBuilder.Entity<TaskBenefit>()
                .HasOne(x => x.TaskItem)
                .WithOne(x => x.Benefit)
                .HasForeignKey<TaskBenefit>(x => x.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Task Risk
            modelBuilder.Entity<TaskRisk>()
                .HasOne(x => x.TaskItem)
                .WithMany(x => x.Risks)
                .HasForeignKey(x => x.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Employee -> Tasks
            modelBuilder.Entity<TaskItem>()
                .HasOne(x => x.Employee)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee -> Monthly Evaluations
            modelBuilder.Entity<MonthlyEvaluation>()
                .HasOne(x => x.Employee)
                .WithMany(x => x.MonthlyEvaluations)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Employee -> Correction Plans
            modelBuilder.Entity<CorrectionPlan>()
                .HasOne(x => x.Employee)
                .WithMany(x => x.CorrectionPlans)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Employee -> Salary History
            modelBuilder.Entity<SalaryHistory>()
                .HasOne(x => x.Employee)
                .WithMany(x => x.SalaryHistories)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Decimal precision
            modelBuilder.Entity<TaskEvaluation>().Property(x => x.ComplianceScore).HasPrecision(5, 2);
            modelBuilder.Entity<TaskEvaluation>().Property(x => x.QualityScore).HasPrecision(5, 2);
            modelBuilder.Entity<TaskEvaluation>().Property(x => x.EfficiencyScore).HasPrecision(5, 2);
            modelBuilder.Entity<TaskEvaluation>().Property(x => x.TimelinessScore).HasPrecision(5, 2);
            modelBuilder.Entity<TaskEvaluation>().Property(x => x.ProblemSolvingScore).HasPrecision(5, 2);
            modelBuilder.Entity<TaskEvaluation>().Property(x => x.DocumentationScore).HasPrecision(5, 2);
            modelBuilder.Entity<TaskEvaluation>().Property(x => x.CommunicationScore).HasPrecision(5, 2);
            modelBuilder.Entity<TaskEvaluation>().Property(x => x.UnauthorizedDeviationScore).HasPrecision(5, 2);
            modelBuilder.Entity<TaskEvaluation>().Property(x => x.TaskScore).HasPrecision(5, 2);
            modelBuilder.Entity<TaskEvaluation>().Property(x => x.ReworkHours).HasPrecision(18, 2);

            modelBuilder.Entity<TaskBenefit>().Property(x => x.Confidence).HasPrecision(5, 4);

            modelBuilder.Entity<TaskRisk>().Property(x => x.Probability).HasPrecision(5, 4);

            // Unique constraints
            modelBuilder.Entity<Employee>()
                .HasIndex(x => x.PersonnelCode)
                .IsUnique()
                .HasFilter("[PersonnelCode] IS NOT NULL");

            modelBuilder.Entity<MonthlyEvaluation>()
                .HasIndex(x => new { x.EmployeeId, x.Year, x.Month })
                .IsUnique();
        }
    }
}