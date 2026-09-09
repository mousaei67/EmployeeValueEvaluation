using System.ComponentModel.DataAnnotations;

namespace EmployeeValueEvaluation.Models
{
    public class TaskInstruction
    {
        public int Id { get; set; }

        public int TaskItemId { get; set; }

        [StringLength(2000)]
        public string? WhatRequired { get; set; }

        [StringLength(3000)]
        public string? RequiredPath { get; set; }

        [StringLength(3000)]
        public string? AcceptanceCriteria { get; set; }

        public bool AlternativeAllowed { get; set; }

        public bool ApprovalRequired { get; set; }

        [StringLength(3000)]
        public string? ApprovedAlternative { get; set; }

        public TaskItem TaskItem { get; set; } = null!;
    }
}