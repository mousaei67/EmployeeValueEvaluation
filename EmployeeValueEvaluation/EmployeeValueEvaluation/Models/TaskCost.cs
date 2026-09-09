namespace EmployeeValueEvaluation.Models
{
    public class TaskCost
    {
        public int Id { get; set; }

        public int TaskItemId { get; set; }

        public decimal EmployeeHoursCost { get; set; }

        public decimal ManagerHoursCost { get; set; }

        public decimal ReworkCost { get; set; }

        public decimal DelayCost { get; set; }

        public decimal OpportunityCost { get; set; }

        public decimal RiskCost { get; set; }

        public decimal ExternalCost { get; set; }

        public decimal TotalCost { get; set; }

        public TaskItem TaskItem { get; set; } = null!;
    }
}