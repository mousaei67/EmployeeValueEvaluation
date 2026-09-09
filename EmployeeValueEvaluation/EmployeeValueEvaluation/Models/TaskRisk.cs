using System.ComponentModel.DataAnnotations;

namespace EmployeeValueEvaluation.Models
{
    public class TaskRisk
    {
        public int Id { get; set; }

        public int TaskItemId { get; set; }

        [Required]
        [StringLength(100)]
        public string RiskType { get; set; } = string.Empty;

        // بین صفر و یک
        public decimal Probability { get; set; }

        public decimal Impact { get; set; }

        public decimal ExpectedLoss { get; set; }

        [StringLength(2000)]
        public string? Mitigation { get; set; }

        public decimal MitigationCost { get; set; }

        public TaskItem TaskItem { get; set; } = null!;
    }
}