namespace MinuteSheetFFC.Web.Services;

public interface IEmployeeService
{
    Task<EmployeeProfile> GetCurrentEmployeeAsync();
    Task<IReadOnlyList<EmployeeProfile>> GetEmployeesAsync();
    Task<EmployeeProfile?> GetEmployeeByPNoAsync(string pNo);
}

public interface IMinuteSheetService
{
    Task<IReadOnlyList<MinuteSheetRecord>> GetMinuteSheetsAsync();
    Task<IReadOnlyList<MinuteSheetRecord>> GetMyPendingActionsAsync(string pNo);
    Task<IReadOnlyList<MinuteSheetRecord>> GetSharedMinuteSheetsAsync(string pNo);
    Task<MinuteSheetRecord?> GetMinuteSheetAsync(int id);
    Task<MinuteSheetRecord> SaveDraftAsync(CreateMinuteSheetModel model, IReadOnlyList<MinuteSheetAttachment> attachments);
    Task<MinuteSheetRecord> SubmitAsync(CreateMinuteSheetModel model, IReadOnlyList<MinuteSheetAttachment> attachments);
    Task ApplyDecisionAsync(int id, string action, string remarks, string actionerPNo);
}

public interface IWorkflowService
{
    Task<IReadOnlyList<WorkflowStage>> GetTimelineAsync(int minuteSheetId);
    Task<bool> CanCurrentUserActAsync(int minuteSheetId, string pNo);
}

public interface IAiService
{
    Task<string> AnalyzeMinuteSheetAsync(CreateMinuteSheetModel model);
    Task<IReadOnlyList<string>> PreviewRouteAsync(CreateMinuteSheetModel model);
}

public interface IDashboardService
{
    Task<DashboardSnapshot> GetDashboardAsync(string pNo);
}

public sealed class MockMinuteSheetDataStore
{
    private readonly List<EmployeeProfile> _employees;
    private readonly List<MinuteSheetRecord> _minuteSheets;
    private int _nextId = 1000;

