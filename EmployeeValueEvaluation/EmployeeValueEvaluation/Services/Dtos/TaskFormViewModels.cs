using System.ComponentModel.DataAnnotations;

namespace EmployeeValueEvaluation.Services.Dtos
{
    /// <summary>فرم ثبت یک Task همراه با مسیر، هزینه، منفعت و یک ریسک.</summary>
    public class TaskFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "انتخاب پرسنل الزامی است")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "عنوان کار الزامی است")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }

        [Range(1, 4)] public int Priority { get; set; } = 2;        // 1..4
        [Range(1, 5)] public int BusinessImpact { get; set; } = 3;  // 1..5

        public decimal EstimatedHours { get; set; }
        public decimal ActualHours { get; set; }

        // مسیر (WHAT/HOW/ACCEPTANCE)
        public string? WhatRequired { get; set; }
        public string? RequiredPath { get; set; }
        public string? AcceptanceCriteria { get; set; }
        public string? ActualPath { get; set; }
        public bool AlternativePathApproved { get; set; }

        // هزینه‌ها
        public decimal ManagerHoursCost { get; set; }
        public decimal ReworkCost { get; set; }
        public decimal DelayCost { get; set; }
        public decimal OpportunityCost { get; set; }
        public decimal RiskCost { get; set; }
        public decimal ExternalCost { get; set; }

        // منفعت‌ها
        public decimal RevenueBenefit { get; set; }
        public decimal SavingBenefit { get; set; }
        public decimal LossAvoidance { get; set; }
        public decimal ProductBenefit { get; set; }
        public decimal StrategicBenefit { get; set; }
        [Range(0, 1)] public decimal Confidence { get; set; } = 1m;

        // یک ریسک اختیاری
        public string? RiskType { get; set; }
        [Range(0, 1)] public decimal RiskProbability { get; set; }
        public decimal RiskImpact { get; set; }

        // 1=Pending,2=InProgress,3=Completed,4=Cancelled,5=OnHold
        public int Status { get; set; } = 3;
    }

    /// <summary>فرم ثبت ارزیابی یک Task.</summary>
    public class TaskEvaluateViewModel
    {
        public int TaskId { get; set; }
        public string TaskTitle { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;

        [Range(0, 100)] public decimal ComplianceScore { get; set; } = 100;
        [Range(0, 100)] public decimal QualityScore { get; set; } = 100;
        [Range(0, 100)] public decimal TimelinessScore { get; set; } = 100;
        [Range(0, 100)] public decimal ProblemSolvingScore { get; set; } = 70;
        [Range(0, 100)] public decimal DocumentationScore { get; set; } = 80;
        [Range(0, 100)] public decimal CommunicationScore { get; set; } = 80;

        [Range(0, 5)] public decimal UnauthorizedDeviationScore { get; set; } = 0;
        public decimal ReworkHours { get; set; }
        public int BugCount { get; set; }
        public int CriticalBugCount { get; set; }

        public string? EvaluatorComment { get; set; }
        public bool MarkCompleted { get; set; } = true;
    }
}
