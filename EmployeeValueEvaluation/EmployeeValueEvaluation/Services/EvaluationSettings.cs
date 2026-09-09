namespace EmployeeValueEvaluation.Services
{
    /// <summary>
    /// تمام ضرایب، وزن‌ها و اهداف مدل ارزندگی در یک جا.
    /// طبق بند ۷۲ سند، این اعداد بعد از ۳ تا ۶ ماه با داده واقعی تنظیم می‌شوند.
    /// می‌توانید این بخش را از appsettings.json هم بایند کنید (بخش "Evaluation").
    /// </summary>
    public class EvaluationSettings
    {
        // ---------- وزن‌های تابع عملکرد (بند ۴۱) ----------
        public decimal PerfCompliance { get; set; } = 0.25m;
        public decimal PerfQuality { get; set; } = 0.20m;
        public decimal PerfProductivity { get; set; } = 0.15m;
        public decimal PerfTimeliness { get; set; } = 0.10m;
        public decimal PerfProblemSolving { get; set; } = 0.10m;
        public decimal PerfDocumentation { get; set; } = 0.10m;
        public decimal PerfCommunication { get; set; } = 0.10m;

        // ---------- وزن‌های امتیاز هر Task (بند ۵۶) ----------
        public decimal TaskCompliance { get; set; } = 0.25m;
        public decimal TaskQuality { get; set; } = 0.25m;
        public decimal TaskEfficiency { get; set; } = 0.20m;
        public decimal TaskTimeliness { get; set; } = 0.10m;
        public decimal TaskProblemSolving { get; set; } = 0.10m;
        public decimal TaskDocumentation { get; set; } = 0.05m;
        public decimal TaskCommunication { get; set; } = 0.05m;

        // ---------- تابع ارزندگی نهایی (بند ۵۰) ----------
        public decimal FinalPerformanceWeight { get; set; } = 0.55m;
        public decimal FinalEconomicWeight { get; set; } = 0.45m;

        // ---------- وزن‌های ریسک (بند ۴۶) ----------
        public decimal RiskDeviation { get; set; } = 0.30m;
        public decimal RiskTechnical { get; set; } = 0.25m;
        public decimal RiskDependency { get; set; } = 0.20m;
        public decimal RiskContinuity { get; set; } = 0.15m;
        public decimal RiskSecurity { get; set; } = 0.10m;

        /// <summary>مبلغ زیان مورد انتظاری که معادل امتیاز ریسک ۱۰۰ است (برای نرمال‌سازی ریسک هر دسته).</summary>
        public decimal RiskExpectedLossFor100 { get; set; } = 50_000_000m;

        // ---------- کیفیت (بند ۱۵ و ۱۶) ----------
        public decimal BugPenaltyNormal { get; set; } = 5m;
        public decimal BugPenaltyCritical { get; set; } = 20m;

        // ---------- اقتصاد ----------
        /// <summary>SVR هدف برای محاسبه حقوق منصفانه (بند ۶۳).</summary>
        public decimal TargetSvr { get; set; } = 3.5m;

        /// <summary>مزایای جانبی ماهانه (بیمه، تجهیزات و ...) اگر در سطح Task ثبت نشده باشد (بند ۳۰).</summary>
        public decimal DefaultMonthlySideBenefits { get; set; } = 5_000_000m;

        /// <summary>ساعت کاری پرداخت‌شده در ماه، برای محاسبه بهره‌وری (بند ۴۳).</summary>
        public decimal PaidHoursPerMonth { get; set; } = 176m;

        // ---------- اهداف KPI (بند ۷۲) ----------
        public decimal TargetCompliance { get; set; } = 90m;
        public decimal TargetQuality { get; set; } = 90m;
        public decimal TargetReworkRatePercent { get; set; } = 5m;   // حداکثر
        public decimal TargetTimeliness { get; set; } = 85m;
        public decimal TargetDocumentation { get; set; } = 90m;
        public decimal TargetProductivity { get; set; } = 75m;
        public decimal TargetProblemSolving { get; set; } = 75m;
        public decimal TargetPerformance { get; set; } = 80m;
        public decimal TargetFinalValue { get; set; } = 70m;
        public decimal TargetRoiPercent { get; set; } = 50m;
        public int MaxUnauthorizedDeviationPerMonth { get; set; } = 1;
    }
}
