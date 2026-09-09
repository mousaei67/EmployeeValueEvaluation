using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeValueEvaluation.Migrations
{
    /// <inheritdoc />
    public partial class InitialEmployeeEvaluation2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PersonnelCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BaseSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CorrectionPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Problem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TargetScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CorrectionCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpectedBenefit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Probability = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdjustedBenefit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CorrectionROI = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorrectionPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorrectionPlans_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyEvaluations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalBenefit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ROI = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BCR = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SVR = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Compliance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quality = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Productivity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Timeliness = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProblemSolving = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Documentation = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Communication = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Performance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RiskScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EconomicScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RiskFactor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FinalValueScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FairSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PotentialValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValueGap = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Decision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlyEvaluations_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalaryHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalaryHistories_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    BusinessImpact = table.Column<int>(type: "int", nullable: false),
                    EstimatedHours = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ActualHours = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RequiredPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActualPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PathApprovalRequired = table.Column<bool>(type: "bit", nullable: false),
                    AlternativePathApproved = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskBenefits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskItemId = table.Column<int>(type: "int", nullable: false),
                    RevenueBenefit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SavingBenefit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LossAvoidance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProductBenefit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StrategicBenefit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Confidence = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: false),
                    AdjustedBenefit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalBenefit = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskBenefits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskBenefits_Tasks_TaskItemId",
                        column: x => x.TaskItemId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskCosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskItemId = table.Column<int>(type: "int", nullable: false),
                    EmployeeHoursCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ManagerHoursCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReworkCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DelayCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OpportunityCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RiskCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExternalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskCosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskCosts_Tasks_TaskItemId",
                        column: x => x.TaskItemId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskEvaluations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskItemId = table.Column<int>(type: "int", nullable: false),
                    ComplianceScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    QualityScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    EfficiencyScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    TimelinessScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ProblemSolvingScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    DocumentationScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CommunicationScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    UnauthorizedDeviationScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    TaskScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    BugCount = table.Column<int>(type: "int", nullable: false),
                    CriticalBugCount = table.Column<int>(type: "int", nullable: false),
                    ReworkHours = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EvaluatorComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvaluatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskEvaluations_Tasks_TaskItemId",
                        column: x => x.TaskItemId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskInstructions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskItemId = table.Column<int>(type: "int", nullable: false),
                    WhatRequired = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RequiredPath = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    AcceptanceCriteria = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    AlternativeAllowed = table.Column<bool>(type: "bit", nullable: false),
                    ApprovalRequired = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedAlternative = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskInstructions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskInstructions_Tasks_TaskItemId",
                        column: x => x.TaskItemId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskRisks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskItemId = table.Column<int>(type: "int", nullable: false),
                    RiskType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Probability = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: false),
                    Impact = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpectedLoss = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Mitigation = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MitigationCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskRisks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskRisks_Tasks_TaskItemId",
                        column: x => x.TaskItemId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorrectionPlans_EmployeeId",
                table: "CorrectionPlans",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PersonnelCode",
                table: "Employees",
                column: "PersonnelCode",
                unique: true,
                filter: "[PersonnelCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyEvaluations_EmployeeId_Year_Month",
                table: "MonthlyEvaluations",
                columns: new[] { "EmployeeId", "Year", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalaryHistories_EmployeeId",
                table: "SalaryHistories",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskBenefits_TaskItemId",
                table: "TaskBenefits",
                column: "TaskItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskCosts_TaskItemId",
                table: "TaskCosts",
                column: "TaskItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskEvaluations_TaskItemId",
                table: "TaskEvaluations",
                column: "TaskItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskInstructions_TaskItemId",
                table: "TaskInstructions",
                column: "TaskItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskRisks_TaskItemId",
                table: "TaskRisks",
                column: "TaskItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_EmployeeId",
                table: "Tasks",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorrectionPlans");

            migrationBuilder.DropTable(
                name: "MonthlyEvaluations");

            migrationBuilder.DropTable(
                name: "SalaryHistories");

            migrationBuilder.DropTable(
                name: "TaskBenefits");

            migrationBuilder.DropTable(
                name: "TaskCosts");

            migrationBuilder.DropTable(
                name: "TaskEvaluations");

            migrationBuilder.DropTable(
                name: "TaskInstructions");

            migrationBuilder.DropTable(
                name: "TaskRisks");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
