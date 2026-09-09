using EmployeeValueEvaluation.Models;

namespace EmployeeValueEvaluation.Services.Dtos
{
    /// <summary>شدت یک یافته/مشکل.</summary>
    public enum Severity
    {
        Ok = 0,
        Info = 1,
        Warning = 2,
        Critical = 3
    }

    /// <summary>سطح تصمیم مدیریتی (بند ۷۸).</summary>
    public enum DecisionLevel
    {
        A_Valuable = 0,          // ارزشمند
        B_ConditionallyValuable, // ارزشمند مشروط
        C_Risky,                 // پرریسک
        D_Uneconomic             // غیراقتصادی
    }

    /// <summary>یک یافته: کجا مشکل هست و چه باید کرد.</summary>
    public class Finding
    {
        public string Metric { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public Severity Severity { get; set; }
        public string Problem { get; set; } = string.Empty;
        public string Recommendation { get; set; } = string.Empty;
    }

    /// <summary>نتیجه محاسبه یک Task.</summary>
    public class TaskComputationResult
    {
        public int TaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Priority { get; set; }
        public int BusinessImpact { get; set; }
        public int TaskWeight { get; set; }

        public decimal Compliance { get; set; }
        public decimal Quality { get; set; }
        public decimal Efficiency { get; set; }
        public decimal Timeliness { get; set; }
        public decimal ProblemSolving { get; set; }
        public decimal Documentation { get; set; }
        public decimal Communication { get; set; }
        public decimal TaskScore { get; set; }

        public decimal TotalCost { get; set; }
        public decimal TotalBenefit { get; set; }
        public decimal NetValue => TotalBenefit - TotalCost;

        public decimal ReworkHours { get; set; }
        public int BugCount { get; set; }
        public int CriticalBugCount { get; set; }
        public bool UnauthorizedDeviation { get; set; }
        public string? EvaluatorComment { get; set; }
    }

    /// <summary>نتیجه کامل ارزیابی یک نفر (خروجی اصلی مدل).</summary>
    public class EmployeeEvaluationResult
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Role { get; set; }
        public string? PersonnelCode { get; set; }
        public decimal Salary { get; set; }

        public int TaskCount { get; set; }
        public bool HasData => TaskCount > 0;

        // اقتصاد (مرحله ۱ تا ۴)
        public decimal TotalBenefit { get; set; }
        public decimal TotalCost { get; set; }
        public decimal NetValue { get; set; }
        public decimal Roi { get; set; }
        public decimal Bcr { get; set; }
        public decimal Svr { get; set; }

        // اجزای هزینه
        public decimal CostSalary { get; set; }
        public decimal CostSideBenefits { get; set; }
        public decimal CostManagement { get; set; }
        public decimal CostRework { get; set; }
        public decimal CostDelay { get; set; }
        public decimal CostOpportunity { get; set; }
        public decimal CostRisk { get; set; }
        public decimal CostExternal { get; set; }

        // عملکرد (مرحله ۵)
        public decimal Compliance { get; set; }
        public decimal Quality { get; set; }
        public decimal Productivity { get; set; }
        public decimal Timeliness { get; set; }
        public decimal ProblemSolving { get; set; }
        public decimal Documentation { get; set; }
        public decimal Communication { get; set; }
        public decimal Performance { get; set; }

        // شاخص‌های کمکی
        public decimal ReworkRatePercent { get; set; }
        public decimal UnauthorizedDeviationRatePercent { get; set; }
        public int UnauthorizedDeviationCount { get; set; }
        public int TotalBugs { get; set; }
        public int CriticalBugs { get; set; }

        // ریسک (مرحله ۷)
        public decimal DeviationRisk { get; set; }
        public decimal TechnicalRisk { get; set; }
        public decimal DependencyRisk { get; set; }
        public decimal ContinuityRisk { get; set; }
        public decimal SecurityRisk { get; set; }
        public decimal RiskScore { get; set; }
        public decimal RiskFactor { get; set; }

        // ارزندگی (مرحله ۶ و ۸)
        public decimal EconomicScore { get; set; }
        public decimal FinalValueScore { get; set; }
        public decimal PotentialValue { get; set; }
        public decimal ValueGap { get; set; }

        // حقوق
        public decimal FairSalary { get; set; }
        public decimal AdjustedSalary { get; set; }
        public decimal SalaryGap { get; set; } // مثبت = زیر ارزش، منفی = بالای ارزش

        // تصمیم
        public DecisionLevel Level { get; set; }
        public string LevelTitle { get; set; } = string.Empty;
        public string Interpretation { get; set; } = string.Empty;
        public string Decision { get; set; } = string.Empty;

        public List<Finding> Findings { get; set; } = new();
        public List<TaskComputationResult> Tasks { get; set; } = new();

        /// <summary>مشکلات مرتب‌شده بر اساس شدت (برای نمایش خلاصه در جدول اصلی).</summary>
        public IEnumerable<Finding> TopProblems =>
            Findings.Where(f => f.Severity >= Severity.Warning)
                    .OrderByDescending(f => f.Severity);
    }

    public class DashboardViewModel
    {
        public List<EmployeeEvaluationResult> Rows { get; set; } = new();
        public int? Year { get; set; }
        public int? Month { get; set; }

        public decimal TotalNetValue => Rows.Sum(r => r.NetValue);
        public decimal TotalBenefit => Rows.Sum(r => r.TotalBenefit);
        public decimal TotalCost => Rows.Sum(r => r.TotalCost);
        public int PeopleWithData => Rows.Count(r => r.HasData);
    }
}
