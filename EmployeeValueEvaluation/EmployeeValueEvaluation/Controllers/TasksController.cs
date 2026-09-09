using EmployeeValueEvaluation.Data;
using EmployeeValueEvaluation.Models;
using EmployeeValueEvaluation.Services;
using EmployeeValueEvaluation.Services.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModelTaskStatus = EmployeeValueEvaluation.Models.TaskStatus;

namespace EmployeeValueEvaluation.Controllers
{
    public class TasksController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IValueEvaluationService _eval;

        public TasksController(AppDbContext context, IValueEvaluationService eval)
        {
            _context = context;
            _eval = eval;
        }

        // GET: Tasks?employeeId=
        public async Task<IActionResult> Index(int? employeeId)
        {
            var query = _context.Tasks
                .Include(t => t.Employee)
                .Include(t => t.Evaluation)
                .AsQueryable();

            if (employeeId.HasValue)
                query = query.Where(t => t.EmployeeId == employeeId.Value);

            var tasks = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();

            ViewBag.Employees = await _context.Employees.OrderBy(e => e.Name).ToListAsync();
            ViewBag.SelectedEmployee = employeeId;
            return View(tasks);
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var task = await _context.Tasks
                .Include(t => t.Employee)
                .Include(t => t.Instruction)
                .Include(t => t.Evaluation)
                .Include(t => t.Cost)
                .Include(t => t.Benefit)
                .Include(t => t.Risks)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null) return NotFound();

