namespace EmployeeValueEvaluation.Models
{
    public class TaskBenefit
    {
        public int Id { get; set; }

        public int TaskItemId { get; set; }

        public decimal RevenueBenefit { get; set; }

        public decimal SavingBenefit { get; set; }

        public decimal LossAvoidance { get; set; }

        public decimal ProductBenefit { get; set; }

        public decimal StrategicBenefit { get; set; }

        // بین صفر و یک
        public decimal Confidence { get; set; } = 1;

        public decimal AdjustedBenefit { get; set; }

        public decimal TotalBenefit { get; set; }

        public TaskItem TaskItem { get; set; } = null!;
    }
}