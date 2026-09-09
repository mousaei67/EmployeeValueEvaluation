using EmployeeValueEvaluation.Models;
using ModelTaskStatus = EmployeeValueEvaluation.Models.TaskStatus;

namespace EmployeeValueEvaluation.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            // ---------- پرسنل ----------
            if (!context.Employees.Any())
            {
                var employees = new List<Employee>
                {
                    new Employee { Name = "مصطفی", PersonnelCode = "1001", Role = "Programmer",
                        BaseSalary = 30000000, StartDate = DateTime.Now.AddMonths(-8), IsActive = true,
                        Description = "برنامه‌نویس با عملکرد خوب اما انحراف مسیر" },
                    new Employee { Name = "علی", PersonnelCode = "1002", Role = "Senior Developer",
                        BaseSalary = 35000000, StartDate = DateTime.Now.AddMonths(-14), IsActive = true,
                        Description = "توسعه‌دهنده ارشد و منظم" },
                    new Employee { Name = "رضا", PersonnelCode = "1003", Role = "Developer",
                        BaseSalary = 40000000, StartDate = DateTime.Now.AddMonths(-5), IsActive = true,
                        Description = "توسعه‌دهنده با دوباره‌کاری و ریسک بالا" }
                };

                context.Employees.AddRange(employees);
                context.SaveChanges();

                foreach (var e in employees)
                    context.SalaryHistories.Add(new SalaryHistory
                    {
                        EmployeeId = e.Id,
                        Salary = e.BaseSalary,
                        EffectiveDate = e.StartDate,
                        Reason = "حقوق اولیه"
                    });

                context.SaveChanges();
            }

            // ---------- کارهای نمونه ----------
            if (!context.Tasks.Any())
            {
                var byCode = context.Employees.ToDictionary(e => e.PersonnelCode!, e => e.Id);
                var now = DateTime.Now;
                var m = new DateTime(now.Year, now.Month, 1);

                var tasks = new List<TaskItem>();

                // ===== مصطفی =====
                tasks.Add(MakeTask(byCode["1001"], "رفع خطای محاسبه قیمت عمده‌فروشی",
                    priority: 4, impact: 5, est: 8, act: 12,
                    due: m.AddDays(10), done: m.AddDays(12),
                    requiredPath: "WholesaleCalcService", actualPath: "Controller", altApproved: false,
                    compliance: 60, quality: 85, timeliness: 90, ps: 90, doc: 75, comm: 80,
                    devScore: 3, rework: 3, bug: 2, crit: 0, taskScore: 74.1m,
                    revenue: 50000000, saving: 20000000, loss: 30000000, product: 15000000, strategic: 10000000, conf: 0.8m,
                    mgmt: 5000000, reworkCost: 3000000, delay: 1000000, opp: 3000000, riskCost: 2000000, ext: 0,
                    risks: new[] { ("Deviation", 0.1m, 50000000m) }));

                tasks.Add(MakeTask(byCode["1001"], "افزودن گزارش سود ماهانه",
                    priority: 2, impact: 3, est: 10, act: 9,
                    due: m.AddDays(15), done: m.AddDays(15),
                    requiredPath: "ReportService", actualPath: "ReportService", altApproved: true,
                    compliance: 95, quality: 90, timeliness: 100, ps: 80, doc: 85, comm: 85,
                    devScore: 0, rework: 0, bug: 1, crit: 0, taskScore: 92.8m,
                    revenue: 0, saving: 5000000, loss: 0, product: 8000000, strategic: 2000000, conf: 0.9m,
                    mgmt: 2000000, reworkCost: 0, delay: 0, opp: 1000000, riskCost: 0, ext: 0,
                    risks: Array.Empty<(string, decimal, decimal)>()));

                // ===== علی =====
                tasks.Add(MakeTask(byCode["1002"], "طراحی و پیاده‌سازی سرویس کش قیمت",
                    priority: 3, impact: 5, est: 20, act: 18,
                    due: m.AddDays(20), done: m.AddDays(19),
                    requiredPath: "CacheService", actualPath: "CacheService", altApproved: true,
                    compliance: 98, quality: 95, timeliness: 100, ps: 92, doc: 92, comm: 90,
                    devScore: 0, rework: 1, bug: 1, crit: 0, taskScore: 96.6m,
                    revenue: 40000000, saving: 40000000, loss: 20000000, product: 20000000, strategic: 15000000, conf: 0.9m,
                    mgmt: 2000000, reworkCost: 1000000, delay: 0, opp: 1000000, riskCost: 0, ext: 0,
                    risks: new[] { ("Technical", 0.05m, 20000000m) }));

                tasks.Add(MakeTask(byCode["1002"], "بهینه‌سازی کوئری‌های گزارش",
                    priority: 2, impact: 4, est: 12, act: 11,
                    due: m.AddDays(25), done: m.AddDays(24),
                    requiredPath: "ReportRepository", actualPath: "ReportRepository", altApproved: true,
                    compliance: 95, quality: 92, timeliness: 100, ps: 85, doc: 90, comm: 88,
                    devScore: 0, rework: 0, bug: 0, crit: 0, taskScore: 94.2m,
                    revenue: 0, saving: 25000000, loss: 0, product: 10000000, strategic: 5000000, conf: 0.9m,
                    mgmt: 1000000, reworkCost: 0, delay: 0, opp: 0, riskCost: 0, ext: 0,
                    risks: Array.Empty<(string, decimal, decimal)>()));

                // ===== رضا =====
                tasks.Add(MakeTask(byCode["1003"], "تغییر ساختار ماژول پرداخت",
                    priority: 3, impact: 4, est: 15, act: 28,
                    due: m.AddDays(10), done: m.AddDays(18),
                    requiredPath: "PaymentService (اصلاح موضعی)", actualPath: "بازنویسی کامل ماژول", altApproved: false,
                    compliance: 40, quality: 55, timeliness: 40, ps: 60, doc: 45, comm: 55,
                    devScore: 4, rework: 12, bug: 6, crit: 1, taskScore: 49.5m,
                    revenue: 0, saving: 10000000, loss: 5000000, product: 5000000, strategic: 0, conf: 0.6m,
                    mgmt: 4000000, reworkCost: 8000000, delay: 3000000, opp: 4000000, riskCost: 2000000, ext: 0,
                    risks: new[] { ("Deviation", 0.2m, 40000000m), ("Technical", 0.3m, 30000000m), ("Continuity", 0.2m, 25000000m) }));

                tasks.Add(MakeTask(byCode["1003"], "رفع چند باگ رابط کاربری",
                    priority: 1, impact: 2, est: 6, act: 8,
                    due: m.AddDays(20), done: m.AddDays(22),
                    requiredPath: "UI Layer", actualPath: "UI Layer", altApproved: true,
                    compliance: 80, quality: 70, timeliness: 90, ps: 60, doc: 50, comm: 60,
                    devScore: 0, rework: 3, bug: 3, crit: 0, taskScore: 73.0m,
                    revenue: 0, saving: 2000000, loss: 0, product: 2000000, strategic: 0, conf: 0.7m,
                    mgmt: 2000000, reworkCost: 2000000, delay: 0, opp: 0, riskCost: 0, ext: 0,
                    risks: Array.Empty<(string, decimal, decimal)>()));

                context.Tasks.AddRange(tasks);
                context.SaveChanges();
            }
        }

        private static TaskItem MakeTask(
            int employeeId, string title,
            int priority, int impact, decimal est, decimal act,
            DateTime due, DateTime done,
            string requiredPath, string actualPath, bool altApproved,
            decimal compliance, decimal quality, decimal timeliness, decimal ps, decimal doc, decimal comm,
            decimal devScore, decimal rework, int bug, int crit, decimal taskScore,
            decimal revenue, decimal saving, decimal loss, decimal product, decimal strategic, decimal conf,
            decimal mgmt, decimal reworkCost, decimal delay, decimal opp, decimal riskCost, decimal ext,
            (string type, decimal prob, decimal impactMoney)[] risks)
        {
            decimal adjusted = (product + strategic) * (conf <= 0 ? 1 : conf);
            decimal efficiency = act > 0 && est > 0 ? Math.Min(100m, Math.Round(est / act * 100m, 2)) : 100m;

            var task = new TaskItem
            {
                EmployeeId = employeeId,
                Title = title,
                CreatedAt = due.AddDays(-5),
                DueDate = due,
                CompletedAt = done,
                Priority = priority,
                BusinessImpact = impact,
                EstimatedHours = est,
                ActualHours = act,
                RequiredPath = requiredPath,
                ActualPath = actualPath,
                AlternativePathApproved = altApproved,
                PathApprovalRequired = true,
                Status = ModelTaskStatus.Completed,
                Instruction = new TaskInstruction
                {
                    WhatRequired = title,
                    RequiredPath = requiredPath,
                    AcceptanceCriteria = "خروجی مطابق نیازمندی و بدون رگرسیون",
                    AlternativeAllowed = true,
                    ApprovalRequired = true,
                    ApprovedAlternative = altApproved ? actualPath : null
                },
                Evaluation = new TaskEvaluation
                {
                    ComplianceScore = compliance,
                    QualityScore = quality,
                    EfficiencyScore = efficiency,
                    TimelinessScore = timeliness,
                    ProblemSolvingScore = ps,
                    DocumentationScore = doc,
                    CommunicationScore = comm,
                    UnauthorizedDeviationScore = devScore,
                    ReworkHours = rework,
                    BugCount = bug,
                    CriticalBugCount = crit,
                    TaskScore = taskScore,
                    EvaluatedAt = done
                },
                Cost = new TaskCost
                {
                    ManagerHoursCost = mgmt,
                    ReworkCost = reworkCost,
                    DelayCost = delay,
                    OpportunityCost = opp,
                    RiskCost = riskCost,
                    ExternalCost = ext,
                    TotalCost = mgmt + reworkCost + delay + opp + riskCost + ext
                },
                Benefit = new TaskBenefit
                {
                    RevenueBenefit = revenue,
                    SavingBenefit = saving,
                    LossAvoidance = loss,
                    ProductBenefit = product,
                    StrategicBenefit = strategic,
                    Confidence = conf <= 0 ? 1 : conf,
                    AdjustedBenefit = adjusted,
                    TotalBenefit = revenue + saving + loss + adjusted
                }
            };

            foreach (var r in risks)
                task.Risks.Add(new TaskRisk
                {
                    RiskType = r.type,
                    Probability = r.prob,
                    Impact = r.impactMoney,
                    ExpectedLoss = r.prob * r.impactMoney
                });

            return task;
        }
    }
}
