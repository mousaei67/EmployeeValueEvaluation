using EmployeeValueEvaluation.Data;
using EmployeeValueEvaluation.Models;
using EmployeeValueEvaluation.Services.Dtos;
using Microsoft.EntityFrameworkCore;
using ModelTaskStatus = EmployeeValueEvaluation.Models.TaskStatus;

namespace EmployeeValueEvaluation.Services
{
    /// <summary>
    /// موتور محاسبه ارزندگی. زنجیره سند:
    /// Task -> Cost + Benefit -> ROI -> Performance -> Risk -> Final Value -> Decision
    /// </summary>
    public class ValueEvaluationService : IValueEvaluationService
    {
        private readonly AppDbContext _context;
        private readonly EvaluationSettings _s;

        public ValueEvaluationService(AppDbContext context, EvaluationSettings settings)
        {
            _context = context;
            _s = settings;
        }

        // ==================================================================
        //  محاسبه یک Task
        // ==================================================================
        public TaskComputationResult ComputeTask(TaskItem task)
        {
            var e = task.Evaluation;

            int weight = Math.Max(1, task.Priority) * Math.Max(1, task.BusinessImpact); // بند ۸

            decimal efficiency = ComputeEfficiency(task, e);
            decimal timeliness = ComputeTimeliness(task, e);

            decimal compliance = e?.ComplianceScore ?? 0m;
            decimal quality = e != null ? ComputeQuality(e) : 0m;
            decimal problemSolving = e?.ProblemSolvingScore ?? 0m;
            decimal documentation = e?.DocumentationScore ?? 0m;
            decimal communication = e?.CommunicationScore ?? 0m;

            // امتیاز Task (بند ۵۶)
            decimal taskScore =
                _s.TaskCompliance * compliance +
                _s.TaskQuality * quality +
                _s.TaskEfficiency * efficiency +
                _s.TaskTimeliness * timeliness +
                _s.TaskProblemSolving * problemSolving +
                _s.TaskDocumentation * documentation +
                _s.TaskCommunication * communication;

            return new TaskComputationResult
            {
                TaskId = task.Id,
                Title = task.Title,
                Priority = task.Priority,
                BusinessImpact = task.BusinessImpact,
                TaskWeight = weight,
                Compliance = Round(compliance),
                Quality = Round(quality),
                Efficiency = Round(efficiency),
                Timeliness = Round(timeliness),
                ProblemSolving = Round(problemSolving),
                Documentation = Round(documentation),
                Communication = Round(communication),
                TaskScore = Round(taskScore),
                TotalCost = ComputeTaskCost(task),
                TotalBenefit = ComputeTaskBenefit(task),
                ReworkHours = e?.ReworkHours ?? 0m,
                BugCount = e?.BugCount ?? 0,
                CriticalBugCount = e?.CriticalBugCount ?? 0,
                UnauthorizedDeviation = IsUnauthorizedDeviation(task, e),
                EvaluatorComment = e?.EvaluatorComment
            };
        }

        // کیفیت (بند ۱۵ و ۱۶)
        private decimal ComputeQuality(TaskEvaluation e)
        {
            // اگر امتیاز کیفیت مستقیم ثبت شده باشد از همان استفاده می‌کنیم،
            // در غیر این صورت از باگ‌ها می‌سازیم.
            decimal bugScore = Clamp(
                100m - _s.BugPenaltyNormal * e.BugCount - _s.BugPenaltyCritical * e.CriticalBugCount,
                0m, 100m);

            if (e.QualityScore > 0m)
                return Clamp(e.QualityScore, 0m, 100m);

            return bugScore;
        }

        // بهره‌وری زمانی هر Task (بند ۱۸ و ۱۹ - سقف ۱۰۰ تا سرعت بیش از حد پاداش نگیرد)
        private decimal ComputeEfficiency(TaskItem task, TaskEvaluation? e)
        {
            if (task.ActualHours > 0m && task.EstimatedHours > 0m)
                return Clamp(task.EstimatedHours / task.ActualHours * 100m, 0m, 100m);

            return e?.EfficiencyScore ?? 100m;
        }

