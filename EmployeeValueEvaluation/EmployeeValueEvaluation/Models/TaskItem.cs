using System.ComponentModel.DataAnnotations;

namespace EmployeeValueEvaluation.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? DueDate { get; set; }

        public DateTime? CompletedAt { get; set; }

        // 1=Low, 2=Medium, 3=High, 4=Critical
        public int Priority { get; set; } = 2;

        // 1 تا 5
        public int BusinessImpact { get; set; } = 3;

        public decimal EstimatedHours { get; set; }

        public decimal ActualHours { get; set; }

        public string? RequiredPath { get; set; }

        public string? ActualPath { get; set; }

        public bool PathApprovalRequired { get; set; }

        public bool AlternativePathApproved { get; set; }

        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        // روابط
        public Employee Employee { get; set; } = null!;

        public TaskInstruction? Instruction { get; set; }

        public TaskEvaluation? Evaluation { get; set; }

        public TaskCost? Cost { get; set; }

        public TaskBenefit? Benefit { get; set; }

        public ICollection<TaskRisk> Risks { get; set; } = new List<TaskRisk>();
    }

    public enum TaskStatus
    {
        Pending = 1,
        InProgress = 2,
        Completed = 3,
        Cancelled = 4,
        OnHold = 5
    }
}