    public MockMinuteSheetDataStore()
    {
        _employees = new List<EmployeeProfile>
        {
            new() { PNo = "P1001", FullName = "Ahsan Iqbal", Department = "Finance Department", Designation = "Admin Officer", Initials = "AI", AvatarUrl = "assets/avatars/avatar-1.jpg" },
            new() { PNo = "P1002", FullName = "Sarah Mitchell", Department = "Information Technology", Designation = "Requester", Initials = "SM", AvatarUrl = "assets/avatars/avatar-2.jpg" },
            new() { PNo = "P1003", FullName = "David Chen", Department = "IT Operations", Designation = "Director", Initials = "DC", AvatarUrl = "assets/avatars/avatar-3.jpg" },
            new() { PNo = "P1004", FullName = "Sarah Jenkins", Department = "Finance", Designation = "Finance Controller", Initials = "SJ", AvatarUrl = "assets/avatars/avatar-4.jpg" },
            new() { PNo = "P1005", FullName = "Omar Khalid", Department = "Procurement", Designation = "Procurement Lead", Initials = "OK", AvatarUrl = "assets/avatars/avatar-5.jpg" },
            new() { PNo = "P1006", FullName = "Robert Vance", Department = "Finance", Designation = "Chief Financial Officer", Initials = "RV", AvatarUrl = "assets/avatars/avatar-6.jpg" },
            new() { PNo = "P1007", FullName = "Marketing Internal", Department = "Marketing", Designation = "Department Group", Initials = "MI", AvatarUrl = string.Empty },
            new() { PNo = "P1008", FullName = "HR Admin", Department = "Human Resources", Designation = "HR Administrator", Initials = "HR", AvatarUrl = "assets/avatars/avatar-7.jpg" }
        };

        _minuteSheets = new List<MinuteSheetRecord>
        {
            BuildDetailRecord(),
            new()
            {
                Id = 1,
                MinuteSheetId = "#MS-2024-001",
                ReferenceNo = "FFC/IT/2024/OCT-0001",
                Type = "Financial",
                Subject = "Annual IT Infrastructure Upgrade - Phase 1 Funding",
                CreationDate = new DateTime(2024, 10, 24),
                Status = MinuteSheetStatus.InReview,
                CurrentActionerPNo = "P1004",
                CurrentActionerName = "Sarah Jenkins",
                RequesterPNo = "P1002",
                RequesterName = "Sarah Mitchell",
                RequesterDepartment = "Information Technology",
                Priority = MinuteSheetPriority.High,
                EstimatedBudget = 78500,
                WorkflowMode = WorkflowMode.Hybrid,
                DescriptionHtml = "<p>Funding request for the first phase of infrastructure modernization.</p>",
                SharedWithCurrentUser = true
            },
            new()
            {
                Id = 2,
                MinuteSheetId = "#MS-2024-002",
                ReferenceNo = "FFC/HR/2024/OCT-0002",
                Type = "Non-Financial",
                Subject = "Policy Update: remote work guidelines for Q1",
                CreationDate = new DateTime(2024, 10, 22),
                Status = MinuteSheetStatus.Approved,
                CurrentActionerPNo = "P1003",
                CurrentActionerName = "David Chen",
                RequesterPNo = "P1008",
                RequesterName = "HR Admin",
                RequesterDepartment = "Human Resources",
                Priority = MinuteSheetPriority.Medium,
                WorkflowMode = WorkflowMode.Manual,
                DescriptionHtml = "<p>Approval record for revised remote work governance.</p>",
                SharedWithCurrentUser = true
            },
            new()
            {
                Id = 3,
                MinuteSheetId = "#MS-2024-003",
                ReferenceNo = "FFC/MKT/2024/OCT-0003",
                Type = "Financial",
                Subject = "Quarterly Marketing Budget Allocation - APAC Region",
                CreationDate = new DateTime(2024, 10, 20),
                Status = MinuteSheetStatus.Submitted,
                CurrentActionerPNo = "P1001",
                CurrentActionerName = "Ahsan Iqbal",
                RequesterPNo = "P1007",
                RequesterName = "Marketing Internal",
                RequesterDepartment = "Marketing",
                Priority = MinuteSheetPriority.Medium,
                EstimatedBudget = 42000,
                WorkflowMode = WorkflowMode.Dynamic,
                DescriptionHtml = "<p>Budget allocation request routed to Finance for initial review.</p>",
                SharedWithCurrentUser = true
            },
            new()
            {
                Id = 4,
                MinuteSheetId = "#MS-2024-004",
                ReferenceNo = "FFC/TRV/2024/OCT-0004",
                Type = "Financial",
                Subject = "Travel Expense Reimbursement - Board Meeting NY",
                CreationDate = new DateTime(2024, 10, 18),
                Status = MinuteSheetStatus.Rejected,
                CurrentActionerPNo = "P1005",
                CurrentActionerName = "Omar Khalid",
                RequesterPNo = "P1001",
                RequesterName = "Ahsan Iqbal",
                RequesterDepartment = "Finance Department",
                Priority = MinuteSheetPriority.Low,
                EstimatedBudget = 6400,
                WorkflowMode = WorkflowMode.Manual,
                DescriptionHtml = "<p>Travel reimbursement request declined due to incomplete documentation.</p>"
            },
            new()
            {
                Id = 5,
                MinuteSheetId = "#MS-2024-005",
                ReferenceNo = "FFC/CSR/2024/OCT-0005",
                Type = "Non-Financial",
                Subject = "CSR Initiative Proposal: Green Office 2024",
                CreationDate = new DateTime(2024, 10, 15),
                Status = MinuteSheetStatus.Draft,
                CurrentActionerPNo = string.Empty,
                CurrentActionerName = "None Assigned",
                RequesterPNo = "P1001",
                RequesterName = "Ahsan Iqbal",
                RequesterDepartment = "Finance Department",
                Priority = MinuteSheetPriority.Low,
                WorkflowMode = WorkflowMode.Manual,
                DescriptionHtml = "<p>Draft proposal for a company-wide sustainability program.</p>"
            },
            new()
            {
                Id = 6,
                MinuteSheetId = "#MS-2024-006",
                ReferenceNo = "FFC/FIN/2024/OCT-0006",
                Type = "Financial",
                Subject = "Vendor Payment Escalation for Turnaround Maintenance",
                CreationDate = new DateTime(2024, 10, 10),
                Status = MinuteSheetStatus.Returned,
                CurrentActionerPNo = "P1001",
                CurrentActionerName = "Ahsan Iqbal",
                RequesterPNo = "P1005",
                RequesterName = "Omar Khalid",
                RequesterDepartment = "Procurement",
                Priority = MinuteSheetPriority.Urgent,
                EstimatedBudget = 118000,
                WorkflowMode = WorkflowMode.Hybrid,
                DescriptionHtml = "<p>Returned for missing vendor tax compliance details.</p>",
                DaysDelayed = 5
            },
            new()
            {
                Id = 7,
                MinuteSheetId = "#MS-2024-007",
                ReferenceNo = "FFC/OPS/2024/OCT-0007",
                Type = "Non-Financial",
                Subject = "Plant Safety Drill Schedule Revision",
                CreationDate = new DateTime(2024, 10, 8),
                Status = MinuteSheetStatus.Cancelled,
                CurrentActionerPNo = string.Empty,
                CurrentActionerName = "None Assigned",
                RequesterPNo = "P1003",
                RequesterName = "David Chen",
                RequesterDepartment = "IT Operations",
                Priority = MinuteSheetPriority.Medium,
                WorkflowMode = WorkflowMode.Manual,
                DescriptionHtml = "<p>Cancelled after revised calendar alignment.</p>"
            },
            new()
            {
                Id = 8,
                MinuteSheetId = "#MS-2024-008",
                ReferenceNo = "FFC/LEGAL/2024/OCT-0008",
                Type = "Legal & Compliance",
                Subject = "Contract Renewal Compliance Note",
                CreationDate = new DateTime(2024, 10, 5),
                Status = MinuteSheetStatus.Resubmitted,
                CurrentActionerPNo = "P1001",
                CurrentActionerName = "Ahsan Iqbal",
                RequesterPNo = "P1008",
                RequesterName = "HR Admin",
                RequesterDepartment = "Human Resources",
                Priority = MinuteSheetPriority.High,
                WorkflowMode = WorkflowMode.Dynamic,
                DescriptionHtml = "<p>Resubmitted after legal clause clarification.</p>"
            },
            new()
            {
                Id = 9,
                MinuteSheetId = "#MS-2024-009",
                ReferenceNo = "FFC/INFO/2024/OCT-0009",
                Type = "Non-Financial",
                Subject = "FYI: Updated Department Delegation Matrix",
                CreationDate = new DateTime(2024, 10, 2),
                Status = MinuteSheetStatus.Marked,
                CurrentActionerPNo = string.Empty,
                CurrentActionerName = "Informational",
                RequesterPNo = "P1006",
                RequesterName = "Robert Vance",
                RequesterDepartment = "Finance",
                Priority = MinuteSheetPriority.Low,
                WorkflowMode = WorkflowMode.Manual,
                DescriptionHtml = "<p>Marked for information only; no approval action required.</p>",
                SharedWithCurrentUser = true
            }
        };

        foreach (var sheet in _minuteSheets.Where(sheet => sheet.WorkflowStages.Count == 0))
        {
            SeedSimpleWorkflow(sheet);
        }
    }

