namespace EmployeeValueEvaluation.Models
{
    public class CorrectionPlan
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string Problem { get; set; } = string.Empty;

        public decimal CurrentScore { get; set; }

        public decimal TargetScore { get; set; }

        public decimal CorrectionCost { get; set; }

        public decimal ExpectedBenefit { get; set; }

        public decimal Probability { get; set; }

        public decimal AdjustedBenefit { get; set; }

        public decimal CorrectionROI { get; set; }

        public DateTime? Deadline { get; set; }

        public string? Result { get; set; }

        public Employee Employee { get; set; } = null!;
    }
}