        // تحویل به‌موقع (بند ۲۰)
        private decimal ComputeTimeliness(TaskItem task, TaskEvaluation? e)
        {
            if (task.DueDate.HasValue && task.CompletedAt.HasValue)
            {
                int delay = (int)Math.Ceiling((task.CompletedAt.Value - task.DueDate.Value).TotalDays);
                return TimelinessFromDelay(delay);
            }

            return e?.TimelinessScore ?? 100m;
        }

        private static decimal TimelinessFromDelay(int days)
        {
            if (days <= 0) return 100m;
            return days switch
            {
                1 => 95m,
                2 => 90m,
                3 => 80m,
                4 => 72m,
                5 => 65m,
                6 => 52m,
                7 => 45m,
                _ => 40m
            };
        }

        // هزینه Task = مجموع اجزای هزینه (بند ۵۹). هزینه ساعات خود فرد در سطح ماه از «حقوق» می‌آید،
        // پس اینجا برای جلوگیری از دوبار‌شماری در مجموع ماهانه لحاظ نمی‌شود.
        private decimal ComputeTaskCost(TaskItem task)
        {
            var c = task.Cost;
            if (c == null) return 0m;
            return c.ManagerHoursCost + c.ReworkCost + c.DelayCost +
                   c.OpportunityCost + c.RiskCost + c.ExternalCost;
        }

        // منفعت Task = منفعت مستقیم + منفعت تخمینی×ضریب اطمینان (بند ۲۴ تا ۲۷)
        private decimal ComputeTaskBenefit(TaskItem task)
        {
            var b = task.Benefit;
            if (b == null) return 0m;

            decimal direct = b.RevenueBenefit + b.SavingBenefit + b.LossAvoidance;
            decimal estimated = b.ProductBenefit + b.StrategicBenefit;
            decimal confidence = b.Confidence <= 0m ? 1m : Clamp(b.Confidence, 0m, 1m);
            decimal adjusted = estimated * confidence;
            return direct + adjusted;
        }

        private bool IsUnauthorizedDeviation(TaskItem task, TaskEvaluation? e)
        {
            if (e != null && e.UnauthorizedDeviationScore > 0m)
                return true;

            bool pathsDiffer =
                !string.IsNullOrWhiteSpace(task.RequiredPath) &&
                !string.IsNullOrWhiteSpace(task.ActualPath) &&
                !string.Equals(task.RequiredPath.Trim(), task.ActualPath.Trim(),
                    StringComparison.OrdinalIgnoreCase);

            return pathsDiffer && !task.AlternativePathApproved;
        }