    public EmployeeProfile CurrentEmployee => _employees[0];

    public IReadOnlyList<EmployeeProfile> Employees => _employees;

    public IReadOnlyList<MinuteSheetRecord> MinuteSheets => _minuteSheets;

    public int NextId() => _nextId++;

    public void Add(MinuteSheetRecord record) => _minuteSheets.Insert(0, record);

    private static MinuteSheetRecord BuildDetailRecord()
    {
        var record = new MinuteSheetRecord
        {
            Id = 892,
            MinuteSheetId = "MS-2024-0892",
            ReferenceNo = "FFC/IT/2024/AUG-0892",
            Type = "Capital Expenditure (CAPEX)",
            Subject = "Annual Infrastructure Tech Refresh 2024",
            CreationDate = new DateTime(2024, 8, 14),
            Status = MinuteSheetStatus.InReview,
            CurrentActionerPNo = "P1001",
            CurrentActionerName = "Ahsan Iqbal",
            RequesterPNo = "P1002",
            RequesterName = "Sarah Mitchell",
            RequesterDepartment = "Information Technology",
            Priority = MinuteSheetPriority.High,
            EstimatedBudget = 142500,
            Confidential = true,
            WorkflowMode = WorkflowMode.Hybrid,
            DescriptionHtml = """
                <h3>Executive Summary</h3>
                <p>This minute sheet proposes the phased replacement of core server infrastructure at the regional data centers. The existing hardware has reached its 5-year end-of-life cycle and is experiencing a 15% increase in maintenance overhead.</p>
                <p>The requested budget covers the procurement of 12 high-density compute nodes, associated networking hardware, and the professional services required for migration. Implementation is planned for Q4 2024 to avoid disruption during year-end financial closing.</p>
                <ul>
                    <li>Modernization of 4 primary data clusters.</li>
                    <li>Reduction in energy consumption by 22% annually.</li>
                    <li>Enhanced hardware-level security protocols for enterprise data governance.</li>
                </ul>
                """,
            SharedWithCurrentUser = true,
            DaysDelayed = 12
        };

        record.Attachments.AddRange(new[]
        {
            new MinuteSheetAttachment { FileName = "Technical_Specifications.pdf", SizeLabel = "2.4 MB", UploadedAt = new DateTimeOffset(2024, 8, 14, 9, 55, 0, TimeSpan.Zero) },
            new MinuteSheetAttachment { FileName = "Budget_Breakdown_v2.xlsx", SizeLabel = "840 KB", UploadedAt = new DateTimeOffset(2024, 8, 14, 9, 58, 0, TimeSpan.Zero) },
            new MinuteSheetAttachment { FileName = "Vendor_Quote_DELL.docx", SizeLabel = "1.1 MB", UploadedAt = new DateTimeOffset(2024, 8, 14, 10, 1, 0, TimeSpan.Zero) }
        });

        record.WorkflowStages.AddRange(new[]
        {
            new WorkflowStage
            {
                Order = 1,
                ActionerPNo = "P1002",
                ActionerName = "Sarah Mitchell",
                Role = "Requester",
                Department = "IT Dept",
                ActionType = "Submit",
                Status = MinuteSheetStatus.Submitted,
                Timestamp = new DateTimeOffset(2024, 8, 14, 10, 20, 0, TimeSpan.Zero)
            },
            new WorkflowStage
            {
                Order = 2,
                ActionerPNo = "P1003",
                ActionerName = "David Chen",
                Role = "Director",
                Department = "IT Operations",
                ActionType = "Verify",
                Status = MinuteSheetStatus.Approved,
                Remarks = "Infrastructure refresh is critical for system stability. Fully support this procurement.",
                Timestamp = new DateTimeOffset(2024, 8, 15, 14, 45, 0, TimeSpan.Zero)
            },
            new WorkflowStage
            {
                Order = 3,
                ActionerPNo = "P1001",
                ActionerName = "Ahsan Iqbal",
                Role = "Executive VP",
                Department = "Finance",
                ActionType = "Approve",
                Status = MinuteSheetStatus.InReview,
                IsCurrent = true
            },
            new WorkflowStage
            {
                Order = 4,
                ActionerPNo = "P1006",
                ActionerName = "Robert Vance",
                Role = "Chief Financial Officer",
                Department = "Finance",
                ActionType = "Approve",
                Status = MinuteSheetStatus.Draft,
                IsUpcoming = true
            }
        });

        return record;
    }

