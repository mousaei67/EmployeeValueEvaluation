using System.ComponentModel.DataAnnotations;

namespace EmployeeValueEvaluation.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? PersonnelCode { get; set; }

        [StringLength(100)]
        public string? Role { get; set; }

        public decimal BaseSalary { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? Description { get; set; }

        // روابط
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();

        public ICollection<MonthlyEvaluation> MonthlyEvaluations { get; set; }
            = new List<MonthlyEvaluation>();

        public ICollection<CorrectionPlan> CorrectionPlans { get; set; }
            = new List<CorrectionPlan>();

        public ICollection<SalaryHistory> SalaryHistories { get; set; }
            = new List<SalaryHistory>();
    }
}