        // ==================================================================
        //  ارزیابی یک نفر
        // ==================================================================
        public EmployeeEvaluationResult EvaluateEmployee(Employee employee, int? year = null, int? month = null)
        {
            var query = _context.Tasks
                .Include(t => t.Evaluation)
                .Include(t => t.Cost)
                .Include(t => t.Benefit)
                .Include(t => t.Risks)
                .Include(t => t.Instruction)
                .Where(t => t.EmployeeId == employee.Id && t.Status == ModelTaskStatus.Completed);

            var tasks = query.ToList();

            if (year.HasValue && month.HasValue)
            {
                tasks = tasks.Where(t =>
                {
                    var d = t.CompletedAt ?? t.CreatedAt;
                    return d.Year == year.Value && d.Month == month.Value;
                }).ToList();
            }

            var result = new EmployeeEvaluationResult
            {
                EmployeeId = employee.Id,
                Name = employee.Name,
                Role = employee.Role,
                PersonnelCode = employee.PersonnelCode,
                Salary = employee.BaseSalary,
                TaskCount = tasks.Count
            };

            var taskResults = tasks.Select(ComputeTask).ToList();
            result.Tasks = taskResults;

            if (taskResults.Count == 0)
            {
                result.LevelTitle = "بدون داده";
                result.Interpretation = "برای این فرد هنوز Task تکمیل‌شده‌ای ثبت نشده است.";
                result.Decision = "ابتدا Task و ارزیابی ثبت کنید.";
                return result;
            }

            decimal totalWeight = taskResults.Sum(t => (decimal)t.TaskWeight);
            if (totalWeight <= 0m) totalWeight = 1m;

            // میانگین وزنی ابعاد عملکرد
            decimal WAvg(Func<TaskComputationResult, decimal> sel) =>
                taskResults.Sum(t => sel(t) * t.TaskWeight) / totalWeight;

            result.Compliance = Round(WAvg(t => t.Compliance));
            result.Quality = Round(WAvg(t => t.Quality));
            result.Timeliness = Round(WAvg(t => t.Timeliness));
            result.ProblemSolving = Round(WAvg(t => t.ProblemSolving));
            result.Documentation = Round(WAvg(t => t.Documentation));
            result.Communication = Round(WAvg(t => t.Communication));

            // بهره‌وری (بند ۴۳). تعریف: سهم ساعات مفید (بدون دوباره‌کاری) از کل ساعات ثبت‌شده.
            // این تعریف با داده جزئی (وقتی همه ساعات ماه ثبت نشده) هم پایدار است.
            // اگر خواستید دقیقاً بر پایه ساعت پرداخت‌شده ماه حساب شود، مخرج را روی PaidHoursPerMonth بگذارید.
            decimal totalActual = tasks.Sum(t => t.ActualHours);
            decimal totalRework = taskResults.Sum(t => t.ReworkHours);
            decimal usefulHours = Math.Max(0m, totalActual - totalRework);
            decimal productivityBase = totalActual > 0m ? totalActual : _s.PaidHoursPerMonth;
            result.Productivity = Round(Clamp(SafeDiv(usefulHours, productivityBase) * 100m, 0m, 100m));

            // عملکرد (بند ۴۱)
            result.Performance = Round(
                _s.PerfCompliance * result.Compliance +
                _s.PerfQuality * result.Quality +
                _s.PerfProductivity * result.Productivity +
                _s.PerfTimeliness * result.Timeliness +
                _s.PerfProblemSolving * result.ProblemSolving +
                _s.PerfDocumentation * result.Documentation +
                _s.PerfCommunication * result.Communication);

            // شاخص‌های کمکی
            result.TotalBugs = taskResults.Sum(t => t.BugCount);
            result.CriticalBugs = taskResults.Sum(t => t.CriticalBugCount);
            result.ReworkRatePercent = Round(Clamp(SafeDiv(totalRework, totalActual) * 100m, 0m, 100m)); // بند ۱۷
            result.UnauthorizedDeviationCount = taskResults.Count(t => t.UnauthorizedDeviation);

            decimal deviationHours = taskResults.Where(t => t.UnauthorizedDeviation).Sum(t => t.ReworkHours);
            result.UnauthorizedDeviationRatePercent =
                Round(Clamp(SafeDiv(deviationHours, totalActual) * 100m, 0m, 100m)); // UDR بند ۱۴

            // ---------- هزینه (بند ۲۸ تا ۳۶) ----------
            result.CostSalary = employee.BaseSalary;
            result.CostSideBenefits = _s.DefaultMonthlySideBenefits;
            result.CostManagement = tasks.Sum(t => t.Cost?.ManagerHoursCost ?? 0m);
            result.CostRework = tasks.Sum(t => t.Cost?.ReworkCost ?? 0m);
            result.CostDelay = tasks.Sum(t => t.Cost?.DelayCost ?? 0m);
            result.CostOpportunity = tasks.Sum(t => t.Cost?.OpportunityCost ?? 0m);
            result.CostRisk = tasks.Sum(t => t.Cost?.RiskCost ?? 0m);
            result.CostExternal = tasks.Sum(t => t.Cost?.ExternalCost ?? 0m);

            result.TotalCost = result.CostSalary + result.CostSideBenefits + result.CostManagement +
                               result.CostRework + result.CostDelay + result.CostOpportunity +
                               result.CostRisk + result.CostExternal;

            // ---------- منفعت (بند ۲۴) ----------
            result.TotalBenefit = taskResults.Sum(t => t.TotalBenefit);

            // ---------- ارزش خالص و شاخص‌های اقتصادی (بند ۳۷ تا ۴۰) ----------
            result.NetValue = result.TotalBenefit - result.TotalCost;
            result.Roi = Round(SafeDiv(result.NetValue, result.TotalCost) * 100m);
            result.Bcr = Round(SafeDiv(result.TotalBenefit, result.TotalCost), 3);
            result.Svr = Round(SafeDiv(result.TotalBenefit, employee.BaseSalary), 3);
            result.EconomicScore = EconomicScoreFromRoi(result.Roi); // بند ۴۹

            // ---------- ریسک (بند ۴۶ و ۴۷) ----------
            ComputeRisk(result, tasks, taskResults);

            // ---------- ارزندگی نهایی (بند ۵۰) ----------
            decimal preRisk =
                _s.FinalPerformanceWeight * result.Performance +
                _s.FinalEconomicWeight * result.EconomicScore;

            result.FinalValueScore = Round(preRisk * result.RiskFactor);

            // ---------- پتانسیل (بند ۶۵) ----------
            ComputePotential(result);

            // ---------- حقوق (بند ۶۳ و ۶۴) ----------
            result.FairSalary = Round(SafeDiv(result.TotalBenefit, _s.TargetSvr), 0);
            result.AdjustedSalary = Round(result.FairSalary * result.Performance / 100m, 0);
            result.SalaryGap = Round(result.AdjustedSalary - employee.BaseSalary, 0);

            // ---------- تفسیر و تصمیم ----------
            result.Interpretation = InterpretationBand(result.FinalValueScore);
            AssignDecision(result);

            // ---------- یافته‌ها ----------
            BuildFindings(result);

            return result;
        }