    private void SeedSimpleWorkflow(MinuteSheetRecord record)
    {
        record.WorkflowStages.Add(new WorkflowStage
        {
            Order = 1,
            ActionerPNo = record.RequesterPNo,
            ActionerName = record.RequesterName,
            Role = "Requester",
            Department = record.RequesterDepartment,
            ActionType = "Submit",
            Status = record.Status == MinuteSheetStatus.Draft ? MinuteSheetStatus.Draft : MinuteSheetStatus.Submitted,
            Timestamp = record.CreationDate
        });

        if (!string.IsNullOrWhiteSpace(record.CurrentActionerPNo))
        {
            record.WorkflowStages.Add(new WorkflowStage
            {
                Order = 2,
                ActionerPNo = record.CurrentActionerPNo,
                ActionerName = record.CurrentActionerName,
                Role = "Current Actioner",
                Department = "Workflow",
                ActionType = "Review",
                Status = record.Status,
                IsCurrent = record.Status is MinuteSheetStatus.Submitted or MinuteSheetStatus.InReview or MinuteSheetStatus.Returned or MinuteSheetStatus.Resubmitted
            });
        }
    }
}

public sealed class InMemoryEmployeeService(MockMinuteSheetDataStore store) : IEmployeeService
{
    public Task<EmployeeProfile> GetCurrentEmployeeAsync() => Task.FromResult(store.CurrentEmployee);

