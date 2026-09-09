namespace EmployeeValueEvaluation.Models
{
    public class TaskEvaluation
    {
        public int Id { get; set; }

        public int TaskItemId { get; set; }

        public decimal ComplianceScore { get; set; }

        public decimal QualityScore { get; set; }

        public decimal EfficiencyScore { get; set; }

        public decimal TimelinessScore { get; set; }

        public decimal ProblemSolvingScore { get; set; }

        public decimal DocumentationScore { get; set; }

        public decimal CommunicationScore { get; set; }

        public decimal UnauthorizedDeviationScore { get; set; }

        public decimal TaskScore { get; set; }

        public int BugCount { get; set; }

        public int CriticalBugCount { get; set; }

        public decimal ReworkHours { get; set; }

        public string? EvaluatorComment { get; set; }

        public DateTime EvaluatedAt { get; set; } = DateTime.Now;

        public TaskItem TaskItem { get; set; } = null!;
    }
}