        // امتیاز اقتصادی از ROI (بند ۴۹)
        private static decimal EconomicScoreFromRoi(decimal roi)
        {
            if (roi < 0m) return 0m;
            if (roi < 25m) return 40m;
            if (roi < 50m) return 55m;
            if (roi < 100m) return 70m;
            if (roi < 200m) return 80m;
            if (roi < 500m) return 90m;
            return 100m;
        }

        private void ComputeRisk(EmployeeEvaluationResult r, List<TaskItem> tasks, List<TaskComputationResult> tr)
        {
            decimal tech = 0m, dep = 0m, cont = 0m, sec = 0m, devLoss = 0m;

            foreach (var risk in tasks.SelectMany(t => t.Risks))
            {
                decimal expected = risk.ExpectedLoss > 0m
                    ? risk.ExpectedLoss
                    : Clamp(risk.Probability, 0m, 1m) * risk.Impact; // بند ۳۵/۶۱

                string type = (risk.RiskType ?? string.Empty).ToLowerInvariant();

                if (type.Contains("secur") || type.Contains("امنیت")) sec += expected;
                else if (type.Contains("depend") || type.Contains("وابست")) dep += expected;
                else if (type.Contains("continu") || type.Contains("تداوم") || type.Contains("خروج")) cont += expected;
                else if (type.Contains("deviat") || type.Contains("انحراف") || type.Contains("مسیر")) devLoss += expected;
                else tech += expected; // پیش‌فرض: ریسک فنی
            }

            decimal Norm(decimal loss) => Clamp(SafeDiv(loss, _s.RiskExpectedLossFor100) * 100m, 0m, 100m);

            // ریسک انحراف: ترکیب UDR و تعداد انحراف بدون تأیید + زیان انحراف
            decimal devFromBehavior = Clamp(
                r.UnauthorizedDeviationRatePercent * 4m + r.UnauthorizedDeviationCount * 10m, 0m, 100m);
            r.DeviationRisk = Round(Math.Max(devFromBehavior, Norm(devLoss)));

            r.TechnicalRisk = Round(Norm(tech));
            r.DependencyRisk = Round(Norm(dep));
            r.ContinuityRisk = Round(Norm(cont));
            r.SecurityRisk = Round(Norm(sec));

            r.RiskScore = Round(
                _s.RiskDeviation * r.DeviationRisk +
                _s.RiskTechnical * r.TechnicalRisk +
                _s.RiskDependency * r.DependencyRisk +
                _s.RiskContinuity * r.ContinuityRisk +
                _s.RiskSecurity * r.SecurityRisk);

            r.RiskFactor = Round(Clamp(1m - r.RiskScore / 100m, 0m, 1m), 4); // بند ۴۷
        }

