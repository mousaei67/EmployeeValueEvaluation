namespace EmployeeValueEvaluation.Models
{
    public class MonthlyEvaluation
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public decimal Salary { get; set; }

        public decimal TotalBenefit { get; set; }

        public decimal TotalCost { get; set; }

        public decimal NetValue { get; set; }

        public decimal ROI { get; set; }

        public decimal BCR { get; set; }

        public decimal SVR { get; set; }

        public decimal Compliance { get; set; }

        public decimal Quality { get; set; }

        public decimal Productivity { get; set; }

        public decimal Timeliness { get; set; }

        public decimal ProblemSolving { get; set; }

        public decimal Documentation { get; set; }

        public decimal Communication { get; set; }

        public decimal Performance { get; set; }

        public decimal RiskScore { get; set; }

        public decimal EconomicScore { get; set; }

        public decimal RiskFactor { get; set; }

        public decimal FinalValueScore { get; set; }

        public decimal FairSalary { get; set; }

        public decimal PotentialValue { get; set; }

        public decimal ValueGap { get; set; }

        public string? Decision { get; set; }

        public string? ManagerComment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Employee Employee { get; set; } = null!;
    }
}