using MinuteSheetFFC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MinuteSheetFFC.WorkflowEngine.Services;

public class ManagerChainItem
{
    public string PNo { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Designation { get; set; }
    public string? DepartmentName { get; set; }
    public int Level { get; set; }
    public bool IsActive { get; set; }
}

public class ManagerChainResult
{
    public List<ManagerChainItem> Chain { get; set; } = new();
}

public class HierarchyResolver
{
    private readonly AppDbContext _db;

    public HierarchyResolver(AppDbContext db) => _db = db;

    public async Task<ManagerChainResult> ResolveManagerChainAsync(string employeePNo, int maxLevels = 10)
    {
        var result = new ManagerChainResult();
        var currentPNo = employeePNo;
        var visited = new HashSet<string>();
        int level = 0;

        while (!string.IsNullOrEmpty(currentPNo) && level < maxLevels)
        {
            if (visited.Contains(currentPNo)) break;
            visited.Add(currentPNo);

            var emp = await _db.Employees.FirstOrDefaultAsync(e => e.PNo == currentPNo);
            if (emp == null) break;

            if (currentPNo != employeePNo)
            {
                result.Chain.Add(new ManagerChainItem
                {
                    PNo = emp.PNo,
                    Name = emp.Name,
                    Designation = emp.Designation,
                    DepartmentName = emp.DepartmentName,
                    Level = level,
                    IsActive = emp.IsActive
                });
            }

            currentPNo = emp.ManagerPNo ?? "";
            level++;
        }

        return result;
    }
}