        private void ComputePotential(EmployeeEvaluationResult r)
        {
            decimal Up(decimal cur, decimal target) => Math.Max(cur, target);

            decimal potPerf =
                _s.PerfCompliance * Up(r.Compliance, _s.TargetCompliance) +
                _s.PerfQuality * Up(r.Quality, _s.TargetQuality) +
                _s.PerfProductivity * Up(r.Productivity, _s.TargetProductivity) +
                _s.PerfTimeliness * Up(r.Timeliness, _s.TargetTimeliness) +
                _s.PerfProblemSolving * Up(r.ProblemSolving, _s.TargetProblemSolving) +
                _s.PerfDocumentation * Up(r.Documentation, _s.TargetDocumentation) +
                _s.PerfCommunication * Up(r.Communication, 85m);

            decimal potRiskFactor = Clamp(1m - (r.RiskScore * 0.5m) / 100m, 0m, 1m); // فرض: نصف ریسک کنترل شود

            decimal potFinal =
                (_s.FinalPerformanceWeight * potPerf + _s.FinalEconomicWeight * r.EconomicScore) * potRiskFactor;

            r.PotentialValue = Round(potFinal);
            r.ValueGap = Round(Math.Max(0m, r.PotentialValue - r.FinalValueScore));
        }

        // تفسیر امتیاز (بند ۵۳)
        private static string InterpretationBand(decimal f)
        {
            if (f >= 90m) return "فوق‌العاده ارزشمند";
            if (f >= 80m) return "بسیار ارزشمند";
            if (f >= 70m) return "ارزشمند";
            if (f >= 60m) return "ارزشمند مشروط";
            if (f >= 50m) return "ضعیف";
            if (f >= 40m) return "پرریسک";
            return "غیراقتصادی";
        }

        // تصمیم مدیریتی (بند ۷۸)
        private void AssignDecision(EmployeeEvaluationResult r)
        {
            if (r.NetValue < 0m && r.FinalValueScore < 50m)
            {
                r.Level = DecisionLevel.D_Uneconomic;
                r.LevelTitle = "غیراقتصادی";
                r.Decision = "ادامه همکاری با شرایط فعلی از نظر اقتصادی توجیه ندارد؛ یا شرایط همکاری/نقش باید تغییر کند یا برنامه اصلاح فشرده اجرا شود.";
            }
            else if (r.FinalValueScore >= 80m && r.NetValue > 0m && r.RiskScore <= 25m)
            {
                r.Level = DecisionLevel.A_Valuable;
                r.LevelTitle = "ارزشمند";
                r.Decision = "ادامه همکاری؛ امکان افزایش مسئولیت و بازنگری حقوق به سمت بالا وجود دارد.";
            }
            else if (r.FinalValueScore >= 60m)
            {
                r.Level = DecisionLevel.B_ConditionallyValuable;
                r.LevelTitle = "ارزشمند مشروط";
                r.Decision = "ادامه همکاری + تعیین KPI + برنامه اصلاح روی نقاط ضعف (نه اخراج، نه افزایش فوری حقوق).";
            }
            else
            {
                r.Level = DecisionLevel.C_Risky;
                r.LevelTitle = "پرریسک";
                r.Decision = "بررسی ریشه‌ای علت، اجرای دوره اصلاح کوتاه‌مدت و ارزیابی مجدد؛ کنترل نزدیک لازم است.";
            }
        }

