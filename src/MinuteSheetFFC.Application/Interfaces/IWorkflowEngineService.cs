using MinuteSheetFFC.Application.DTOs.Common;
using MinuteSheetFFC.Application.DTOs.Workflow;

namespace MinuteSheetFFC.Application.Interfaces;

public interface IWorkflowEngineService
{
    Task<ApiResponse<RoutePreviewDto>> GenerateRoutePreviewAsync(int minuteSheetId);
    Task<ApiResponse<WorkflowActionResultDto>> ProcessActionAsync(int minuteSheetId, WorkflowActionDto action);
    Task<ApiResponse<List<PendingActionDto>>> GetPendingActionsAsync(string actionerPNo);
}