    public Task<IReadOnlyList<EmployeeProfile>> GetEmployeesAsync() => Task.FromResult(store.Employees);

    public Task<EmployeeProfile?> GetEmployeeByPNoAsync(string pNo) =>
        Task.FromResult(store.Employees.FirstOrDefault(employee => employee.PNo == pNo));
}

public sealed class InMemoryMinuteSheetService(MockMinuteSheetDataStore store) : IMinuteSheetService
{
    public Task<IReadOnlyList<MinuteSheetRecord>> GetMinuteSheetsAsync() =>
        Task.FromResult<IReadOnlyList<MinuteSheetRecord>>(store.MinuteSheets
            .OrderByDescending(sheet => sheet.CreationDate)
            .ToList());

    public Task<IReadOnlyList<MinuteSheetRecord>> GetMyPendingActionsAsync(string pNo) =>
        Task.FromResult<IReadOnlyList<MinuteSheetRecord>>(store.MinuteSheets
            .Where(sheet => sheet.CurrentActionerPNo == pNo)
            .Where(sheet => sheet.Status is MinuteSheetStatus.Submitted or MinuteSheetStatus.InReview or MinuteSheetStatus.Returned or MinuteSheetStatus.Resubmitted)
            .OrderByDescending(sheet => sheet.Priority)
            .ThenBy(sheet => sheet.CreationDate)
            .ToList());

    public Task<IReadOnlyList<MinuteSheetRecord>> GetSharedMinuteSheetsAsync(string pNo) =>
        Task.FromResult<IReadOnlyList<MinuteSheetRecord>>(store.MinuteSheets
            .Where(sheet => sheet.SharedWithCurrentUser || sheet.Status == MinuteSheetStatus.Marked)
            .OrderByDescending(sheet => sheet.CreationDate)
            .ToList());

    public Task<MinuteSheetRecord?> GetMinuteSheetAsync(int id) =>
        Task.FromResult(store.MinuteSheets.FirstOrDefault(sheet => sheet.Id == id));

