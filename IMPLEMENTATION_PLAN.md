# Minute Sheet / General Approval Workflow - Implementation Plan

## Table of Contents
1. [Solution Architecture](#1-solution-architecture)
2. [Project Structure](#2-project-structure)
3. [Technology Stack](#3-technology-stack)
4. [Database Design](#4-database-design)
5. [Domain Models](#5-domain-models)
6. [Backend API Design](#6-backend-api-design)
7. [Workflow Engine Design](#7-workflow-engine-design)
8. [AI Service Architecture](#8-ai-service-architecture)
9. [Frontend Blazor Design](#9-frontend-blazor-design)
10. [Authentication & Authorization (Simulated)](#10-authentication--authorization-simulated)
11. [Testing Strategy](#11-testing-strategy)
12. [Implementation Milestones](#12-implementation-milestones)
13. [Folder/File Map](#13-folderfile-map)

---

## 1. Solution Architecture

### Architecture Pattern: Clean Architecture + CQRS-Lite

```
+------------------------------------------------------------------+
|                     Blazor Server Frontend                        |
|          (Components, Pages, Services, State Management)          |
+------------------------------------------------------------------+
          |                    |                    |
          v                    v                    v
+------------------+  +------------------+  +------------------+
|   HTTP Clients   |  |  SignalR (Live)   |  | JS Interop (UI) |
+------------------+  +------------------+  +------------------+
          |                    |
          v                    v
+------------------------------------------------------------------+
|                    ASP.NET Web API Backend                         |
|              (Controllers, Middleware, Filters)                    |
+------------------------------------------------------------------+
          |                    |                    |
          v                    v                    v
+------------------+  +------------------+  +------------------+
| Application Layer|  | Workflow Engine  |  |   AI Service     |
| (Services, DTOs) |  | (Route Resolver) |  | (Mock/Real)      |
+------------------+  +------------------+  +------------------+
          |                    |                    |
          v                    v                    v
+------------------------------------------------------------------+
|                      Domain Layer                                  |
|            (Entities, Enums, Value Objects, Interfaces)            |
+------------------------------------------------------------------+
          |
          v
+------------------------------------------------------------------+
|                   Infrastructure Layer                             |
|        (EF Core DbContext, Repositories, Migrations, Seed)        |
+------------------------------------------------------------------+
          |
          v
+------------------------------------------------------------------+
|                     SQL Server / SQLite                            |
+------------------------------------------------------------------+
```

### Communication Flow
- **Blazor Frontend** communicates with **Web API Backend** via HTTP (HttpClient)
- Backend uses **service layer** for business logic
- **Workflow Engine** is a dedicated service handling route resolution, stage generation, and action processing
- **AI Service** is abstracted behind an interface; mock implementation first, real integration later

---

## 2. Project Structure

### Solution: `MinuteSheetFFC.sln`

```
MinuteSheetFFC/
|
+-- src/
|   |
|   +-- MinuteSheetFFC.Domain/              (.NET Class Library)
|   |   Domain entities, enums, interfaces, value objects
|   |
|   +-- MinuteSheetFFC.Application/         (.NET Class Library)
|   |   DTOs, service interfaces, mapping profiles, validators
|   |
|   +-- MinuteSheetFFC.Infrastructure/      (.NET Class Library)
|   |   EF Core DbContext, migrations, repositories, seed data
|   |
|   +-- MinuteSheetFFC.WorkflowEngine/      (.NET Class Library)
|   |   Hierarchy resolver, route generator, stage manager, action processor
|   |
|   +-- MinuteSheetFFC.AiService/           (.NET Class Library)
|   |   IAiMinuteSheetService, mock implementation, analysis models
|   |
|   +-- MinuteSheetFFC.Api/                 (ASP.NET Web API)
|   |   Controllers, middleware, filters, Program.cs, appsettings
|   |
|   +-- MinuteSheetFFC.Web/                 (Blazor Server - existing project, restructured)
|       Components, pages, layouts, services (HTTP clients), state
|
+-- tests/
|   |
|   +-- MinuteSheetFFC.Domain.Tests/        (xUnit)
|   +-- MinuteSheetFFC.Application.Tests/   (xUnit)
|   +-- MinuteSheetFFC.WorkflowEngine.Tests/ (xUnit)
|   +-- MinuteSheetFFC.Api.Tests/           (xUnit + WebApplicationFactory)
|   +-- MinuteSheetFFC.Integration.Tests/   (xUnit)
|
+-- docs/
|   +-- ERD.md
|   +-- API_Reference.md
|   +-- User_Guide.md
|   +-- Setup_Guide.md
|   +-- Technical_Notes.md
|
+-- MinuteSheetFFC.sln
```

### Project References
```
Domain          -> (no dependencies)
Application     -> Domain
Infrastructure  -> Domain, Application
WorkflowEngine  -> Domain, Application
AiService       -> Domain, Application
Api             -> Application, Infrastructure, WorkflowEngine, AiService
Web             -> Application (DTOs/interfaces only; communicates via HTTP)
```

---

## 3. Technology Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| Runtime | .NET | 10.0 |
| Frontend | Blazor Server (Interactive SSR) | .NET 10 |
| Backend API | ASP.NET Core Web API | .NET 10 |
| ORM | Entity Framework Core | 10.x |
| Database | SQL Server LocalDB (dev) / SQLite (portable) | Latest |
| UI Framework | Bootstrap 5 + custom CSS | 5.3 |
| Rich Text Editor | TinyMCE Blazor or Radzen RichTextEditor | Latest |
| Testing | xUnit + Moq + FluentAssertions | Latest |
| API Docs | Swagger / Swashbuckle | Latest |
| Mapping | AutoMapper or Mapster | Latest |
| Validation | FluentValidation | Latest |
| Logging | Serilog | Latest |
| JSON | System.Text.Json | Built-in |

---

## 4. Database Design

### 4.1 Entity Relationship Diagram (Text)

```
Employees (1) -----> (N) MinuteSheetRequests     [RequesterPNo]
Employees (1) -----> (N) MinuteSheetRequests     [CurrentActionerPNo]
Employees (1) -----> (1) Employees               [ManagerPNo -> PNo] (self-referencing)

RequestTypes (1) ---> (N) MinuteSheetRequests     [RequestTypeId]
RequestTypes (1) ---> (N) WorkflowRules           [RequestTypeId]

MinuteSheetRequests (1) ---> (N) MinuteSheetAttachments  [MinuteSheetId]
MinuteSheetRequests (1) ---> (N) WorkflowStages          [MinuteSheetId]
MinuteSheetRequests (1) ---> (N) WorkflowHistory         [MinuteSheetId]
MinuteSheetRequests (1) ---> (N) WorkflowExceptions      [MinuteSheetId]
MinuteSheetRequests (1) ---> (1) AiAnalysisResults       [MinuteSheetId]

Employees (1) -----> (N) WorkflowStages          [ActionerPNo]
Employees (1) -----> (N) WorkflowHistory         [ActionerPNo]
```

### 4.2 Table Definitions

#### Table: `Employees`
| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| PNo | VARCHAR(10) | PK | Unique employee number (e.g., 00001001) |
| OldPNo | VARCHAR(10) | NULL | Legacy employee number reference |
| Name | NVARCHAR(200) | NOT NULL | Employee display name |
| Email | NVARCHAR(200) | NULL | Dummy email |
| Phone | VARCHAR(20) | NULL | Dummy phone |
| CNIC | VARCHAR(20) | NULL | Fake CNIC pattern |
| Gender | INT | NULL | Coded gender |
| MaritalStatus | INT | NULL | Coded marital status |
| FatherName | NVARCHAR(200) | NULL | Guardian name |
| DOB | DATE | NULL | Date of birth |
| HireDate | DATE | NULL | Hire/joining date |
| LastPromotionDate | DATE | NULL | Last promotion date |
| RetirementDate | DATE | NULL | Retirement date |
| LeavingDate | DATE | NULL | Leaving date (inactive) |
| Designation | NVARCHAR(100) | NOT NULL | Full designation |
| DesignationShort | VARCHAR(20) | NULL | Short designation code |
| JobDescription | NVARCHAR(200) | NULL | Job title/description |
| JobKey | VARCHAR(20) | NULL | Job identifier code |
| PositionId | VARCHAR(20) | NULL | Position identifier |
| DepartmentId | VARCHAR(10) | NOT NULL | Department code (FK) |
| DepartmentName | NVARCHAR(100) | NULL | Department name |
| DepartmentShort | VARCHAR(20) | NULL | Short department code |
| SectionId | VARCHAR(10) | NULL | Section code |
| EmployeeGroup | VARCHAR(20) | NULL | Employee group code |
| EmployeeCategory | VARCHAR(20) | NULL | Employee category code |
| GroupId | VARCHAR(20) | NULL | Group identifier |
| GroupDesc | NVARCHAR(100) | NULL | Group description |
| GroupShort | VARCHAR(20) | NULL | Short group code |
| PAreaId | VARCHAR(20) | NULL | Personnel area ID |
| PAreaDesc | NVARCHAR(100) | NULL | Personnel area description |
| PSAreaId | VARCHAR(20) | NULL | Personnel sub-area ID |
| PSAreaDesc | NVARCHAR(100) | NULL | Personnel sub-area description |
| CostCenter | VARCHAR(20) | NULL | Cost center code |
| CostCenterDesc | NVARCHAR(100) | NULL | Cost center description |
| ManagerPNo | VARCHAR(10) | FK -> Employees.PNo, NULL | Manager's employee number |
| IsActive | BIT | NOT NULL, DEFAULT 1 | Active/inactive flag |
| ActingPNo | VARCHAR(10) | FK -> Employees.PNo, NULL | Acting authority PNo |
| ActingFrom | DATE | NULL | Acting period start |
| ActingTo | DATE | NULL | Acting period end |
| AnnualLeaveBalance | DECIMAL(5,1) | NULL | Leave balance |
| CreatedAt | DATETIME2 | NOT NULL, DEFAULT GETDATE() | Record creation timestamp |
| UpdatedAt | DATETIME2 | NULL | Last update timestamp |

#### Table: `Departments`
| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| DepartmentId | VARCHAR(10) | PK | Department code |
| Name | NVARCHAR(100) | NOT NULL | Department name |
| ShortName | VARCHAR(20) | NULL | Short code |
| IsActive | BIT | NOT NULL, DEFAULT 1 | Active flag |

#### Table: `RequestTypes`
| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | INT | PK, IDENTITY | Auto-increment ID |
| Code | VARCHAR(20) | UNIQUE, NOT NULL | Type code (FIN, NONFIN, HR, IT, PROC, ADMIN) |
| Name | NVARCHAR(100) | NOT NULL | Display name |
| Description | NVARCHAR(500) | NULL | Description |
| IsActive | BIT | NOT NULL, DEFAULT 1 | Active flag |

#### Table: `WorkflowRules`
| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | INT | PK, IDENTITY | Auto-increment ID |
| RequestTypeId | INT | FK -> RequestTypes.Id, NOT NULL | Request type |
| BudgetFrom | DECIMAL(18,2) | NOT NULL, DEFAULT 0 | Budget range start |
| BudgetTo | DECIMAL(18,2) | NULL | Budget range end (NULL = no limit) |
| RequiredManagerLevels | INT | NOT NULL | Number of manager levels required |
| RequiresFinanceReview | BIT | NOT NULL, DEFAULT 0 | Finance review flag |
| FallbackBehavior | VARCHAR(20) | NOT NULL, DEFAULT 'WARN' | WARN, SKIP, ROUTE_TO_ADMIN |
| FinalAction | VARCHAR(20) | NOT NULL, DEFAULT 'APPROVE' | Final action type |
| IsActive | BIT | NOT NULL, DEFAULT 1 | Active flag |

#### Table: `MinuteSheetRequests`
| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | INT | PK, IDENTITY | Auto-increment ID |
| ReferenceNumber | VARCHAR(30) | UNIQUE, NOT NULL | Auto-generated ref (MS-2026-00001) |
| Subject | NVARCHAR(500) | NOT NULL | Request subject |
| Body | NVARCHAR(MAX) | NOT NULL | Rich text body |
| RequestTypeId | INT | FK -> RequestTypes.Id, NOT NULL | Request type |
| EstimatedBudget | DECIMAL(18,2) | NULL | Estimated budget amount |
| Priority | VARCHAR(10) | NOT NULL, DEFAULT 'NORMAL' | LOW, NORMAL, HIGH, URGENT |
| IsConfidential | BIT | NOT NULL, DEFAULT 0 | Confidentiality flag |
| WorkflowMode | VARCHAR(10) | NOT NULL | MANUAL, DYNAMIC, HYBRID |
| Status | VARCHAR(20) | NOT NULL, DEFAULT 'DRAFT' | Current request status |
| RequesterPNo | VARCHAR(10) | FK -> Employees.PNo, NOT NULL | Initiator |
| CurrentActionerPNo | VARCHAR(10) | FK -> Employees.PNo, NULL | Current pending actioner |
| CurrentStageOrder | INT | NULL | Current active stage order |
| SubmittedAt | DATETIME2 | NULL | Submission timestamp |
| CompletedAt | DATETIME2 | NULL | Completion timestamp |
| CreatedAt | DATETIME2 | NOT NULL, DEFAULT GETDATE() | Creation timestamp |
| UpdatedAt | DATETIME2 | NULL | Last update timestamp |

**Status Values**: `DRAFT`, `SUBMITTED`, `IN_REVIEW`, `APPROVED`, `REJECTED`, `RETURNED`, `CANCELLED`, `RESUBMITTED`

#### Table: `MinuteSheetAttachments`
| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | INT | PK, IDENTITY | Auto-increment ID |
| MinuteSheetId | INT | FK -> MinuteSheetRequests.Id, NOT NULL | Parent request |
| FileName | NVARCHAR(255) | NOT NULL | Original file name |
| FileType | VARCHAR(50) | NULL | MIME type |
| FileSize | BIGINT | NULL | File size in bytes |
| StoragePath | NVARCHAR(500) | NULL | Storage path (optional for prototype) |
| UploadedByPNo | VARCHAR(10) | FK -> Employees.PNo, NOT NULL | Uploader |
| UploadedAt | DATETIME2 | NOT NULL, DEFAULT GETDATE() | Upload timestamp |

#### Table: `WorkflowStages`
| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | INT | PK, IDENTITY | Auto-increment ID |
| MinuteSheetId | INT | FK -> MinuteSheetRequests.Id, NOT NULL | Parent request |
| StageOrder | INT | NOT NULL | Sequential order (1, 2, 3...) |
| ActionerPNo | VARCHAR(10) | FK -> Employees.PNo, NOT NULL | Assigned actioner |
| ActionerName | NVARCHAR(200) | NOT NULL | Actioner name (snapshot) |
| ActionerDesignation | NVARCHAR(100) | NULL | Designation (snapshot) |
| ActionType | VARCHAR(20) | NOT NULL | REVIEW, APPROVE, FINANCE_REVIEW |
| Status | VARCHAR(20) | NOT NULL, DEFAULT 'PENDING' | PENDING, ACTIVE, COMPLETED, SKIPPED |
| Action | VARCHAR(20) | NULL | REVIEWED, APPROVED, REJECTED, RETURNED |
| Remarks | NVARCHAR(1000) | NULL | Actioner remarks |
| ActionedAt | DATETIME2 | NULL | Action timestamp |
| Source | VARCHAR(10) | NOT NULL | MANUAL, DYNAMIC, HYBRID |
| CreatedAt | DATETIME2 | NOT NULL, DEFAULT GETDATE() | Creation timestamp |

#### Table: `WorkflowHistory`
| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | INT | PK, IDENTITY | Auto-increment ID |
| MinuteSheetId | INT | FK -> MinuteSheetRequests.Id, NOT NULL | Parent request |
| ActionerPNo | VARCHAR(10) | FK -> Employees.PNo, NOT NULL | Who performed the action |
| ActionerName | NVARCHAR(200) | NOT NULL | Name snapshot |
| Action | VARCHAR(20) | NOT NULL | CREATE, SUBMIT, REVIEW, APPROVE, REJECT, RETURN, RESUBMIT, CANCEL |
| PreviousStatus | VARCHAR(20) | NOT NULL | Status before action |
| NewStatus | VARCHAR(20) | NOT NULL | Status after action |
| Remarks | NVARCHAR(1000) | NULL | User remarks |
| StageOrder | INT | NULL | Stage number (if applicable) |
| Timestamp | DATETIME2 | NOT NULL, DEFAULT GETDATE() | Action timestamp |

#### Table: `WorkflowExceptions`
| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | INT | PK, IDENTITY | Auto-increment ID |
| MinuteSheetId | INT | FK -> MinuteSheetRequests.Id, NULL | Related request (NULL for employee-level) |
| EmployeePNo | VARCHAR(10) | FK -> Employees.PNo, NULL | Related employee |
| ExceptionType | VARCHAR(30) | NOT NULL | MISSING_MANAGER, INACTIVE_MANAGER, SELF_MANAGER, CIRCULAR_HIERARCHY, INSUFFICIENT_LEVELS |
| Description | NVARCHAR(500) | NOT NULL | Human-readable description |
| Severity | VARCHAR(10) | NOT NULL | WARNING, ERROR, CRITICAL |
| IsResolved | BIT | NOT NULL, DEFAULT 0 | Resolution flag |
| ResolvedByPNo | VARCHAR(10) | NULL | Resolver |
| ResolvedAt | DATETIME2 | NULL | Resolution timestamp |
| CreatedAt | DATETIME2 | NOT NULL, DEFAULT GETDATE() | Detection timestamp |

#### Table: `AiAnalysisResults`
| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | INT | PK, IDENTITY | Auto-increment ID |
| MinuteSheetId | INT | FK -> MinuteSheetRequests.Id, NOT NULL | Parent request |
| Summary | NVARCHAR(2000) | NULL | 5-7 line summary |
| DetectedBudget | DECIMAL(18,2) | NULL | AI-detected budget |
| Impact | NVARCHAR(1000) | NULL | Impact assessment |
| Beneficiaries | NVARCHAR(1000) | NULL | JSON array of beneficiaries |
| Urgency | VARCHAR(20) | NULL | Low, Medium, High, Critical |
| RiskLevel | VARCHAR(20) | NULL | Low, Medium, High |
| SuggestedCategory | VARCHAR(50) | NULL | Suggested request category |
| SuggestedSubject | NVARCHAR(500) | NULL | Suggested clean subject |
| MissingInformation | NVARCHAR(2000) | NULL | JSON array of missing items |
| RiskFlags | NVARCHAR(2000) | NULL | JSON array of risk flags |
| SuggestedRoute | NVARCHAR(2000) | NULL | JSON array of route suggestion |
| ReviewerChecklist | NVARCHAR(2000) | NULL | JSON array of checklist items |
| SuggestedWorkflowMode | VARCHAR(10) | NULL | MANUAL, DYNAMIC, HYBRID |
| SuggestedLevels | INT | NULL | Recommended approval levels |
| AnalyzedAt | DATETIME2 | NOT NULL, DEFAULT GETDATE() | Analysis timestamp |
| ModelUsed | VARCHAR(50) | NULL | AI model identifier |
| RawResponse | NVARCHAR(MAX) | NULL | Full raw AI response JSON |

### 4.3 Indexes

```sql
-- Employees
CREATE INDEX IX_Employees_ManagerPNo ON Employees(ManagerPNo);
CREATE INDEX IX_Employees_DepartmentId ON Employees(DepartmentId);
CREATE INDEX IX_Employees_IsActive ON Employees(IsActive);
CREATE INDEX IX_Employees_Name ON Employees(Name);

-- MinuteSheetRequests
CREATE INDEX IX_MinuteSheet_RequesterPNo ON MinuteSheetRequests(RequesterPNo);
CREATE INDEX IX_MinuteSheet_CurrentActioner ON MinuteSheetRequests(CurrentActionerPNo);
CREATE INDEX IX_MinuteSheet_Status ON MinuteSheetRequests(Status);
CREATE INDEX IX_MinuteSheet_RequestTypeId ON MinuteSheetRequests(RequestTypeId);
CREATE INDEX IX_MinuteSheet_CreatedAt ON MinuteSheetRequests(CreatedAt);

-- WorkflowStages
CREATE INDEX IX_WorkflowStages_MinuteSheetId ON WorkflowStages(MinuteSheetId);
CREATE INDEX IX_WorkflowStages_ActionerPNo ON WorkflowStages(ActionerPNo);

-- WorkflowHistory
CREATE INDEX IX_WorkflowHistory_MinuteSheetId ON WorkflowHistory(MinuteSheetId);
```

---

## 5. Domain Models

### 5.1 Enums

```csharp
public enum RequestStatus
{
    Draft,
    Submitted,
    InReview,
    Approved,
    Rejected,
    Returned,
    Cancelled,
    Resubmitted
}

public enum WorkflowMode
{
    Manual,
    Dynamic,
    Hybrid
}

public enum Priority
{
    Low,
    Normal,
    High,
    Urgent
}

public enum StageActionType
{
    Review,
    Approve,
    FinanceReview
}

public enum StageStatus
{
    Pending,
    Active,
    Completed,
    Skipped
}

public enum StageAction
{
    Reviewed,
    Approved,
    Rejected,
    Returned
}

public enum WorkflowActionType
{
    Create,
    Submit,
    Review,
    Approve,
    Reject,
    Return,
    Resubmit,
    Cancel
}

public enum ExceptionType
{
    MissingManager,
    InactiveManager,
    SelfManager,
    CircularHierarchy,
    InsufficientLevels,
    DuplicateApprover
}

public enum ExceptionSeverity
{
    Warning,
    Error,
    Critical
}

public enum FallbackBehavior
{
    Warn,
    Skip,
    RouteToAdmin
}
```

### 5.2 Core Domain Entities

```csharp
// Domain/Entities/Employee.cs
public class Employee
{
    public string PNo { get; set; }
    public string? OldPNo { get; set; }
    public string Name { get; set; }
    public string? Email { get; set; }
    public string Designation { get; set; }
    public string? DesignationShort { get; set; }
    public string DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string? SectionId { get; set; }
    public string? ManagerPNo { get; set; }
    public bool IsActive { get; set; }
    public string? ActingPNo { get; set; }
    public DateTime? ActingFrom { get; set; }
    public DateTime? ActingTo { get; set; }
    // ... additional Orbit fields ...

    // Navigation
    public Employee? Manager { get; set; }
    public ICollection<Employee> DirectReports { get; set; }
}

// Domain/Entities/MinuteSheetRequest.cs
public class MinuteSheetRequest
{
    public int Id { get; set; }
    public string ReferenceNumber { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public int RequestTypeId { get; set; }
    public decimal? EstimatedBudget { get; set; }
    public Priority Priority { get; set; }
    public bool IsConfidential { get; set; }
    public WorkflowMode WorkflowMode { get; set; }
    public RequestStatus Status { get; set; }
    public string RequesterPNo { get; set; }
    public string? CurrentActionerPNo { get; set; }
    public int? CurrentStageOrder { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Employee Requester { get; set; }
    public Employee? CurrentActioner { get; set; }
    public RequestType RequestType { get; set; }
    public ICollection<WorkflowStage> Stages { get; set; }
    public ICollection<WorkflowHistory> History { get; set; }
    public ICollection<MinuteSheetAttachment> Attachments { get; set; }
    public AiAnalysisResult? AiAnalysis { get; set; }
}
```

### 5.3 Domain Interfaces

```csharp
// Domain/Interfaces/IHierarchyResolver.cs
public interface IHierarchyResolver
{
    Task<HierarchyResult> ResolveManagerChainAsync(string employeePNo, int maxLevels = 10);
    Task<List<HierarchyException>> ValidateHierarchyAsync(string employeePNo);
}

// Domain/Interfaces/IWorkflowEngine.cs
public interface IWorkflowEngine
{
    Task<RoutePreview> GenerateRoutePreviewAsync(int minuteSheetId, WorkflowMode mode);
    Task<List<WorkflowStage>> GenerateStagesAsync(int minuteSheetId, RoutePreview approvedRoute);
    Task<WorkflowActionResult> ProcessActionAsync(int minuteSheetId, string actionerPNo, WorkflowActionType action, string? remarks);
    Task<bool> ValidateActionerAsync(int minuteSheetId, string actionerPNo);
}
```

---

## 6. Backend API Design

### 6.1 API Base URL
```
/api/v1/
```

### 6.2 Controllers & Endpoints

#### EmployeesController
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/employees` | List employees (paginated, filterable) |
| GET | `/api/v1/employees/{pno}` | Get employee by PNo |
| GET | `/api/v1/employees/{pno}/manager-chain` | Resolve full manager hierarchy chain |
| GET | `/api/v1/employees/{pno}/direct-reports` | List direct reports |
| GET | `/api/v1/employees/search?q={term}` | Search employees by name, PNo, email, designation |
| GET | `/api/v1/employees/{pno}/hierarchy-validation` | Validate hierarchy for issues |

**Query Parameters for listing:**
- `page`, `pageSize` - Pagination
- `department` - Filter by department
- `isActive` - Filter active/inactive
- `search` - Search term
- `sortBy`, `sortDir` - Sorting

#### MinuteSheetsController
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/minute-sheets` | List requests (paginated, filterable) |
| GET | `/api/v1/minute-sheets/{id}` | Get request details with stages and history |
| POST | `/api/v1/minute-sheets` | Create new draft request |
| PUT | `/api/v1/minute-sheets/{id}` | Update draft request |
| DELETE | `/api/v1/minute-sheets/{id}` | Delete draft request |
| POST | `/api/v1/minute-sheets/{id}/submit` | Submit request (triggers stage generation) |
| POST | `/api/v1/minute-sheets/{id}/cancel` | Cancel request |
| POST | `/api/v1/minute-sheets/{id}/resubmit` | Resubmit returned request |
| GET | `/api/v1/minute-sheets/{id}/stages` | Get workflow stages |
| GET | `/api/v1/minute-sheets/{id}/history` | Get audit trail |
| GET | `/api/v1/minute-sheets/{id}/exceptions` | Get workflow exceptions |

**Query Parameters for listing:**
- `page`, `pageSize` - Pagination
- `status` - Filter by status
- `requestType` - Filter by type
- `requesterPNo` - Filter by requester
- `currentActionerPNo` - Filter by current actioner
- `priority` - Filter by priority
- `dateFrom`, `dateTo` - Date range
- `budgetMin`, `budgetMax` - Budget range
- `search` - Search subject/reference
- `sortBy`, `sortDir` - Sorting

#### WorkflowController
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/workflow/{minuteSheetId}/preview-route` | Generate route preview |
| POST | `/api/v1/workflow/{minuteSheetId}/action` | Perform workflow action (review/approve/reject/return) |
| GET | `/api/v1/workflow/my-pending-actions?actioner={pno}` | Get pending actions for user |
| GET | `/api/v1/workflow/rules` | Get all workflow rules |
| GET | `/api/v1/workflow/rules/{requestTypeId}` | Get rules for request type |

**Action Request Body:**
```json
{
  "actionerPNo": "00001010",
  "actionType": "REVIEWED",   // REVIEWED, APPROVED, REJECTED, RETURNED
  "remarks": "Looks good, forwarding."
}
```

#### AttachmentsController
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/minute-sheets/{id}/attachments` | List attachments |
| POST | `/api/v1/minute-sheets/{id}/attachments` | Add attachment metadata |
| DELETE | `/api/v1/minute-sheets/{id}/attachments/{attachId}` | Remove attachment |

#### AiController
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/ai/analyze` | Analyze minute sheet content |
| GET | `/api/v1/ai/analysis/{minuteSheetId}` | Get stored analysis for request |

**Analyze Request Body:**
```json
{
  "subject": "Procurement of IT Equipment",
  "body": "We need to purchase 50 laptops for the new department...",
  "estimatedBudget": 250000,
  "requestType": "Financial"
}
```

#### DashboardController
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/dashboard/summary?userPNo={pno}` | Dashboard summary stats |
| GET | `/api/v1/dashboard/my-requests?userPNo={pno}` | My submitted requests |
| GET | `/api/v1/dashboard/my-pending?userPNo={pno}` | My pending actions |
| GET | `/api/v1/dashboard/status-counts` | Request count by status |
| GET | `/api/v1/dashboard/aging` | Aging analysis (requests pending > N days) |
| GET | `/api/v1/dashboard/delayed` | Delayed approvals |

#### RequestTypesController
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/request-types` | List all request types |

#### AuthController (Simulated)
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/auth/login` | Simulate login (select dummy user) |
| GET | `/api/v1/auth/current-user` | Get current simulated user |
| GET | `/api/v1/auth/switch-user/{pno}` | Switch simulated user for testing |

### 6.3 Standard API Response Format

```csharp
// Success response
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
}

// Paginated response
public class PagedResponse<T>
{
    public bool Success { get; set; }
    public List<T> Data { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

// Error response
public class ApiErrorResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; }
    public List<string>? Errors { get; set; }
    public string? ErrorCode { get; set; }
}
```

### 6.4 Key DTOs

```csharp
// Create/Update Minute Sheet
public class CreateMinuteSheetDto
{
    public string Subject { get; set; }
    public string Body { get; set; }
    public int RequestTypeId { get; set; }
    public decimal? EstimatedBudget { get; set; }
    public string Priority { get; set; } = "NORMAL";
    public bool IsConfidential { get; set; }
    public string WorkflowMode { get; set; } // MANUAL, DYNAMIC, HYBRID
    public string RequesterPNo { get; set; }
    public List<ManualStageDto>? ManualStages { get; set; } // For MANUAL/HYBRID mode
}

public class ManualStageDto
{
    public int StageOrder { get; set; }
    public string ActionerPNo { get; set; }
    public string ActionType { get; set; } // REVIEW, APPROVE, FINANCE_REVIEW
}

// Route Preview
public class RoutePreviewDto
{
    public string WorkflowMode { get; set; }
    public List<RouteStepDto> Steps { get; set; }
    public List<RouteWarningDto> Warnings { get; set; }
    public bool IsValid { get; set; }
    public int RequiredLevels { get; set; }
    public int ResolvedLevels { get; set; }
}

public class RouteStepDto
{
    public int Order { get; set; }
    public string ActionerPNo { get; set; }
    public string ActionerName { get; set; }
    public string Designation { get; set; }
    public string Department { get; set; }
    public string ActionType { get; set; }
    public string Source { get; set; } // DYNAMIC, MANUAL, FINANCE
}

// Dashboard Summary
public class DashboardSummaryDto
{
    public int MyTotalRequests { get; set; }
    public int MyPendingActions { get; set; }
    public int DraftCount { get; set; }
    public int InReviewCount { get; set; }
    public int ApprovedCount { get; set; }
    public int RejectedCount { get; set; }
    public int ReturnedCount { get; set; }
    public List<AgingItemDto> AgingItems { get; set; }
    public List<DelayedApprovalDto> DelayedApprovals { get; set; }
}
```

---

## 7. Workflow Engine Design

### 7.1 Hierarchy Resolver

```
Input: Employee PNo, Max Levels
Output: Ordered list of managers OR list of exceptions

Algorithm:
1. Load employee by PNo
2. If employee not found -> Exception: MISSING_EMPLOYEE
3. Check if employee.ManagerPNo == employee.PNo -> Exception: SELF_MANAGER
4. Initialize visited set = { employee.PNo }
5. Set current = employee.ManagerPNo
6. While current != null AND levels < maxLevels:
   a. If current is in visited set -> Exception: CIRCULAR_HIERARCHY
   b. Add current to visited set
   c. Load manager by current PNo
   d. If manager not found -> Exception: MISSING_MANAGER
   e. If manager.IsActive == false -> Warning: INACTIVE_MANAGER
      (based on FallbackBehavior: SKIP, WARN, or ROUTE_TO_ADMIN)
   f. Add manager to chain
   g. current = manager.ManagerPNo
   h. levels++
7. If levels < requiredLevels -> Warning: INSUFFICIENT_LEVELS
8. Remove consecutive duplicates from chain
9. Return chain + exceptions/warnings
```

### 7.2 Route Generator

```
Input: MinuteSheetRequest, WorkflowMode, WorkflowRule

Mode: DYNAMIC
1. Resolve manager chain from requester's ManagerPNo
2. Assign action types:
   - Levels 1 to (N-1) = REVIEW
   - Level N = APPROVE
3. If rule.RequiresFinanceReview, insert FINANCE_REVIEW stage before final APPROVE
4. Return RoutePreview with steps and warnings

Mode: MANUAL
1. Use provided ManualStages from user input
2. Validate each actioner exists and is active
3. Validate no duplicate consecutive actioners
4. Return RoutePreview

Mode: HYBRID
1. Generate DYNAMIC route first
2. Merge with any user-added manual stages
3. Allow user to insert additional reviewers or reorder
4. Return RoutePreview for user confirmation
```

### 7.3 Stage Manager

```
On Submit:
1. Generate stages from approved RoutePreview
2. Set stage 1 status = ACTIVE, all others = PENDING
3. Set request.CurrentActionerPNo = stage1.ActionerPNo
4. Set request.CurrentStageOrder = 1
5. Set request.Status = SUBMITTED (or IN_REVIEW)
6. Log to WorkflowHistory

On Action (REVIEWED / APPROVED):
1. Validate actioner == currentActioner
2. Mark current stage as COMPLETED with action and remarks
3. If next stage exists:
   a. Set next stage to ACTIVE
   b. Update request.CurrentActionerPNo
   c. Update request.CurrentStageOrder
   d. Set request.Status = IN_REVIEW
4. If no next stage (final):
   a. Set request.Status = APPROVED
   b. Set request.CurrentActionerPNo = NULL
   c. Set request.CompletedAt = now
5. Log to WorkflowHistory

On REJECTED:
1. Validate actioner == currentActioner
2. Mark current stage COMPLETED with action = REJECTED
3. Mark all remaining PENDING stages as SKIPPED
4. Set request.Status = REJECTED
5. Set request.CurrentActionerPNo = NULL
6. Set request.CompletedAt = now
7. Log to WorkflowHistory

On RETURNED:
1. Validate actioner == currentActioner
2. Mark current stage COMPLETED with action = RETURNED
3. Set request.Status = RETURNED
4. Set request.CurrentActionerPNo = request.RequesterPNo
5. Log to WorkflowHistory

On RESUBMIT:
1. Validate actioner == requester
2. Optionally regenerate stages or reset existing stages
3. Set stage 1 back to ACTIVE
4. Set request.Status = RESUBMITTED -> IN_REVIEW
5. Update CurrentActionerPNo to stage 1 actioner
6. Log to WorkflowHistory

On CANCEL:
1. Validate actioner == requester
2. Only allowed if status is DRAFT, SUBMITTED, IN_REVIEW, or RETURNED
3. Mark all PENDING/ACTIVE stages as SKIPPED
4. Set request.Status = CANCELLED
5. Log to WorkflowHistory
```

### 7.4 State Machine

```
DRAFT       -> SUBMITTED     (on Submit)
DRAFT       -> CANCELLED     (on Cancel)
SUBMITTED   -> IN_REVIEW     (stage 1 becomes active)
IN_REVIEW   -> IN_REVIEW     (on Review, move to next stage)
IN_REVIEW   -> APPROVED      (on final Approve)
IN_REVIEW   -> REJECTED      (on Reject at any stage)
IN_REVIEW   -> RETURNED      (on Return at any stage)
IN_REVIEW   -> CANCELLED     (on Cancel by requester)
RETURNED    -> RESUBMITTED   (on Resubmit by requester)
RETURNED    -> CANCELLED     (on Cancel by requester)
RESUBMITTED -> IN_REVIEW     (stages reset, stage 1 active)
```

Invalid transitions should throw `InvalidOperationException` with descriptive messages.

---

## 8. AI Service Architecture

### 8.1 Interface

```csharp
public interface IAiMinuteSheetService
{
    Task<AiMinuteSheetAnalysis> AnalyzeAsync(
        string subject,
        string body,
        decimal? estimatedBudget,
        string requestType);
}

public class AiMinuteSheetAnalysis
{
    public string Summary { get; set; }
    public decimal? DetectedBudget { get; set; }
    public string? Impact { get; set; }
    public List<string> Beneficiaries { get; set; }
    public string Urgency { get; set; }
    public string RiskLevel { get; set; }
    public string SuggestedCategory { get; set; }
    public string? SuggestedSubject { get; set; }
    public List<string> MissingInformation { get; set; }
    public List<string> RiskFlags { get; set; }
    public List<string> SuggestedRoute { get; set; }
    public List<string> ReviewerChecklist { get; set; }
    public string SuggestedWorkflowMode { get; set; }
    public int SuggestedLevels { get; set; }
    public string ModelUsed { get; set; }
}
```

### 8.2 Mock Implementation

```csharp
public class MockAiMinuteSheetService : IAiMinuteSheetService
{
    // Uses keyword matching and rule-based logic to simulate AI analysis
    // - Detects budget keywords ("budget", "cost", "PKR", numbers)
    // - Detects urgency keywords ("urgent", "immediately", "ASAP")
    // - Checks for missing attachments, dates, vendor names
    // - Suggests category based on keywords
    // - Generates summary from first N sentences
    // - Returns structured mock response
}
```

### 8.3 Real Implementation (Future)

```csharp
public class OpenAiMinuteSheetService : IAiMinuteSheetService
{
    // Uses OpenAI/Azure OpenAI API with structured prompts
    // - System prompt defines the analysis format
    // - Returns JSON-structured response
    // - Fallback to mock on failure
    // Configuration via appsettings.json
}
```

### 8.4 Registration

```csharp
// In API Program.cs - swap implementations easily
services.AddScoped<IAiMinuteSheetService, MockAiMinuteSheetService>();
// OR
// services.AddScoped<IAiMinuteSheetService, OpenAiMinuteSheetService>();
```

---

## 9. Frontend Blazor Design

### 9.1 Project Type
- **Blazor Server** with Interactive Server rendering (already set up)
- Uses **HttpClient** to call the Web API backend
- Bootstrap 5 for styling

### 9.2 Page Structure

```
/login                           -> Login Page
/register                        -> Registration Page
/                                -> Dashboard (Home) [Authenticated]
/profile                         -> User Profile / Change Password
/employees                       -> Employee Directory (list/search)
/employees/{pno}                 -> Employee Detail / Profile
/employees/{pno}/hierarchy       -> Visual Manager Chain
/minute-sheets                   -> Minute Sheet Listing (all / filtered)
/minute-sheets/create            -> Create New Minute Sheet
/minute-sheets/{id}              -> View Minute Sheet Detail + Workflow
/minute-sheets/{id}/edit         -> Edit Draft Minute Sheet
/minute-sheets/{id}/route        -> Route Preview & Adjustment
/minute-sheets/{id}/ai           -> AI Analysis Panel
/my-requests                     -> My Submitted Requests
/my-pending                      -> My Pending Actions
/workflow-rules                  -> Workflow Rules Reference
/exceptions                      -> Workflow Exceptions Panel
/admin/users                     -> User Management (Admin only)
```

### 9.3 Component Hierarchy

```
App.razor
+-- Routes.razor
    +-- MainLayout.razor
        +-- NavMenu.razor                    (Sidebar navigation)
        +-- UserBadge.razor                  (Current user display + switch)
        +-- @Body
            |
            +-- Pages/
            |   +-- Dashboard.razor
            |   |   +-- StatCard.razor                  (Reusable stat card)
            |   |   +-- MyRequestsTable.razor           (Quick list)
            |   |   +-- MyPendingActionsTable.razor      (Quick list)
            |   |   +-- StatusChart.razor                (Status distribution)
            |   |   +-- AgingAlert.razor                 (Delayed items)
            |   |
            |   +-- Employees/
            |   |   +-- EmployeeList.razor               (Directory with search/filter)
            |   |   +-- EmployeeDetail.razor             (Profile view)
            |   |   +-- ManagerChainView.razor           (Visual hierarchy)
            |   |
            |   +-- MinuteSheets/
            |   |   +-- MinuteSheetList.razor            (Listing with filters)
            |   |   +-- MinuteSheetCreate.razor          (Create/edit form)
            |   |   +-- MinuteSheetDetail.razor          (View + workflow + actions)
            |   |   +-- RoutePreview.razor               (Route preview/adjust page)
            |   |   +-- AiAnalysisPanel.razor            (AI results display)
            |   |
            |   +-- Admin/
            |   |   +-- WorkflowRules.razor              (Rules listing)
            |   |   +-- ExceptionsPanel.razor            (Exception viewer)
            |   |   +-- UserSwitcher.razor               (Testing tool)
            |   |
            +-- Shared/
                +-- SearchBar.razor
                +-- Pagination.razor
                +-- StatusBadge.razor
                +-- PriorityBadge.razor
                +-- ConfirmDialog.razor
                +-- LoadingSpinner.razor
                +-- EmptyState.razor
                +-- ErrorAlert.razor
                +-- RemarksModal.razor          (Modal for action + remarks)
                +-- EmployeePicker.razor        (Search & select employee)
                +-- AttachmentList.razor
                +-- AuditTrailTimeline.razor    (Workflow history timeline)
                +-- RouteStepCard.razor         (Individual route step)
                +-- WorkflowStageCard.razor     (Stage status card)
```

### 9.4 Key Page Designs

#### Dashboard (`/`)
```
+---------------------------------------------------------------+
| Dashboard                                    [User: Employee 001] |
+---------------------------------------------------------------+
| [My Requests: 12] [Pending Actions: 3] [Draft: 2] [Approved: 5] |
+---------------------------------------------------------------+
| My Pending Actions                                               |
| +---+----------+------------------+-----------+---------+------+ |
| | # | Ref      | Subject          | Requester | Status  | Act  | |
| +---+----------+------------------+-----------+---------+------+ |
| | 1 | MS-2026- | Equipment Purch  | Emp 001   | Review  | [->] | |
| | 2 | MS-2026- | Leave Policy     | Emp 003   | Review  | [->] | |
| +---+----------+------------------+-----------+---------+------+ |
+---------------------------------------------------------------+
| My Recent Requests                                               |
| +---+----------+------------------+-----------+---------+------+ |
| | 1 | MS-2026- | IT Infrastructure| Me        | In Rev  | [->] | |
| +---+----------+------------------+-----------+---------+------+ |
+---------------------------------------------------------------+
| Aging / Delayed Approvals                                        |
| [3 requests pending > 3 days] [1 request pending > 7 days]       |
+---------------------------------------------------------------+
```

#### Create Minute Sheet (`/minute-sheets/create`)
```
+---------------------------------------------------------------+
| Create New Minute Sheet                            [Save Draft] |
+---------------------------------------------------------------+
| Requester: Employee 001 (AM) - Dept: ICT                       |
+---------------------------------------------------------------+
| Subject:     [________________________________]                 |
| Type:        [Financial        v]                               |
| Budget:      [250,000____________]                              |
| Priority:    [Normal v]    [ ] Confidential                     |
+---------------------------------------------------------------+
| Body:                                                           |
| +-----------------------------------------------------------+  |
| | [Rich Text Editor - TinyMCE / Radzen]                     |  |
| |                                                            |  |
| +-----------------------------------------------------------+  |
+---------------------------------------------------------------+
| Attachments:                                                    |
| [+ Add Attachment]                                              |
| - Budget_Quotation.pdf (120KB)  [x]                             |
+---------------------------------------------------------------+
| Workflow Mode: ( ) Manual  (x) Dynamic  ( ) Hybrid              |
+---------------------------------------------------------------+
| [AI Analyze]  [Preview Route]  [Submit]  [Save Draft]           |
+---------------------------------------------------------------+
```

#### Route Preview (`/minute-sheets/{id}/route`)
```
+---------------------------------------------------------------+
| Route Preview - MS-2026-00005                                   |
+---------------------------------------------------------------+
| Mode: Dynamic | Required Levels: 3 | Finance Review: Yes       |
+---------------------------------------------------------------+
|                                                                 |
|  [1] Employee 001 (Requester)                                   |
|      |                                                          |
|  [2] Manager 001 - Manager (REVIEW)              [Dynamic]     |
|      |                                                          |
|  [3] Senior Manager 001 - Sr. Manager (REVIEW)   [Dynamic]     |
|      |                                                          |
|  [4] Finance Reviewer (FINANCE_REVIEW)            [Rule]       |
|      |                                                          |
|  [5] General Manager 001 - GM (APPROVE)           [Dynamic]    |
|                                                                 |
+---------------------------------------------------------------+
| Warnings:                                                       |
| (i) Route generated successfully. No issues detected.           |
+---------------------------------------------------------------+
| [+ Add Reviewer] (Hybrid mode only)                             |
| [Confirm & Submit]  [Back to Edit]                              |
+---------------------------------------------------------------+
```

#### AI Analysis Panel
```
+---------------------------------------------------------------+
| AI Analysis - MS-2026-00005                         [Re-Analyze]|
+---------------------------------------------------------------+
| Summary                                                         |
| This request proposes procurement of 50 laptops for the new     |
| ICT department. The estimated budget is PKR 250,000. The        |
| request aims to improve operational efficiency and reduce       |
| equipment downtime for end users.                               |
+---------------------------------------------------------------+
| Key Information                                                  |
| Budget: PKR 250,000  |  Urgency: Medium  |  Risk: Medium       |
| Category: Financial   |  Impact: Operational Efficiency          |
+---------------------------------------------------------------+
| Missing Information                                              |
| [!] Vendor quotation not attached                               |
| [!] Expected completion/delivery date not specified             |
| [!] Warranty terms not mentioned                                |
+---------------------------------------------------------------+
| Risk Flags                                                       |
| [!] Budget exceeds PKR 100K - requires 3+ approval levels       |
+---------------------------------------------------------------+
| Suggested Route                                                  |
| Manager -> Department Head -> Finance Reviewer -> Final Approver |
| Recommended Mode: Dynamic | Levels: 3                           |
+---------------------------------------------------------------+
| Reviewer Checklist                                               |
| [ ] Verify budget against department allocation                  |
| [ ] Confirm vendor selection process compliance                  |
| [ ] Check if similar procurement was done recently               |
+---------------------------------------------------------------+
```

### 9.5 Frontend Services (HttpClient Wrappers)

```csharp
// Services/IEmployeeService.cs
public interface IEmployeeService
{
    Task<PagedResponse<EmployeeListDto>> GetEmployeesAsync(EmployeeFilter filter);
    Task<EmployeeDetailDto> GetEmployeeAsync(string pno);
    Task<List<ManagerChainItemDto>> GetManagerChainAsync(string pno);
    Task<List<EmployeeListDto>> SearchEmployeesAsync(string query);
}

// Services/IMinuteSheetService.cs
public interface IMinuteSheetService
{
    Task<PagedResponse<MinuteSheetListDto>> GetMinuteSheetsAsync(MinuteSheetFilter filter);
    Task<MinuteSheetDetailDto> GetMinuteSheetAsync(int id);
    Task<MinuteSheetDetailDto> CreateAsync(CreateMinuteSheetDto dto);
    Task<MinuteSheetDetailDto> UpdateAsync(int id, UpdateMinuteSheetDto dto);
    Task DeleteAsync(int id);
    Task<MinuteSheetDetailDto> SubmitAsync(int id);
    Task<MinuteSheetDetailDto> CancelAsync(int id);
    Task<MinuteSheetDetailDto> ResubmitAsync(int id);
}

// Services/IWorkflowService.cs
public interface IWorkflowService
{
    Task<RoutePreviewDto> PreviewRouteAsync(int minuteSheetId, WorkflowMode mode);
    Task<WorkflowActionResultDto> PerformActionAsync(int minuteSheetId, WorkflowActionDto action);
    Task<List<PendingActionDto>> GetMyPendingActionsAsync(string actuerPNo);
}

// Services/IAiService.cs
public interface IAiService
{
    Task<AiAnalysisDto> AnalyzeAsync(AiAnalyzeRequestDto request);
    Task<AiAnalysisDto> GetAnalysisAsync(int minuteSheetId);
}

// Services/IDashboardService.cs
public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(string userPNo);
}
```

### 9.6 State Management

```csharp
// State/AppState.cs
public class AppState
{
    public EmployeeDetailDto? CurrentUser { get; private set; }
    public event Action? OnChange;

    public void SetCurrentUser(EmployeeDetailDto user)
    {
        CurrentUser = user;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}

// Registered as Scoped in DI (per-circuit in Blazor Server)
```

---

## 10. Authentication & Authorization

### 10.1 Overview
Full JWT-based authentication with login/registration. Users table links to Employees via `EmployeePNo`.

### 10.2 Database Tables
- **Users**: Id, Username, Email, PasswordHash, EmployeePNo (FK), IsActive, IsLocked, FailedLoginAttempts, LockoutEnd, LastLoginAt
- **Roles**: Id, Name (Admin, Employee, Manager, FinanceReviewer, HRAdmin, Auditor)
- **UserRoles**: UserId + RoleId (M:N junction)
- **RefreshTokens**: Id, UserId, Token, ExpiresAt, RevokedAt, CreatedByIp

See `docs/Schema.sql` and `docs/SeedData.sql` for full table definitions and dummy data.

### 10.3 Auth Flow

```
Registration:
1. User submits: username, email, password, employeePNo (optional)
2. Validate username/email uniqueness
3. Hash password with BCrypt
4. Create User record, assign default "Employee" role
5. If employeePNo provided, validate it exists in Employees table
6. Return success (user must login separately)

Login:
1. User submits: username + password
2. Lookup user by username
3. Verify BCrypt hash
4. Check IsActive, IsLocked, LockoutEnd
5. On success: reset FailedLoginAttempts, set LastLoginAt
6. Generate JWT access token (15 min) + refresh token (7 days)
7. Store refresh token in RefreshTokens table
8. Return { accessToken, refreshToken, user profile }

Token Refresh:
1. Client sends expired access token + valid refresh token
2. Validate refresh token exists, not expired, not revoked
3. Issue new access token + new refresh token
4. Revoke old refresh token
5. Return new tokens

Logout:
1. Revoke all active refresh tokens for user
2. Client discards access token
```

### 10.4 JWT Configuration
```csharp
// appsettings.json
"JwtSettings": {
    "Secret": "YourSuperSecretKeyForDevelopment_MinimumLength32Characters!",
    "Issuer": "MinuteSheetFFC.Api",
    "Audience": "MinuteSheetFFC.Web",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
}
```

### 10.5 Auth API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/auth/register` | Register new user |
| POST | `/api/v1/auth/login` | Login, returns JWT + refresh token |
| POST | `/api/v1/auth/refresh-token` | Refresh access token |
| POST | `/api/v1/auth/logout` | Revoke refresh tokens |
| GET  | `/api/v1/auth/me` | Get current authenticated user profile |
| POST | `/api/v1/auth/change-password` | Change password |

### 10.6 Authorization Policies
```csharp
// Role-based authorization
[Authorize(Roles = "Admin")]                    // Admin-only endpoints
[Authorize(Roles = "Employee,Manager")]         // Any authenticated employee
[Authorize(Roles = "FinanceReviewer")]          // Finance review actions
[Authorize]                                     // Any authenticated user

// Custom policy: workflow action authorization
services.AddAuthorization(options =>
{
    options.AddPolicy("CanPerformWorkflowAction", policy =>
        policy.RequireAuthenticatedUser());
    // Actual actioner validation done in service layer
    // by comparing JWT user's EmployeePNo with CurrentActionerPNo
});
```

### 10.7 Blazor Auth Integration
```csharp
// AppState stores JWT tokens and user profile
// AuthenticationStateProvider is customized for JWT
// HttpClient interceptor adds Authorization: Bearer {token} header
// Protected routes use [Authorize] attribute or <AuthorizeView> component
```

### 10.8 Frontend Auth Pages
```
/login                -> Login page
/register             -> Registration page
/profile              -> User profile (linked employee info)
/change-password      -> Change password page
```

---

## 11. Testing Strategy

### 11.1 Unit Tests

#### Domain / WorkflowEngine Tests
| Test Category | Test Cases |
|--------------|------------|
| HierarchyResolver | Normal 3-level chain resolves correctly |
| | Missing manager returns MISSING_MANAGER exception |
| | Inactive manager handled per FallbackBehavior |
| | Self-manager detected and blocked |
| | Circular hierarchy (A->B->C->A) detected |
| | Insufficient levels produces warning |
| | Duplicate consecutive approver removed |
| | Top-level employee (no manager) handled |
| RouteGenerator | Dynamic mode generates correct number of levels |
| | Finance review inserted when required |
| | Manual mode uses provided stages |
| | Hybrid mode merges dynamic + manual stages |
| | Budget-based rule selection works |
| StageManager | Submit creates stages, sets stage 1 active |
| | Review advances to next stage |
| | Final approve closes workflow |
| | Reject skips remaining stages |
| | Return sets requester as actioner |
| | Resubmit resets stages |
| | Cancel marks all stages as skipped |
| | Wrong actioner blocked |
| | Invalid state transition blocked |
| StatusTransitions | All valid transitions succeed |
| | All invalid transitions throw |

#### AI Service Tests
| Test | Description |
|------|-------------|
| AnalyzeFinancialRequest | Returns financial category, budget detection, risk flags |
| AnalyzeNonFinancialRequest | Returns non-financial category, lower risk |
| MissingBudgetDetected | Flags missing budget when body mentions cost |
| UrgentKeywordDetection | Detects urgency from keywords |
| SummaryGenerated | Summary is 5-7 lines |
| ReviewerChecklistGenerated | Checklist has actionable items |

### 11.2 Integration Tests

| Test | Description |
|------|-------------|
| CreateAndSubmitFlow | Create draft -> submit -> verify stages created |
| FullApprovalFlow | Submit -> review (L1) -> review (L2) -> approve (L3) -> status = Approved |
| RejectFlow | Submit -> review (L1) -> reject (L2) -> status = Rejected |
| ReturnAndResubmitFlow | Submit -> review -> return -> resubmit -> review -> approve |
| WrongActionerBlocked | Non-actioner cannot perform action |
| DynamicRouteGeneration | Verify route matches manager chain |
| BudgetRuleSelection | Correct rule selected based on budget range |
| ApiPaginationAndFiltering | Verify paging, filtering, sorting on list endpoints |

### 11.3 Edge Case Tests

| Test | Description |
|------|-------------|
| MissingManagerHierarchy | Employee with non-existent ManagerPNo |
| InactiveManagerInChain | Chain includes inactive manager |
| CircularHierarchyDetection | A -> B -> C -> A detected |
| SelfManagerDetection | Employee is own manager |
| InsufficientLevels | Only 2 managers but 4 required |
| CancelDraftRequest | Cancel a draft (no stages) |
| CancelInReviewRequest | Cancel while in review |
| ResubmitNonReturnedRequest | Should fail |
| DuplicateManagerInChain | Same manager appears twice consecutively |

### 11.4 Test Project Configuration

```csharp
// Use in-memory database for unit tests
services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TestDb"));

// Use WebApplicationFactory for API integration tests
public class ApiTestFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace SQL Server with in-memory
            // Seed test data
        });
    }
}
```

---

## 12. Implementation Milestones

### Milestone 1: Foundation (Days 1-2)
**Focus**: Solution setup, database, seed data, employee directory

| # | Task | Details | Priority |
|---|------|---------|----------|
| 1.1 | Create solution structure | Create all projects, add references, configure solution | High |
| 1.2 | Define domain entities & enums | All entities in Domain project | High |
| 1.3 | Configure EF Core DbContext | All entity configurations, relationships, indexes | High |
| 1.4 | Create seed data | 20+ dummy employees with various hierarchy patterns, request types, workflow rules | High |
| 1.5 | Run initial migration | Create database, verify schema | High |
| 1.6 | Build Employee repository & service | CRUD, search, manager chain resolution | High |
| 1.7 | Build EmployeesController API | All employee endpoints | High |
| 1.8 | Build Blazor Employee Directory page | List, search, filter, view detail | High |
| 1.9 | Build Manager Chain visualization | Visual hierarchy display | Medium |
| 1.10 | Configure API Swagger docs | Swashbuckle setup | Medium |
| 1.11 | Setup simulated auth middleware | X-Current-User header + user switcher | High |
| 1.12 | Build User Switcher page in Blazor | Select dummy user for testing | High |

**Deliverables**: Working employee directory with search, manager chain query, user switching

---

### Milestone 2: Request Creation & Route System (Days 3-4)
**Focus**: Minute sheet CRUD, route generation, route preview

| # | Task | Details | Priority |
|---|------|---------|----------|
| 2.1 | Build MinuteSheet repository & service | Create, update, delete drafts, reference number generation | High |
| 2.2 | Build MinuteSheetsController API | All CRUD + list endpoints | High |
| 2.3 | Build HierarchyResolver service | Manager chain walking with all edge case detection | High |
| 2.4 | Build RouteGenerator service | Dynamic, Manual, Hybrid route generation | High |
| 2.5 | Build route preview API endpoint | POST preview with warnings | High |
| 2.6 | Build Blazor Create Minute Sheet page | Form with all fields, rich text editor, file metadata | High |
| 2.7 | Build Blazor Minute Sheet listing page | Table with filters, status badges, pagination | High |
| 2.8 | Build Route Preview page | Visual route display, warnings, confirm/adjust | High |
| 2.9 | Build EmployeePicker component | Reusable search & select employee component | Medium |
| 2.10 | Build Hybrid mode route adjustment | Add/remove reviewers in hybrid mode | Medium |
| 2.11 | Implement WorkflowRules lookup | Select rule based on request type + budget | High |
| 2.12 | Build request type selector | Dropdown with types from API | High |

**Deliverables**: Create minute sheets, preview routes in all 3 modes, see warnings

---

### Milestone 3: Workflow Actions & Processing (Day 5)
**Focus**: Submit, review, approve, reject, return, resubmit, cancel

| # | Task | Details | Priority |
|---|------|---------|----------|
| 3.1 | Build StageManager service | Create stages on submit, advance stages, handle all actions | High |
| 3.2 | Build WorkflowController API | Action endpoint with validation | High |
| 3.3 | Implement state machine validation | Valid/invalid transition checks | High |
| 3.4 | Build current actioner enforcement | Only current actioner can act | High |
| 3.5 | Build WorkflowHistory logging | Automatic audit trail on every action | High |
| 3.6 | Build Blazor Minute Sheet Detail page | View request + stages + history + action buttons | High |
| 3.7 | Build RemarksModal component | Modal for entering remarks with action | High |
| 3.8 | Build AuditTrailTimeline component | Visual history timeline | Medium |
| 3.9 | Build WorkflowStageCard component | Stage status display with actioner info | Medium |
| 3.10 | Implement Return + Resubmit flow | Full return-to-requester and resubmit cycle | High |
| 3.11 | Build My Pending Actions page | List actions requiring current user | High |
| 3.12 | Implement Cancel flow | Cancel with stage cleanup | Medium |

**Deliverables**: Full workflow lifecycle working end-to-end

---

### Milestone 4: AI & Dashboard (Day 6)
**Focus**: AI analysis, dashboard, advanced filters, exceptions

| # | Task | Details | Priority |
|---|------|---------|----------|
| 4.1 | Build MockAiMinuteSheetService | Keyword-based mock AI analysis | High |
| 4.2 | Build AI API endpoint | POST analyze + GET stored analysis | High |
| 4.3 | Build AI Analysis Panel page | Display all AI results in structured format | High |
| 4.4 | Integrate AI into Create page | "Analyze" button that calls AI and shows results | High |
| 4.5 | Build AiAnalysisResults storage | Save/retrieve AI analyses | Medium |
| 4.6 | Build Dashboard API | Summary stats, my requests, pending, aging | High |
| 4.7 | Build Dashboard page | StatCards, tables, aging alerts | High |
| 4.8 | Build advanced search/filters | All filter parameters on listing page | Medium |
| 4.9 | Build Exceptions Panel | Display workflow exceptions with details | Medium |
| 4.10 | Build Workflow Rules reference page | Display all rules in a table | Low |
| 4.11 | Add real-time notifications (optional) | SignalR for action notifications | Low |
| 4.12 | Polish UI - status badges, empty states, loading states | UI refinement | Medium |

**Deliverables**: AI analysis working, dashboard functional, exceptions visible

---

### Milestone 5: Testing, Polish & Documentation (Day 7)
**Focus**: Testing, bug fixes, documentation, demo prep

| # | Task | Details | Priority |
|---|------|---------|----------|
| 5.1 | Write unit tests for HierarchyResolver | All edge cases | High |
| 5.2 | Write unit tests for RouteGenerator | All 3 modes | High |
| 5.3 | Write unit tests for StageManager | All action types + transitions | High |
| 5.4 | Write unit tests for MockAiService | Analysis scenarios | Medium |
| 5.5 | Write integration tests for API | Full flow tests | High |
| 5.6 | Write edge case tests | Broken hierarchy, wrong actioner, invalid transitions | High |
| 5.7 | Fix bugs found during testing | Bug fixes | High |
| 5.8 | Write Setup Guide | How to run the project | High |
| 5.9 | Write User Guide | How to use the application | Medium |
| 5.10 | Write Technical Notes | Workflow engine explanation, architecture decisions | Medium |
| 5.11 | Create ERD diagram | Database relationship diagram | Medium |
| 5.12 | Prepare demo walkthrough | Script for final demo | Medium |

**Deliverables**: All tests passing, documentation complete, demo-ready

---

## 13. Folder/File Map

```
MinuteSheetFFC.sln

src/
+-- MinuteSheetFFC.Domain/
|   +-- Entities/
|   |   +-- Employee.cs
|   |   +-- MinuteSheetRequest.cs
|   |   +-- MinuteSheetAttachment.cs
|   |   +-- RequestType.cs
|   |   +-- WorkflowRule.cs
|   |   +-- WorkflowStage.cs
|   |   +-- WorkflowHistory.cs
|   |   +-- WorkflowException.cs
|   |   +-- AiAnalysisResult.cs
|   |   +-- Department.cs
|   +-- Enums/
|   |   +-- RequestStatus.cs
|   |   +-- WorkflowMode.cs
|   |   +-- Priority.cs
|   |   +-- StageActionType.cs
|   |   +-- StageStatus.cs
|   |   +-- StageAction.cs
|   |   +-- WorkflowActionType.cs
|   |   +-- ExceptionType.cs
|   |   +-- ExceptionSeverity.cs
|   |   +-- FallbackBehavior.cs
|   +-- Interfaces/
|   |   +-- IEmployeeRepository.cs
|   |   +-- IMinuteSheetRepository.cs
|   |   +-- IWorkflowStageRepository.cs
|   |   +-- IWorkflowHistoryRepository.cs
|   |   +-- IWorkflowExceptionRepository.cs
|   |   +-- IAiAnalysisRepository.cs
|   |   +-- IRequestTypeRepository.cs
|   |   +-- IWorkflowRuleRepository.cs
|   |   +-- IUnitOfWork.cs
|   +-- MinuteSheetFFC.Domain.csproj
|
+-- MinuteSheetFFC.Application/
|   +-- DTOs/
|   |   +-- Employee/
|   |   |   +-- EmployeeListDto.cs
|   |   |   +-- EmployeeDetailDto.cs
|   |   |   +-- ManagerChainItemDto.cs
|   |   |   +-- EmployeeFilterDto.cs
|   |   +-- MinuteSheet/
|   |   |   +-- MinuteSheetListDto.cs
|   |   |   +-- MinuteSheetDetailDto.cs
|   |   |   +-- CreateMinuteSheetDto.cs
|   |   |   +-- UpdateMinuteSheetDto.cs
|   |   |   +-- MinuteSheetFilterDto.cs
|   |   +-- Workflow/
|   |   |   +-- RoutePreviewDto.cs
|   |   |   +-- RouteStepDto.cs
|   |   |   +-- RouteWarningDto.cs
|   |   |   +-- WorkflowActionDto.cs
|   |   |   +-- WorkflowActionResultDto.cs
|   |   |   +-- ManualStageDto.cs
|   |   |   +-- PendingActionDto.cs
|   |   |   +-- WorkflowStageDto.cs
|   |   |   +-- WorkflowHistoryDto.cs
|   |   +-- Ai/
|   |   |   +-- AiAnalyzeRequestDto.cs
|   |   |   +-- AiAnalysisDto.cs
|   |   +-- Dashboard/
|   |   |   +-- DashboardSummaryDto.cs
|   |   |   +-- AgingItemDto.cs
|   |   |   +-- DelayedApprovalDto.cs
|   |   +-- Common/
|   |       +-- ApiResponse.cs
|   |       +-- PagedResponse.cs
|   |       +-- ApiErrorResponse.cs
|   +-- Interfaces/
|   |   +-- IEmployeeService.cs
|   |   +-- IMinuteSheetService.cs
|   |   +-- IWorkflowEngineService.cs
|   |   +-- IDashboardService.cs
|   |   +-- IAiMinuteSheetService.cs
|   +-- Mapping/
|   |   +-- MappingProfile.cs
|   +-- Validators/
|   |   +-- CreateMinuteSheetValidator.cs
|   |   +-- WorkflowActionValidator.cs
|   +-- MinuteSheetFFC.Application.csproj
|
+-- MinuteSheetFFC.Infrastructure/
|   +-- Data/
|   |   +-- AppDbContext.cs
|   |   +-- Configurations/
|   |   |   +-- EmployeeConfiguration.cs
|   |   |   +-- MinuteSheetRequestConfiguration.cs
|   |   |   +-- WorkflowStageConfiguration.cs
|   |   |   +-- WorkflowHistoryConfiguration.cs
|   |   |   +-- WorkflowExceptionConfiguration.cs
|   |   |   +-- AiAnalysisResultConfiguration.cs
|   |   |   +-- RequestTypeConfiguration.cs
|   |   |   +-- WorkflowRuleConfiguration.cs
|   |   |   +-- DepartmentConfiguration.cs
|   |   |   +-- AttachmentConfiguration.cs
|   |   +-- Migrations/
|   |   +-- Seed/
|   |       +-- SeedData.cs
|   |       +-- EmployeeSeedData.cs
|   |       +-- RequestTypeSeedData.cs
|   |       +-- WorkflowRuleSeedData.cs
|   +-- Repositories/
|   |   +-- EmployeeRepository.cs
|   |   +-- MinuteSheetRepository.cs
|   |   +-- WorkflowStageRepository.cs
|   |   +-- WorkflowHistoryRepository.cs
|   |   +-- WorkflowExceptionRepository.cs
|   |   +-- AiAnalysisRepository.cs
|   |   +-- RequestTypeRepository.cs
|   |   +-- WorkflowRuleRepository.cs
|   |   +-- UnitOfWork.cs
|   +-- MinuteSheetFFC.Infrastructure.csproj
|
+-- MinuteSheetFFC.WorkflowEngine/
|   +-- Services/
|   |   +-- HierarchyResolver.cs
|   |   +-- RouteGenerator.cs
|   |   +-- StageManager.cs
|   |   +-- WorkflowEngineService.cs       (Orchestrator)
|   +-- Models/
|   |   +-- HierarchyResult.cs
|   |   +-- HierarchyException.cs
|   |   +-- RoutePreview.cs
|   |   +-- WorkflowActionResult.cs
|   +-- Validators/
|   |   +-- StateTransitionValidator.cs
|   |   +-- ActionerValidator.cs
|   +-- MinuteSheetFFC.WorkflowEngine.csproj
|
+-- MinuteSheetFFC.AiService/
|   +-- MockAiMinuteSheetService.cs
|   +-- OpenAiMinuteSheetService.cs        (Future)
|   +-- Models/
|   |   +-- AiMinuteSheetAnalysis.cs
|   +-- MinuteSheetFFC.AiService.csproj
|
+-- MinuteSheetFFC.Api/
|   +-- Controllers/
|   |   +-- EmployeesController.cs
|   |   +-- MinuteSheetsController.cs
|   |   +-- WorkflowController.cs
|   |   +-- AttachmentsController.cs
|   |   +-- AiController.cs
|   |   +-- DashboardController.cs
|   |   +-- RequestTypesController.cs
|   |   +-- AuthController.cs
|   +-- Middleware/
|   |   +-- SimulatedAuthMiddleware.cs
|   |   +-- ExceptionHandlingMiddleware.cs
|   +-- Filters/
|   |   +-- ValidationFilter.cs
|   +-- Program.cs
|   +-- appsettings.json
|   +-- appsettings.Development.json
|   +-- MinuteSheetFFC.Api.csproj
|
+-- MinuteSheetFFC.Web/                    (Existing Blazor project, restructured)
    +-- Components/
    |   +-- App.razor
    |   +-- Routes.razor
    |   +-- _Imports.razor
    |   +-- Layout/
    |   |   +-- MainLayout.razor
    |   |   +-- MainLayout.razor.css
    |   |   +-- NavMenu.razor
    |   |   +-- NavMenu.razor.css
    |   +-- Pages/
    |   |   +-- Home.razor                  (Dashboard)
    |   |   +-- Employees/
    |   |   |   +-- EmployeeList.razor
    |   |   |   +-- EmployeeDetail.razor
    |   |   |   +-- ManagerChain.razor
    |   |   +-- MinuteSheets/
    |   |   |   +-- MinuteSheetList.razor
    |   |   |   +-- MinuteSheetCreate.razor
    |   |   |   +-- MinuteSheetEdit.razor
    |   |   |   +-- MinuteSheetDetail.razor
    |   |   |   +-- RoutePreview.razor
    |   |   +-- Ai/
    |   |   |   +-- AiAnalysisPanel.razor
    |   |   +-- Admin/
    |   |       +-- UserSwitcher.razor
    |   |       +-- WorkflowRules.razor
    |   |       +-- ExceptionsPanel.razor
    |   +-- Shared/
    |       +-- SearchBar.razor
    |       +-- Pagination.razor
    |       +-- StatusBadge.razor
    |       +-- PriorityBadge.razor
    |       +-- ConfirmDialog.razor
    |       +-- LoadingSpinner.razor
    |       +-- EmptyState.razor
    |       +-- ErrorAlert.razor
    |       +-- RemarksModal.razor
    |       +-- EmployeePicker.razor
    |       +-- AttachmentList.razor
    |       +-- AuditTrailTimeline.razor
    |       +-- RouteStepCard.razor
    |       +-- WorkflowStageCard.razor
    |       +-- StatCard.razor
    +-- Services/
    |   +-- HttpEmployeeService.cs
    |   +-- HttpMinuteSheetService.cs
    |   +-- HttpWorkflowService.cs
    |   +-- HttpAiService.cs
    |   +-- HttpDashboardService.cs
    +-- State/
    |   +-- AppState.cs
    +-- wwwroot/
    |   +-- css/
    |   |   +-- app.css
    |   +-- js/
    |       +-- interop.js
    +-- Program.cs
    +-- MinuteSheetFFC.Web.csproj           (renamed from MinuteSheetFFC.csproj)

tests/
+-- MinuteSheetFFC.Domain.Tests/
|   +-- HierarchyResolverTests.cs
|   +-- RouteGeneratorTests.cs
|   +-- StageManagerTests.cs
|   +-- StateTransitionTests.cs
+-- MinuteSheetFFC.Application.Tests/
|   +-- EmployeeServiceTests.cs
|   +-- MinuteSheetServiceTests.cs
|   +-- WorkflowEngineServiceTests.cs
+-- MinuteSheetFFC.WorkflowEngine.Tests/
|   +-- HierarchyResolverTests.cs
|   +-- RouteGeneratorTests.cs
|   +-- StageManagerTests.cs
|   +-- ActionerValidatorTests.cs
|   +-- StateTransitionValidatorTests.cs
+-- MinuteSheetFFC.AiService.Tests/
|   +-- MockAiServiceTests.cs
+-- MinuteSheetFFC.Api.Tests/
|   +-- EmployeesControllerTests.cs
|   +-- MinuteSheetsControllerTests.cs
|   +-- WorkflowControllerTests.cs
|   +-- AiControllerTests.cs
+-- MinuteSheetFFC.Integration.Tests/
    +-- FullApprovalFlowTests.cs
    +-- RejectFlowTests.cs
    +-- ReturnResubmitFlowTests.cs
    +-- EdgeCaseFlowTests.cs
    +-- ApiTestFixture.cs

docs/
+-- ERD.md
+-- API_Reference.md
+-- User_Guide.md
+-- Setup_Guide.md
+-- Technical_Notes.md
```

---

## Appendix: Seed Data Plan

### Employees (20+ records)

| PNo | Name | Dept | Designation | ManagerPNo | IsActive | Test Purpose |
|-----|------|------|-------------|-----------|----------|--------------|
| 00001001 | Employee 001 | D001 | Assistant Manager | 00001010 | Active | Normal requester |
| 00001002 | Employee 002 | D001 | Officer | 00001010 | Active | Normal requester |
| 00001010 | Manager 001 | D001 | Manager | 00001020 | Active | Level 1 manager |
| 00001020 | Senior Manager 001 | D001 | Senior Manager | 00001030 | Active | Level 2 manager |
| 00001030 | General Manager 001 | D001 | General Manager | 00001040 | Active | Level 3 manager |
| 00001040 | Director 001 | D001 | Director | NULL | Active | Top level (no manager) |
| 00002001 | Employee 003 | D002 | Officer | 00002010 | Active | Different department |
| 00002010 | Manager 002 | D002 | Manager | 00002020 | Active | Dept 2 manager |
| 00002020 | Senior Manager 002 | D002 | Senior Manager | 00001040 | Active | Cross-dept reporting |
| 00003001 | Employee Missing Mgr | D003 | Officer | 99999999 | Active | **Missing manager** |
| 00003002 | Employee Self Mgr | D003 | Officer | 00003002 | Active | **Self manager** |
| 00004001 | Employee Circular A | D004 | Officer | 00004002 | Active | **Circular: A->B->C->A** |
| 00004002 | Employee Circular B | D004 | Officer | 00004003 | Active | **Circular chain** |
| 00004003 | Employee Circular C | D004 | Supervisor | 00004001 | Active | **Circular chain** |
| 00005001 | Employee Inactive Mgr | D005 | Officer | 00005010 | Active | Reports to inactive |
| 00005010 | Inactive Manager | D005 | Manager | 00001030 | **Inactive** | **Inactive manager** |
| 00006001 | Employee Short Chain | D006 | Officer | 00006010 | Active | **Only 1 manager** |
| 00006010 | Lone Manager | D006 | Manager | NULL | Active | **Insufficient levels** |
| 00007001 | Finance Reviewer | D007 | Finance Officer | 00001030 | Active | Finance review role |
| 00008001 | HR Admin | D008 | HR Officer | 00001030 | Active | Fallback admin |

### Request Types
| Code | Name |
|------|------|
| FIN | Financial |
| NONFIN | Non-Financial |
| HR | Human Resources |
| IT | Information Technology |
| PROC | Procurement |
| ADMIN | Administrative |

### Workflow Rules
| Type | Budget From | Budget To | Levels | Finance Review |
|------|------------|-----------|--------|---------------|
| NONFIN | 0 | NULL | 2 | No |
| FIN | 0 | 100,000 | 2 | Yes |
| FIN | 100,001 | 500,000 | 3 | Yes |
| FIN | 500,001 | NULL | 4 | Yes |
| HR | 0 | NULL | 2 | No |
| IT | 0 | 50,000 | 2 | No |
| IT | 50,001 | NULL | 3 | Yes |
| PROC | 0 | 200,000 | 2 | Yes |
| PROC | 200,001 | NULL | 3 | Yes |
| ADMIN | 0 | NULL | 1 | No |

---

## Key Architecture Decisions

1. **Separate API and Blazor projects**: The API is independently testable and could serve other frontends in the future. The Blazor frontend communicates via HTTP, matching a real-world deployment pattern.

2. **Clean Architecture layers**: Domain has zero dependencies. Application defines interfaces. Infrastructure implements persistence. This makes unit testing straightforward.

3. **Workflow Engine as dedicated project**: Keeps workflow logic isolated, testable, and reusable. The engine is pure business logic with no direct database dependencies (uses repository interfaces).

4. **AI Service behind interface**: Swappable between mock and real implementations. Mock uses keyword matching for demonstration; real implementation can use OpenAI/Azure OpenAI later.

5. **Simulated auth via header**: Avoids implementing real authentication while still enforcing actioner validation. The User Switcher page makes testing different user perspectives easy.

6. **EF Core with SQL Server LocalDB**: Full relational database with proper constraints, indexes, and referential integrity. SQLite can be used as a portable alternative.

7. **Blazor Server (not WASM)**: Simpler deployment, server-side rendering, no need for separate hosting. Direct access to server resources for prototype purposes.

---

*End of Implementation Plan*
