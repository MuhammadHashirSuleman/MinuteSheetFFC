using MinuteSheetFFC.Domain.Enums;

namespace MinuteSheetFFC.Domain.Entities;

public class WorkflowRule
{
    public int Id { get; set; }
    public int RequestTypeId { get; set; }
    public decimal BudgetFrom { get; set; }
    public decimal? BudgetTo { get; set; }
    public int RequiredManagerLevels { get; set; } = 3;
    public bool RequiresFinanceReview { get; set; }
    public FallbackBehavior FallbackBehavior { get; set; } = FallbackBehavior.UseParent;
    public bool IsActive { get; set; } = true;

    // Navigation
    public RequestType RequestType { get; set; } = null!;
}