    public Task<MinuteSheetRecord> SaveDraftAsync(CreateMinuteSheetModel model, IReadOnlyList<MinuteSheetAttachment> attachments) =>
        CreateMinuteSheetAsync(model, attachments, MinuteSheetStatus.Draft);

    public Task<MinuteSheetRecord> SubmitAsync(CreateMinuteSheetModel model, IReadOnlyList<MinuteSheetAttachment> attachments) =>
        CreateMinuteSheetAsync(model, attachments, MinuteSheetStatus.Submitted);

    public Task ApplyDecisionAsync(int id, string action, string remarks, string actionerPNo)
    {
        var sheet = store.MinuteSheets.FirstOrDefault(item => item.Id == id);
        if (sheet is null)
        {
            return Task.CompletedTask;
        }

        var currentStage = sheet.WorkflowStages.FirstOrDefault(stage => stage.ActionerPNo == actionerPNo && stage.IsCurrent);
        if (currentStage is not null)
        {
            currentStage.Remarks = remarks;
            currentStage.Timestamp = DateTimeOffset.Now;
            currentStage.IsCurrent = false;
            currentStage.Status = action switch
            {
                "Approve" => MinuteSheetStatus.Approved,
                "Reject" => MinuteSheetStatus.Rejected,
                "Return" => MinuteSheetStatus.Returned,
                _ => currentStage.Status
            };
        }

        sheet.Status = action switch
        {
            "Approve" => MinuteSheetStatus.Approved,
            "Reject" => MinuteSheetStatus.Rejected,
            "Return" => MinuteSheetStatus.Returned,
            _ => sheet.Status
        };

        sheet.CurrentActionerPNo = string.Empty;
        sheet.CurrentActionerName = action == "Approve" ? "Completed" : "Returned to requester";

        return Task.CompletedTask;
    }

    private Task<MinuteSheetRecord> CreateMinuteSheetAsync(CreateMinuteSheetModel model, IReadOnlyList<MinuteSheetAttachment> attachments, MinuteSheetStatus status)
    {
        var id = store.NextId();
        var current = store.CurrentEmployee;
        var highBudget = model.EstimatedBudget > 100000;
        var nextActionerPNo = highBudget ? "P1006" : "P1003";
        var nextActionerName = highBudget ? "Robert Vance" : "David Chen";
        var nextActionerRole = highBudget ? "Chief Financial Officer" : "Director";
        var nextActionerDepartment = highBudget ? "Finance" : current.Department;

        var record = new MinuteSheetRecord
        {
            Id = id,
            MinuteSheetId = $"#MS-2024-{id}",
            ReferenceNo = $"FFC/{ReferenceType(model.Type)}/2024/{id}",
            Type = model.Type,
            Subject = model.Subject,
            CreationDate = DateTime.Today,
            Status = status,
            CurrentActionerPNo = status == MinuteSheetStatus.Draft ? string.Empty : nextActionerPNo,
            CurrentActionerName = status == MinuteSheetStatus.Draft ? "None Assigned" : nextActionerName,
            RequesterPNo = current.PNo,
            RequesterName = current.FullName,
            RequesterDepartment = current.Department,
            Priority = model.Priority,
            EstimatedBudget = model.EstimatedBudget,
            Confidential = model.Confidential,
            WorkflowMode = model.WorkflowMode,
            DescriptionHtml = model.DescriptionHtml,
            SharedWithCurrentUser = true
        };

        record.Attachments.AddRange(attachments);
        record.WorkflowStages.Add(new WorkflowStage
        {
            Order = 1,
            ActionerPNo = current.PNo,
            ActionerName = current.FullName,
            Role = current.Designation,
            Department = current.Department,
            ActionType = status == MinuteSheetStatus.Draft ? "Save Draft" : "Submit",
            Status = status,
            Timestamp = DateTimeOffset.Now
        });

        if (status != MinuteSheetStatus.Draft)
        {
            record.WorkflowStages.Add(new WorkflowStage
            {
                Order = 2,
                ActionerPNo = nextActionerPNo,
                ActionerName = nextActionerName,
                Role = nextActionerRole,
                Department = nextActionerDepartment,
                ActionType = "Review",
                Status = MinuteSheetStatus.InReview,
                IsCurrent = true
            });
        }

        store.Add(record);
        return Task.FromResult(record);
    }