        // تولید یافته‌ها: کجا مشکل هست و چه اقدام مدیریتی لازم است
        private void BuildFindings(EmployeeEvaluationResult r)
        {
            var list = r.Findings;

            // Compliance
            if (r.Compliance < _s.TargetCompliance)
                list.Add(new Finding
                {
                    Metric = "پایبندی به مسیر (Compliance)",
                    Value = $"{r.Compliance:0.#}%",
                    Target = $"≥ {_s.TargetCompliance:0}%",
                    Severity = r.Compliance < 70m ? Severity.Critical : Severity.Warning,
                    Problem = "کارها همیشه مطابق مسیر و روش توافق‌شده انجام نمی‌شوند.",
                    Recommendation = "برای هر Task مسیر (WHAT/HOW/ACCEPTANCE) را مکتوب کنید و قبل از شروع تأیید بگیرید؛ انحراف باید از پیش هماهنگ شود."
                });

            // انحراف بدون تأیید
            if (r.UnauthorizedDeviationCount > _s.MaxUnauthorizedDeviationPerMonth)
                list.Add(new Finding
                {
                    Metric = "انحراف بدون هماهنگی",
                    Value = $"{r.UnauthorizedDeviationCount} مورد (UDR {r.UnauthorizedDeviationRatePercent:0.#}%)",
                    Target = $"≤ {_s.MaxUnauthorizedDeviationPerMonth} مورد",
                    Severity = r.UnauthorizedDeviationCount > 2 ? Severity.Critical : Severity.Warning,
                    Problem = "تغییر مسیر بدون اطلاع، منشأ دوباره‌کاری و ریسک است.",
                    Recommendation = "قانون «تصمیم فنی متفاوت مجاز است، اما فقط با اطلاع و تأیید قبلی» را اجرا کنید؛ تغییر مستقل مسیر باید ثبت و بررسی شود."
                });

            // کیفیت / باگ بحرانی
            if (r.CriticalBugs > 0)
                list.Add(new Finding
                {
                    Metric = "باگ بحرانی",
                    Value = $"{r.CriticalBugs} مورد",
                    Target = "نزدیک صفر",
                    Severity = Severity.Critical,
                    Problem = "وجود باگ بحرانی، ریسک مستقیم روی کسب‌وکار می‌گذارد.",
                    Recommendation = "بازبینی کد اجباری (Code Review) و تست پذیرش قبل از Merge برای کارهای با اثر بالا."
                });

            if (r.Quality < _s.TargetQuality)
                list.Add(new Finding
                {
                    Metric = "کیفیت (Quality)",
                    Value = $"{r.Quality:0.#}",
                    Target = $"≥ {_s.TargetQuality:0}",
                    Severity = r.Quality < 70m ? Severity.Critical : Severity.Warning,
                    Problem = "نرخ خطا/بازگشت کار بالاتر از حد هدف است.",
                    Recommendation = "چک‌لیست کیفیت و تست خودکار برای مسیرهای حساس اضافه کنید."
                });

            // دوباره‌کاری
            if (r.ReworkRatePercent > _s.TargetReworkRatePercent)
                list.Add(new Finding
                {
                    Metric = "دوباره‌کاری (Rework)",
                    Value = $"{r.ReworkRatePercent:0.#}%",
                    Target = $"≤ {_s.TargetReworkRatePercent:0}%",
                    Severity = r.ReworkRatePercent > _s.TargetReworkRatePercent * 2m ? Severity.Critical : Severity.Warning,
                    Problem = "بخشی از ساعات صرف انجام دوبارهٔ کار می‌شود که هزینه مستقیم دارد.",
                    Recommendation = "علت دوباره‌کاری‌ها را ریشه‌یابی کنید (ابهام نیازمندی؟ نبود تست؟ انحراف مسیر؟) و همان ریشه را برطرف کنید."
                });

            // تحویل به‌موقع
            if (r.Timeliness < _s.TargetTimeliness)
                list.Add(new Finding
                {
                    Metric = "تحویل به‌موقع (Timeliness)",
                    Value = $"{r.Timeliness:0.#}",
                    Target = $"≥ {_s.TargetTimeliness:0}",
                    Severity = r.Timeliness < 60m ? Severity.Critical : Severity.Warning,
                    Problem = "تأخیر در تحویل روی برنامه‌ریزی و هزینه فرصت اثر می‌گذارد.",
                    Recommendation = "تخمین‌ها را واقعی‌تر کنید و کارهای بزرگ را به گام‌های کوچک‌تر با موعد مشخص بشکنید."
                });

            // بهره‌وری
            if (r.Productivity < _s.TargetProductivity)
                list.Add(new Finding
                {
                    Metric = "بهره‌وری (Productivity)",
                    Value = $"{r.Productivity:0.#}%",
                    Target = $"≥ {_s.TargetProductivity:0}%",
                    Severity = Severity.Warning,
                    Problem = "نسبت ساعات مفید به ساعات پرداخت‌شده پایین است.",
                    Recommendation = "منابع اتلاف زمان (جلسات اضافی، بلاک‌شدن، ابهام کار) را شناسایی و حذف کنید."
                });

            // مستندسازی
            if (r.Documentation < _s.TargetDocumentation)
                list.Add(new Finding
                {
                    Metric = "مستندسازی (Documentation)",
                    Value = $"{r.Documentation:0.#}",
                    Target = $"≥ {_s.TargetDocumentation:0}",
                    Severity = Severity.Warning,
                    Problem = "ثبت تصمیم‌ها، تغییرات دیتابیس و روش استفاده کامل نیست؛ این وابستگی و ریسک تداوم می‌سازد.",
                    Recommendation = "ثبت خلاصهٔ تصمیم فنی و Migration/تغییر دیتابیس را بخشی از تعریف «کار تمام‌شده» کنید."
                });

            // ریسک
            if (r.RiskScore > 25m)
                list.Add(new Finding
                {
                    Metric = "ریسک کلی (Risk)",
                    Value = $"{r.RiskScore:0.#}",
                    Target = "پایین (< 25)",
                    Severity = r.RiskScore > 40m ? Severity.Critical : Severity.Warning,
                    Problem = "ریسک عملکرد (انحراف/فنی/وابستگی/تداوم/امنیت) بالاست و روی ارزندگی نهایی اثر کاهنده دارد.",
                    Recommendation = "بزرگ‌ترین مؤلفهٔ ریسک را هدف بگیرید؛ برای وابستگی، دانش را مستند و توزیع کنید و کارهای حساس را دو‌نفره کنید."
                });

            // اقتصاد
            if (r.NetValue < 0m)
                list.Add(new Finding
                {
                    Metric = "ارزش خالص (Net Value)",
                    Value = $"{r.NetValue:N0}",
                    Target = "> 0",
                    Severity = Severity.Critical,
                    Problem = "هزینهٔ کل بیش از منفعت ایجادشده است.",
                    Recommendation = "فرد را روی کارهای با اثر اقتصادی بالاتر بگذارید و هزینه‌های قابل‌حذف (دوباره‌کاری، مدیریت اضافی، فرصت) را کم کنید."
                });
            else if (r.Roi < _s.TargetRoiPercent)
                list.Add(new Finding
                {
                    Metric = "بازده (ROI)",
                    Value = $"{r.Roi:0.#}%",
                    Target = $"> {_s.TargetRoiPercent:0}%",
                    Severity = Severity.Warning,
                    Problem = "بازده اقتصادی مثبت اما پایین‌تر از هدف است.",
                    Recommendation = "تخصیص کار را به سمت Taskهای با TaskWeight بالاتر (اولویت × اثر) ببرید."
                });

            // عملکرد کلی
            if (r.Performance < _s.TargetPerformance)
                list.Add(new Finding
                {
                    Metric = "عملکرد کلی (Performance)",
                    Value = $"{r.Performance:0.#}",
                    Target = $"≥ {_s.TargetPerformance:0}",
                    Severity = Severity.Warning,
                    Problem = "میانگین وزنی ابعاد عملکرد زیر هدف است.",
                    Recommendation = "روی دو مؤلفهٔ ضعیف‌تر (که در همین جدول علامت خورده‌اند) تمرکز کنید، نه همه‌چیز هم‌زمان."
                });

            // نقاط قوت (Info) برای تصویر متوازن
            if (r.ProblemSolving >= 85m)
                list.Add(new Finding
                {
                    Metric = "حل مسئله",
                    Value = $"{r.ProblemSolving:0.#}",
                    Target = "-",
                    Severity = Severity.Info,
                    Problem = "نقطه قوت: توان حل مسئله بالاست.",
                    Recommendation = "از این نقطه قوت برای کارهای پیچیده‌تر و منتورینگ استفاده کنید."
                });

            if (r.FinalValueScore >= _s.TargetFinalValue && r.NetValue > 0m && !list.Any(f => f.Severity >= Severity.Warning))
                list.Add(new Finding
                {
                    Metric = "وضعیت کلی",
                    Value = $"{r.FinalValueScore:0.#}",
                    Target = $"≥ {_s.TargetFinalValue:0}",
                    Severity = Severity.Ok,
                    Problem = "مشکل قابل‌توجهی یافت نشد.",
                    Recommendation = "وضعیت مطلوب است؛ فقط پایش دوره‌ای ادامه یابد."
                });
        }

