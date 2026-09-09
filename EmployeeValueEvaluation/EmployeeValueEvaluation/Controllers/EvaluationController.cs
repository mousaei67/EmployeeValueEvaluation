using EmployeeValueEvaluation.Data;
using EmployeeValueEvaluation.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeValueEvaluation.Controllers
{
    public class EvaluationController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IValueEvaluationService _eval;

        public EvaluationController(AppDbContext context, IValueEvaluationService eval)
        {
            _context = context;
            _eval = eval;
        }

        // جدول کامل مقایسه‌ای همه پرسنل (خروجی اصلی)
        public IActionResult Index(int? year, int? month)
        {
            var vm = _eval.BuildDashboard(year, month);
            ViewData["Year"] = year;
            ViewData["Month"] = month;
            return View(vm);
        }

        // گزارش کامل یک نفر
        public async Task<IActionResult> Details(int? id, int? year, int? month)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null) return NotFound();

            var result = _eval.EvaluateEmployee(employee, year, month);
            return View(result);
        }

        // محاسبه و ذخیره ارزیابی ماهانه برای ماه انتخابی (پیش‌فرض: ماه جاری)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Recompute(int? year, int? month)
        {
            int y = year ?? DateTime.Now.Year;
            int m = month ?? DateTime.Now.Month;
            int n = _eval.RecomputeAndPersistMonthly(y, m);
            TempData["Message"] = $"ارزیابی ماهانه برای {n} نفر محاسبه و ذخیره شد ({y}/{m}).";
            return RedirectToAction(nameof(Index), new { year = y, month = m });
        }
    }
}
