using EmployeeValueEvaluation.Models;
using EmployeeValueEvaluation.Services.Dtos;

namespace EmployeeValueEvaluation.Services
{
    public interface IValueEvaluationService
    {
        /// <summary>محاسبه یک Task (امتیاز، هزینه، منفعت).</summary>
        TaskComputationResult ComputeTask(TaskItem task);

        /// <summary>ارزیابی کامل یک نفر بر پایه Taskهای تکمیل‌شده در بازه انتخابی (یا کل زمان اگر بازه null باشد).</summary>
        EmployeeEvaluationResult EvaluateEmployee(Employee employee, int? year = null, int? month = null);

        /// <summary>ساخت جدول مقایسه‌ای همه پرسنل فعال.</summary>
        DashboardViewModel BuildDashboard(int? year = null, int? month = null);

        /// <summary>محاسبه و ذخیره ارزیابی ماهانه برای همه پرسنل فعال در ماه جاری.</summary>
        int RecomputeAndPersistMonthly(int year, int month);

        /// <summary>محاسبه ROI اصلاح (بند ۶۶).</summary>
        decimal ComputeCorrectionRoi(decimal correctionCost, decimal expectedBenefit, decimal probability);
    }
}