        // ==================================================================
        //  داشبورد
        // ==================================================================
        public DashboardViewModel BuildDashboard(int? year = null, int? month = null)
        {
            var employees = _context.Employees
                .Where(e => e.IsActive)
                .OrderBy(e => e.Name)
                .ToList();

            var vm = new DashboardViewModel { Year = year, Month = month };
            foreach (var emp in employees)
                vm.Rows.Add(EvaluateEmployee(emp, year, month));

            vm.Rows = vm.Rows
                .OrderByDescending(r => r.HasData)
                .ThenByDescending(r => r.FinalValueScore)
                .ToList();

            return vm;
        }

        // ==================================================================
        //  ذخیره ارزیابی ماهانه
        // ==================================================================
        public int RecomputeAndPersistMonthly(int year, int month)
        {
            var employees = _context.Employees.Where(e => e.IsActive).ToList();
            int count = 0;

            foreach (var emp in employees)
            {
                var r = EvaluateEmployee(emp, year, month);
                if (!r.HasData) continue;

                var row = _context.MonthlyEvaluations
                    .FirstOrDefault(m => m.EmployeeId == emp.Id && m.Year == year && m.Month == month);

                if (row == null)
                {
                    row = new MonthlyEvaluation { EmployeeId = emp.Id, Year = year, Month = month };
                    _context.MonthlyEvaluations.Add(row);
                }

                row.Salary = r.Salary;
                row.TotalBenefit = r.TotalBenefit;
                row.TotalCost = r.TotalCost;
                row.NetValue = r.NetValue;
                row.ROI = r.Roi;
                row.BCR = r.Bcr;
                row.SVR = r.Svr;
                row.Compliance = r.Compliance;
                row.Quality = r.Quality;
                row.Productivity = r.Productivity;
                row.Timeliness = r.Timeliness;
                row.ProblemSolving = r.ProblemSolving;
                row.Documentation = r.Documentation;
                row.Communication = r.Communication;
                row.Performance = r.Performance;
                row.RiskScore = r.RiskScore;
                row.RiskFactor = r.RiskFactor;
                row.EconomicScore = r.EconomicScore;
                row.FinalValueScore = r.FinalValueScore;
                row.FairSalary = r.FairSalary;
                row.PotentialValue = r.PotentialValue;
                row.ValueGap = r.ValueGap;
                row.Decision = $"{r.LevelTitle} — {r.Decision}";
                row.CreatedAt = DateTime.Now;

                count++;
            }

            _context.SaveChanges();
            return count;
        }

        // ROI اصلاح (بند ۶۶)
        public decimal ComputeCorrectionRoi(decimal correctionCost, decimal expectedBenefit, decimal probability)
        {
            decimal adjusted = expectedBenefit * Clamp(probability, 0m, 1m);
            if (correctionCost <= 0m) return 0m;
            return Round((adjusted - correctionCost) / correctionCost * 100m);
        }

        // ==================================================================
        //  کمک‌ها
        // ==================================================================
        private static decimal SafeDiv(decimal a, decimal b) => b == 0m ? 0m : a / b;
        private static decimal Clamp(decimal v, decimal lo, decimal hi) => v < lo ? lo : (v > hi ? hi : v);
        private static decimal Round(decimal v, int digits = 2) => Math.Round(v, digits, MidpointRounding.AwayFromZero);
    }
}