    private static string ReferenceType(string type) =>
        type switch
        {
            "Financial" or "Financial & Procurement" => "FIN",
            "IT Services" => "IT",
            "Legal & Compliance" => "LEGAL",
            "Administrative" => "ADMIN",
            _ => "HR"
        };
}

public sealed class InMemoryWorkflowService(MockMinuteSheetDataStore store) : IWorkflowService
{
    public Task<IReadOnlyList<WorkflowStage>> GetTimelineAsync(int minuteSheetId)
    {
        var sheet = store.MinuteSheets.FirstOrDefault(item => item.Id == minuteSheetId);
        return Task.FromResult<IReadOnlyList<WorkflowStage>>(sheet?.WorkflowStages.OrderBy(stage => stage.Order).ToList() ?? new List<WorkflowStage>());
    }

    public Task<bool> CanCurrentUserActAsync(int minuteSheetId, string pNo)
    {
        var sheet = store.MinuteSheets.FirstOrDefault(item => item.Id == minuteSheetId);
        return Task.FromResult(sheet?.CurrentActionerPNo == pNo);
    }
}

public sealed class InMemoryAiService : IAiService
{
    public Task<string> AnalyzeMinuteSheetAsync(CreateMinuteSheetModel model)
    {
        var budgetNote = CreateMinuteSheetModel.IsFinancialType(model.Type)
            ? "Budget-sensitive request; route through Finance before final approval."
            : "No budget review required; standard administrative routing is sufficient.";

        return Task.FromResult($"{model.Priority} priority detected. {budgetNote}");
    }

    public Task<IReadOnlyList<string>> PreviewRouteAsync(CreateMinuteSheetModel model)
    {
        IReadOnlyList<string> route = model.WorkflowMode switch
        {
            WorkflowMode.Dynamic when CreateMinuteSheetModel.IsFinancialType(model.Type) => new[] { "Requester", "Finance Controller", "Executive VP Finance", "CFO" },
            WorkflowMode.Dynamic => new[] { "Requester", "Department Head", "Admin Officer" },
            WorkflowMode.Hybrid => new[] { "Requester", "Department Head", "Finance Review", "Custom Approver" },
            _ => new[] { "Requester", "Manual Recipient", "Manual Approver" }
        };

        return Task.FromResult(route);
    }
}

public sealed class InMemoryDashboardService(IMinuteSheetService minuteSheetService) : IDashboardService
{
    public async Task<DashboardSnapshot> GetDashboardAsync(string pNo)
    {
        var all = await minuteSheetService.GetMinuteSheetsAsync();
        var pending = await minuteSheetService.GetMyPendingActionsAsync(pNo);

        return new DashboardSnapshot
        {
            MyRequests = all.Count(sheet => sheet.RequesterPNo == pNo),
            PendingActions = pending.Count,
            Drafts = all.Count(sheet => sheet.Status == MinuteSheetStatus.Draft),
            Approved = all.Count(sheet => sheet.Status == MinuteSheetStatus.Approved) + 142,
            PendingItems = pending.Take(3).ToList(),
            RecentItems = all.Where(sheet => sheet.RequesterPNo == pNo || sheet.SharedWithCurrentUser).Take(3).ToList(),
            DelayedItems = all.Where(sheet => sheet.DaysDelayed > 0).OrderByDescending(sheet => sheet.DaysDelayed).Take(3).ToList()
        };
    }
}