            ViewBag.Computed = _eval.ComputeTask(task);
            return View(task);
        }

        // GET: Tasks/Create
        public async Task<IActionResult> Create(int? employeeId)
        {
            await PopulateEmployees(employeeId);
            return View(new TaskFormViewModel { EmployeeId = employeeId ?? 0, Status = 3 });
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateEmployees(vm.EmployeeId);
                return View(vm);
            }

            var task = new TaskItem
            {
                EmployeeId = vm.EmployeeId,
                Title = vm.Title,
                Description = vm.Description,
                CreatedAt = DateTime.Now,
                DueDate = vm.DueDate,
                CompletedAt = vm.CompletedAt,
                Priority = vm.Priority,
                BusinessImpact = vm.BusinessImpact,
                EstimatedHours = vm.EstimatedHours,
                ActualHours = vm.ActualHours,
                RequiredPath = vm.RequiredPath,
                ActualPath = vm.ActualPath,
                AlternativePathApproved = vm.AlternativePathApproved,
                Status = (ModelTaskStatus)vm.Status
            };

            task.Instruction = new TaskInstruction
            {
                WhatRequired = vm.WhatRequired,
                RequiredPath = vm.RequiredPath,
                AcceptanceCriteria = vm.AcceptanceCriteria,
                AlternativeAllowed = true,
                ApprovalRequired = true,
                ApprovedAlternative = vm.AlternativePathApproved ? vm.ActualPath : null
            };

            task.Cost = new TaskCost
            {
                ManagerHoursCost = vm.ManagerHoursCost,
                ReworkCost = vm.ReworkCost,
                DelayCost = vm.DelayCost,
                OpportunityCost = vm.OpportunityCost,
                RiskCost = vm.RiskCost,
                ExternalCost = vm.ExternalCost,
                TotalCost = vm.ManagerHoursCost + vm.ReworkCost + vm.DelayCost +
                            vm.OpportunityCost + vm.RiskCost + vm.ExternalCost
            };

            decimal adjusted = (vm.ProductBenefit + vm.StrategicBenefit) *
                               (vm.Confidence <= 0m ? 1m : vm.Confidence);
            task.Benefit = new TaskBenefit
            {
                RevenueBenefit = vm.RevenueBenefit,
                SavingBenefit = vm.SavingBenefit,
                LossAvoidance = vm.LossAvoidance,
                ProductBenefit = vm.ProductBenefit,
                StrategicBenefit = vm.StrategicBenefit,
                Confidence = vm.Confidence <= 0m ? 1m : vm.Confidence,
                AdjustedBenefit = adjusted,
                TotalBenefit = vm.RevenueBenefit + vm.SavingBenefit + vm.LossAvoidance + adjusted
            };

            if (!string.IsNullOrWhiteSpace(vm.RiskType) && vm.RiskImpact > 0m)
            {
                task.Risks.Add(new TaskRisk
                {
                    RiskType = vm.RiskType!,
                    Probability = vm.RiskProbability,
                    Impact = vm.RiskImpact,
                    ExpectedLoss = vm.RiskProbability * vm.RiskImpact
                });
            }

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = task.Id });
        }

        // GET: Tasks/Evaluate/5
        public async Task<IActionResult> Evaluate(int? id)
        {
            if (id == null) return NotFound();

            var task = await _context.Tasks
                .Include(t => t.Employee)
                .Include(t => t.Evaluation)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null) return NotFound();

            var e = task.Evaluation;
            var vm = new TaskEvaluateViewModel
            {
                TaskId = task.Id,
                TaskTitle = task.Title,
                EmployeeName = task.Employee?.Name ?? string.Empty,
                ComplianceScore = e?.ComplianceScore ?? 100,
                QualityScore = e?.QualityScore ?? 100,
                TimelinessScore = e?.TimelinessScore ?? 100,
                ProblemSolvingScore = e?.ProblemSolvingScore ?? 70,
                DocumentationScore = e?.DocumentationScore ?? 80,
                CommunicationScore = e?.CommunicationScore ?? 80,
                UnauthorizedDeviationScore = e?.UnauthorizedDeviationScore ?? 0,
                ReworkHours = e?.ReworkHours ?? 0,
                BugCount = e?.BugCount ?? 0,
                CriticalBugCount = e?.CriticalBugCount ?? 0,
                EvaluatorComment = e?.EvaluatorComment,
                MarkCompleted = true
            };

            return View(vm);
        }

        // POST: Tasks/Evaluate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Evaluate(TaskEvaluateViewModel vm)
        {
            var task = await _context.Tasks
                .Include(t => t.Evaluation)
                .FirstOrDefaultAsync(t => t.Id == vm.TaskId);

            if (task == null) return NotFound();

            if (!ModelState.IsValid)
                return View(vm);

            var e = task.Evaluation ?? new TaskEvaluation { TaskItemId = task.Id };

            e.ComplianceScore = vm.ComplianceScore;
            e.QualityScore = vm.QualityScore;
            e.TimelinessScore = vm.TimelinessScore;
            e.ProblemSolvingScore = vm.ProblemSolvingScore;
            e.DocumentationScore = vm.DocumentationScore;
            e.CommunicationScore = vm.CommunicationScore;
            e.UnauthorizedDeviationScore = vm.UnauthorizedDeviationScore;
            e.ReworkHours = vm.ReworkHours;
            e.BugCount = vm.BugCount;
            e.CriticalBugCount = vm.CriticalBugCount;
            e.EvaluatorComment = vm.EvaluatorComment;
            e.EvaluatedAt = DateTime.Now;

            // بهره‌وری زمانی و امتیاز نهایی Task را از موتور می‌گیریم
            e.EfficiencyScore = task.ActualHours > 0m && task.EstimatedHours > 0m
                ? Math.Round(Math.Min(100m, task.EstimatedHours / task.ActualHours * 100m), 2)
                : 100m;

            if (task.Evaluation == null)
                task.Evaluation = e;

            if (vm.MarkCompleted)
            {
                task.Status = ModelTaskStatus.Completed;
                task.CompletedAt ??= DateTime.Now;
            }

            // ثبت امتیاز نهایی Task
            var computed = _eval.ComputeTask(task);
            e.TaskScore = computed.TaskScore;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = task.Id });
        }

        private async Task PopulateEmployees(int? selected)
        {
            var employees = await _context.Employees
                .Where(e => e.IsActive)
                .OrderBy(e => e.Name)
                .ToListAsync();

            ViewBag.EmployeeList = new SelectList(employees, "Id", "Name", selected);
        }
    }
}
