namespace EmployeeValueEvaluation.Models
{
    public class SalaryHistory
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public decimal Salary { get; set; }

        public DateTime EffectiveDate { get; set; }

        public string? Reason { get; set; }

        public Employee Employee { get; set; } = null!;
    }
}