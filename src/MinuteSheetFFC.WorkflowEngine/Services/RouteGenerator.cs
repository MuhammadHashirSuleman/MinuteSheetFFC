using Microsoft.EntityFrameworkCore;
using MinuteSheetFFC.Application.DTOs.Workflow;
using MinuteSheetFFC.Domain.Entities;
using MinuteSheetFFC.Domain.Enums;
using MinuteSheetFFC.Infrastructure.Data;

namespace MinuteSheetFFC.WorkflowEngine.Services;

public class RouteGenerator
{
    private readonly AppDbContext _db;
    private readonly HierarchyResolver _hierarchyResolver;

    public RouteGenerator(AppDbContext db, HierarchyResolver hierarchyResolver)
    {
        _db = db;
        _hierarchyResolver = hierarchyResolver;
    }

    public async Task<RoutePreviewDto> GenerateDynamicRouteAsync(MinuteSheetRequest request, WorkflowRule rule)
    {
        var preview = new RoutePreviewDto { IsValid = true };

        var chainResult = await _hierarchyResolver.ResolveManagerChainAsync(request.RequesterPNo, rule.RequiredManagerLevels);

        if (chainResult.Chain.Count == 0)
        {
            preview.Warnings.Add(new RouteWarningDto { Message = "No managers found in hierarchy chain.", Level = "Error" });
            preview.IsValid = false;
            return preview;
        }

        int stageOrder = 1;
        foreach (var manager in chainResult.Chain.Take(rule.RequiredManagerLevels))
        {
            if (!manager.IsActive)
            {
                if (rule.FallbackBehavior == FallbackBehavior.Skip)
                {
                    preview.Warnings.Add(new RouteWarningDto { Message = $"Skipping inactive manager {manager.Name} ({manager.PNo}).", Level = "Warning" });
                    continue;
                }
                else if (rule.FallbackBehavior == FallbackBehavior.Block)
                {
                    preview.Warnings.Add(new RouteWarningDto { Message = $"Blocked: Manager {manager.Name} ({manager.PNo}) is inactive.", Level = "Error" });
                    preview.IsValid = false;
                    return preview;
                }
            }

            var actionType = stageOrder == chainResult.Chain.Count || stageOrder == rule.RequiredManagerLevels
                ? "Approve" : "Recommend";

            preview.Steps.Add(new RouteStepDto
            {
                StageOrder = stageOrder,
                ActionerPNo = manager.PNo,
                ActionerName = manager.Name,
                ActionerDesignation = manager.Designation,
                ActionType = actionType,
                Source = "Auto"
            });
            stageOrder++;
        }

        if (rule.RequiresFinanceReview)
        {
            preview.Warnings.Add(new RouteWarningDto { Message = "Finance review stage will be added.", Level = "Info" });
        }

        if (preview.Steps.Count == 0)
        {
            preview.IsValid = false;
            preview.Warnings.Add(new RouteWarningDto { Message = "No valid route steps could be generated.", Level = "Error" });
        }

        return preview;
    }
}
