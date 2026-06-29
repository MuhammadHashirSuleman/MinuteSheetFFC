-- ============================================================================
-- MINUTE SHEET / GENERAL APPROVAL WORKFLOW
-- DATABASE SCHEMA - Microsoft SQL Server
-- ============================================================================
-- Run Order: Execute this entire script top-to-bottom in SSMS or sqlcmd.
-- It creates the database, all tables, indexes, and constraints.
-- ============================================================================
-- Data types aligned with: FFC_Orbit_Employee_Template_Anonymized.xlsx
-- ============================================================================

-- ============================================================================
-- 1. CREATE DATABASE
-- ============================================================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'MinuteSheetDB')
BEGIN
    CREATE DATABASE [MinuteSheetDB];
END
GO

USE [MinuteSheetDB];
GO

-- ============================================================================
-- 2. DROP EXISTING TABLES (reverse dependency order, re-runnable)
-- ============================================================================
IF OBJECT_ID('dbo.AiAnalysisResults', 'U')      IS NOT NULL DROP TABLE dbo.AiAnalysisResults;
IF OBJECT_ID('dbo.WorkflowExceptions', 'U')      IS NOT NULL DROP TABLE dbo.WorkflowExceptions;
IF OBJECT_ID('dbo.WorkflowHistory', 'U')         IS NOT NULL DROP TABLE dbo.WorkflowHistory;
IF OBJECT_ID('dbo.WorkflowStages', 'U')          IS NOT NULL DROP TABLE dbo.WorkflowStages;
IF OBJECT_ID('dbo.MinuteSheetAttachments', 'U')  IS NOT NULL DROP TABLE dbo.MinuteSheetAttachments;
IF OBJECT_ID('dbo.MinuteSheetRequests', 'U')     IS NOT NULL DROP TABLE dbo.MinuteSheetRequests;
IF OBJECT_ID('dbo.WorkflowRules', 'U')           IS NOT NULL DROP TABLE dbo.WorkflowRules;
IF OBJECT_ID('dbo.RequestTypes', 'U')            IS NOT NULL DROP TABLE dbo.RequestTypes;
IF OBJECT_ID('dbo.RefreshTokens', 'U')           IS NOT NULL DROP TABLE dbo.RefreshTokens;
IF OBJECT_ID('dbo.UserRoles', 'U')               IS NOT NULL DROP TABLE dbo.UserRoles;
IF OBJECT_ID('dbo.Users', 'U')                   IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID('dbo.Roles', 'U')                   IS NOT NULL DROP TABLE dbo.Roles;
IF OBJECT_ID('dbo.Employees', 'U')               IS NOT NULL DROP TABLE dbo.Employees;
IF OBJECT_ID('dbo.Departments', 'U')             IS NOT NULL DROP TABLE dbo.Departments;
GO

-- ============================================================================
-- 3. DEPARTMENTS
-- ============================================================================
-- DepartmentId is INT matching Excel values like 14002001, 14002002, etc.
-- ============================================================================
CREATE TABLE dbo.Departments
(
    DepartmentId    INT             NOT NULL,
    Name            NVARCHAR(100)   NOT NULL,
    ShortName       VARCHAR(10)     NULL,
    IsActive        BIT             NOT NULL  DEFAULT 1,
    CreatedAt       DATETIME2(3)    NOT NULL  DEFAULT GETDATE(),
    UpdatedAt       DATETIME2(3)    NULL,

    CONSTRAINT PK_Departments PRIMARY KEY (DepartmentId)
);
GO

-- ============================================================================
-- 4. EMPLOYEES
-- ============================================================================
-- Column types matched to FFC_Orbit_Employee_Template_Anonymized.xlsx:
--   PNo / ManagerPNo : VARCHAR(8)  - 8-digit with leading zeros (e.g. 00010101)
--   OldPNo           : VARCHAR(10) - alphanumeric (e.g. OLD10101)
--   DepartmentId     : INT         - numeric (e.g. 14002001)
--   JobKey           : INT         - numeric (e.g. 50000001)
--   PositionId       : INT         - numeric (e.g. 14000001)
--   GroupId          : INT         - numeric (e.g. 14000001)
--   PAreaId          : INT         - numeric (e.g. 9001)
--   PSAreaId         : INT         - numeric (e.g. 9101)
--   CostCenter       : INT         - numeric (e.g. 900001001)
--   EmployeeGroup    : VARCHAR(5)  - short code (e.g. M)
--   EmployeeCategory : VARCHAR(5)  - short code (e.g. AH, AG)
--   Gender           : INT         - coded (1, 2)
--   MaritalStatus    : INT         - coded (0, 1)
--   AnnualLeaveBalance : INT       - whole number
--   IsActive (Active in SAP) : BIT - TRUE/FALSE
-- ============================================================================
CREATE TABLE dbo.Employees
(
    PNo                 VARCHAR(8)      NOT NULL,
    OldPNo              VARCHAR(10)     NULL,
    Name                NVARCHAR(200)   NOT NULL,
    Email               NVARCHAR(200)   NULL,
    Phone               VARCHAR(20)     NULL,
    CNIC                VARCHAR(20)     NULL,
    Gender              INT             NULL,
    MaritalStatus       INT             NULL,
    FatherName          NVARCHAR(200)   NULL,
    DOB                 DATE            NULL,
    HireDate            DATE            NULL,
    LastPromotionDate   DATE            NULL,
    RetirementDate      DATE            NULL,
    LeavingDate         DATE            NULL,

    -- Position / Title
    Designation         NVARCHAR(100)   NOT NULL,
    DesignationShort    VARCHAR(10)     NULL,
    JobDescription      NVARCHAR(200)   NULL,
    JobKey              INT             NULL,
    PositionId          INT             NULL,
    Grade               VARCHAR(10)     NULL,

    -- Department
    DepartmentId        INT             NOT NULL,
    DepartmentName      NVARCHAR(100)   NULL,
    DepartmentShort     VARCHAR(10)     NULL,

    -- Employee Group
    EmployeeGroup       VARCHAR(5)      NULL,
    EmployeeCategory    VARCHAR(5)      NULL,
    GroupId             INT             NULL,
    GroupDesc           NVARCHAR(100)   NULL,
    GroupShort          VARCHAR(10)     NULL,

    -- Personnel Area
    PAreaId             INT             NULL,
    PAreaDesc           NVARCHAR(100)   NULL,
    PSAreaId            INT             NULL,
    PSAreaDesc          NVARCHAR(100)   NULL,

    -- Cost Center
    CostCenter          INT             NULL,
    CostCenterDesc      NVARCHAR(100)   NULL,

    -- Hierarchy (core field)
    ManagerPNo          VARCHAR(8)      NULL,

    -- Status (Active in SAP)
    IsActive            BIT             NOT NULL  DEFAULT 1,

    -- Acting / Delegation (optional challenge)
    ActingPNo           VARCHAR(8)      NULL,
    ActingFrom          DATE            NULL,
    ActingTo            DATE            NULL,

    -- Leave
    AnnualLeaveBalance  INT             NULL,

    -- Timestamps
    CreatedAt           DATETIME2(3)    NOT NULL  DEFAULT GETDATE(),
    UpdatedAt           DATETIME2(3)    NULL,

    CONSTRAINT PK_Employees PRIMARY KEY (PNo),
    CONSTRAINT FK_Employees_Department FOREIGN KEY (DepartmentId) REFERENCES dbo.Departments(DepartmentId)

    -- NOTE: ManagerPNo FK is intentionally omitted to allow
    -- broken hierarchy test data (e.g. ManagerPNo = 00999999 which does not exist).
    -- The application layer validates hierarchy integrity.
);
GO

-- ============================================================================
-- 5. ROLES (Login/Registration system)
-- ============================================================================
CREATE TABLE dbo.Roles
(
    Id              INT             NOT NULL  IDENTITY(1,1),
    Name            VARCHAR(50)     NOT NULL,
    Description     NVARCHAR(200)   NULL,
    CreatedAt       DATETIME2(3)    NOT NULL  DEFAULT GETDATE(),

    CONSTRAINT PK_Roles PRIMARY KEY (Id),
    CONSTRAINT UQ_Roles_Name UNIQUE (Name)
);
GO

-- ============================================================================
-- 6. USERS (Login/Registration)
-- ============================================================================
CREATE TABLE dbo.Users
(
    Id                  INT             NOT NULL  IDENTITY(1,1),
    Username            VARCHAR(50)     NOT NULL,
    Email               NVARCHAR(200)   NOT NULL,
    PasswordHash        NVARCHAR(500)   NOT NULL,
    EmployeePNo         VARCHAR(8)      NULL,
    IsActive            BIT             NOT NULL  DEFAULT 1,
    IsLocked            BIT             NOT NULL  DEFAULT 0,
    FailedLoginAttempts INT             NOT NULL  DEFAULT 0,
    LockoutEnd          DATETIME2(3)    NULL,
    LastLoginAt         DATETIME2(3)    NULL,
    PasswordChangedAt   DATETIME2(3)    NULL,
    CreatedAt           DATETIME2(3)    NOT NULL  DEFAULT GETDATE(),
    UpdatedAt           DATETIME2(3)    NULL,

    CONSTRAINT PK_Users PRIMARY KEY (Id),
    CONSTRAINT UQ_Users_Username UNIQUE (Username),
    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    CONSTRAINT FK_Users_Employee FOREIGN KEY (EmployeePNo) REFERENCES dbo.Employees(PNo)
);
GO

-- ============================================================================
-- 7. USER-ROLE ASSIGNMENTS (Many-to-Many junction)
-- ============================================================================
CREATE TABLE dbo.UserRoles
(
    UserId          INT             NOT NULL,
    RoleId          INT             NOT NULL,
    AssignedAt      DATETIME2(3)    NOT NULL  DEFAULT GETDATE(),
    AssignedBy      INT             NULL,

    CONSTRAINT PK_UserRoles PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_UserRoles_User FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE CASCADE,
    CONSTRAINT FK_UserRoles_Role FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id) ON DELETE CASCADE
);
GO

-- ============================================================================
-- 8. REFRESH TOKENS (JWT token management)
-- ============================================================================
CREATE TABLE dbo.RefreshTokens
(
    Id              INT             NOT NULL  IDENTITY(1,1),
    UserId          INT             NOT NULL,
    Token           VARCHAR(500)    NOT NULL,
    ExpiresAt       DATETIME2(3)    NOT NULL,
    CreatedAt       DATETIME2(3)    NOT NULL  DEFAULT GETDATE(),
    RevokedAt       DATETIME2(3)    NULL,
    ReplacedByToken VARCHAR(500)    NULL,
    CreatedByIp     VARCHAR(50)     NULL,

    CONSTRAINT PK_RefreshTokens PRIMARY KEY (Id),
    CONSTRAINT FK_RefreshTokens_User FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE CASCADE
);
GO

-- ============================================================================
-- 9. REQUEST TYPES
-- ============================================================================
CREATE TABLE dbo.RequestTypes
(
    Id              INT             NOT NULL  IDENTITY(1,1),
    Code            VARCHAR(20)     NOT NULL,
    Name            NVARCHAR(100)   NOT NULL,
    Description     NVARCHAR(500)   NULL,
    IsActive        BIT             NOT NULL  DEFAULT 1,
    CreatedAt       DATETIME2(3)    NOT NULL  DEFAULT GETDATE(),

    CONSTRAINT PK_RequestTypes PRIMARY KEY (Id),
    CONSTRAINT UQ_RequestTypes_Code UNIQUE (Code)
);
GO

-- ============================================================================
-- 10. WORKFLOW RULES
-- ============================================================================
CREATE TABLE dbo.WorkflowRules
(
    Id                      INT             NOT NULL  IDENTITY(1,1),
    RequestTypeId           INT             NOT NULL,
    BudgetFrom              DECIMAL(18,2)   NOT NULL  DEFAULT 0,
    BudgetTo                DECIMAL(18,2)   NULL,       -- NULL = no upper limit
    RequiredManagerLevels   INT             NOT NULL,
    RequiresFinanceReview   BIT             NOT NULL  DEFAULT 0,
    FallbackBehavior        VARCHAR(20)     NOT NULL  DEFAULT 'WARN',
    FinalAction             VARCHAR(20)     NOT NULL  DEFAULT 'APPROVE',
    IsActive                BIT             NOT NULL  DEFAULT 1,
    CreatedAt               DATETIME2(3)    NOT NULL  DEFAULT GETDATE(),
    UpdatedAt               DATETIME2(3)    NULL,

    CONSTRAINT PK_WorkflowRules PRIMARY KEY (Id),
    CONSTRAINT FK_WorkflowRules_RequestType FOREIGN KEY (RequestTypeId) REFERENCES dbo.RequestTypes(Id),
    CONSTRAINT CK_WorkflowRules_BudgetRange CHECK (BudgetTo IS NULL OR BudgetTo >= BudgetFrom),
    CONSTRAINT CK_WorkflowRules_Levels CHECK (RequiredManagerLevels >= 1),
    CONSTRAINT CK_WorkflowRules_Fallback CHECK (FallbackBehavior IN ('WARN','SKIP','ROUTE_TO_ADMIN'))
);
GO

-- ============================================================================
-- 11. MINUTE SHEET REQUESTS
-- ============================================================================
CREATE TABLE dbo.MinuteSheetRequests
(
    Id                  INT             NOT NULL  IDENTITY(1,1),
    ReferenceNumber     VARCHAR(30)     NOT NULL,
    Subject             NVARCHAR(500)   NOT NULL,
    Body                NVARCHAR(MAX)   NOT NULL,
    RequestTypeId       INT             NOT NULL,
    EstimatedBudget     DECIMAL(18,2)   NULL,
    Priority            VARCHAR(10)     NOT NULL  DEFAULT 'NORMAL',
    IsConfidential      BIT             NOT NULL  DEFAULT 0,
    WorkflowMode        VARCHAR(10)     NOT NULL,
    Status              VARCHAR(20)     NOT NULL  DEFAULT 'DRAFT',
    RequesterPNo        VARCHAR(8)      NOT NULL,
    CurrentActionerPNo  VARCHAR(8)      NULL,
    CurrentStageOrder   INT             NULL,
    SubmittedAt         DATETIME2(3)    NULL,
    CompletedAt         DATETIME2(3)    NULL,
    CreatedAt           DATETIME2(3)    NOT NULL  DEFAULT GETDATE(),
    UpdatedAt           DATETIME2(3)    NULL,

    CONSTRAINT PK_MinuteSheetRequests PRIMARY KEY (Id),
    CONSTRAINT UQ_MinuteSheet_RefNum UNIQUE (ReferenceNumber),
    CONSTRAINT FK_MinuteSheet_RequestType FOREIGN KEY (RequestTypeId) REFERENCES dbo.RequestTypes(Id),
    CONSTRAINT FK_MinuteSheet_Requester FOREIGN KEY (RequesterPNo) REFERENCES dbo.Employees(PNo),
    CONSTRAINT FK_MinuteSheet_Actioner FOREIGN KEY (CurrentActionerPNo) REFERENCES dbo.Employees(PNo),
    CONSTRAINT CK_MinuteSheet_Priority CHECK (Priority IN ('LOW','NORMAL','HIGH','URGENT')),
    CONSTRAINT CK_MinuteSheet_Mode CHECK (WorkflowMode IN ('MANUAL','DYNAMIC','HYBRID')),
    CONSTRAINT CK_MinuteSheet_Status CHECK (Status IN ('DRAFT','SUBMITTED','IN_REVIEW','APPROVED','REJECTED','RETURNED','CANCELLED','RESUBMITTED'))
);
GO

-- ============================================================================
-- 12. MINUTE SHEET ATTACHMENTS
-- ============================================================================
CREATE TABLE dbo.MinuteSheetAttachments
(
    Id              INT             NOT NULL  IDENTITY(1,1),
    MinuteSheetId   INT             NOT NULL,
    FileName        NVARCHAR(255)   NOT NULL,
    FileType        VARCHAR(100)    NULL,
    FileSize        BIGINT          NULL,
    StoragePath     NVARCHAR(500)   NULL,
    UploadedByPNo   VARCHAR(8)      NOT NULL,
    UploadedAt      DATETIME2(3)    NOT NULL  DEFAULT GETDATE(),

    CONSTRAINT PK_MinuteSheetAttachments PRIMARY KEY (Id),
    CONSTRAINT FK_Attachment_MinuteSheet FOREIGN KEY (MinuteSheetId) REFERENCES dbo.MinuteSheetRequests(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Attachment_Uploader FOREIGN KEY (UploadedByPNo) REFERENCES dbo.Employees(PNo)
);
GO

-- ============================================================================
-- 13. WORKFLOW STAGES
-- ============================================================================
CREATE TABLE dbo.WorkflowStages
(
    Id                      INT             NOT NULL  IDENTITY(1,1),
    MinuteSheetId           INT             NOT NULL,
    StageOrder              INT             NOT NULL,
    ActionerPNo             VARCHAR(8)      NOT NULL,
    ActionerName            NVARCHAR(200)   NOT NULL,
    ActionerDesignation     NVARCHAR(100)   NULL,
    ActionType              VARCHAR(20)     NOT NULL,
    Status                  VARCHAR(20)     NOT NULL  DEFAULT 'PENDING',
    Action                  VARCHAR(20)     NULL,
    Remarks                 NVARCHAR(1000)  NULL,
    ActionedAt              DATETIME2(3)    NULL,
    Source                  VARCHAR(10)     NOT NULL,
    CreatedAt               DATETIME2(3)    NOT NULL  DEFAULT GETDATE(),

    CONSTRAINT PK_WorkflowStages PRIMARY KEY (Id),
    CONSTRAINT FK_Stage_MinuteSheet FOREIGN KEY (MinuteSheetId) REFERENCES dbo.MinuteSheetRequests(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Stage_Actioner FOREIGN KEY (ActionerPNo) REFERENCES dbo.Employees(PNo),
    CONSTRAINT CK_Stage_ActionType CHECK (ActionType IN ('REVIEW','APPROVE','FINANCE_REVIEW')),
    CONSTRAINT CK_Stage_Status CHECK (Status IN ('PENDING','ACTIVE','COMPLETED','SKIPPED')),
    CONSTRAINT CK_Stage_Action CHECK (Action IS NULL OR Action IN ('REVIEWED','APPROVED','REJECTED','RETURNED')),
    CONSTRAINT CK_Stage_Source CHECK (Source IN ('MANUAL','DYNAMIC','HYBRID')),
    CONSTRAINT UQ_Stage_Order UNIQUE (MinuteSheetId, StageOrder)
);
GO

-- ============================================================================
-- 14. WORKFLOW HISTORY (audit trail)
-- ============================================================================
CREATE TABLE dbo.WorkflowHistory
(
    Id              INT             NOT NULL  IDENTITY(1,1),
    MinuteSheetId   INT             NOT NULL,
    ActionerPNo     VARCHAR(8)      NOT NULL,
    ActionerName    NVARCHAR(200)   NOT NULL,
    Action          VARCHAR(20)     NOT NULL,
    PreviousStatus  VARCHAR(20)     NOT NULL,
    NewStatus       VARCHAR(20)     NOT NULL,
    Remarks         NVARCHAR(1000)  NULL,
    StageOrder      INT             NULL,
    Timestamp       DATETIME2(3)    NOT NULL  DEFAULT GETDATE(),

    CONSTRAINT PK_WorkflowHistory PRIMARY KEY (Id),
    CONSTRAINT FK_History_MinuteSheet FOREIGN KEY (MinuteSheetId) REFERENCES dbo.MinuteSheetRequests(Id) ON DELETE CASCADE,
    CONSTRAINT FK_History_Actioner FOREIGN KEY (ActionerPNo) REFERENCES dbo.Employees(PNo),
    CONSTRAINT CK_History_Action CHECK (Action IN ('CREATE','SUBMIT','REVIEW','APPROVE','REJECT','RETURN','RESUBMIT','CANCEL'))
);
GO

-- ============================================================================
-- 15. WORKFLOW EXCEPTIONS
-- ============================================================================
CREATE TABLE dbo.WorkflowExceptions
(
    Id              INT             NOT NULL  IDENTITY(1,1),
    MinuteSheetId   INT             NULL,
    EmployeePNo     VARCHAR(8)      NULL,
    ExceptionType   VARCHAR(30)     NOT NULL,
    Description     NVARCHAR(500)   NOT NULL,
    Severity        VARCHAR(10)     NOT NULL,
    IsResolved      BIT             NOT NULL  DEFAULT 0,
    ResolvedByPNo   VARCHAR(8)      NULL,
    ResolvedAt      DATETIME2(3)    NULL,
    CreatedAt       DATETIME2(3)    NOT NULL  DEFAULT GETDATE(),

    CONSTRAINT PK_WorkflowExceptions PRIMARY KEY (Id),
    CONSTRAINT FK_Exception_MinuteSheet FOREIGN KEY (MinuteSheetId) REFERENCES dbo.MinuteSheetRequests(Id),
    CONSTRAINT FK_Exception_Employee FOREIGN KEY (EmployeePNo) REFERENCES dbo.Employees(PNo),
    CONSTRAINT CK_Exception_Type CHECK (ExceptionType IN ('MISSING_MANAGER','INACTIVE_MANAGER','SELF_MANAGER','CIRCULAR_HIERARCHY','INSUFFICIENT_LEVELS','DUPLICATE_APPROVER')),
    CONSTRAINT CK_Exception_Severity CHECK (Severity IN ('WARNING','ERROR','CRITICAL'))
);
GO

-- ============================================================================
-- 16. AI ANALYSIS RESULTS
-- ============================================================================
CREATE TABLE dbo.AiAnalysisResults
(
    Id                      INT             NOT NULL  IDENTITY(1,1),
    MinuteSheetId           INT             NOT NULL,
    Summary                 NVARCHAR(2000)  NULL,
    DetectedBudget          DECIMAL(18,2)   NULL,
    Impact                  NVARCHAR(1000)  NULL,
    Beneficiaries           NVARCHAR(1000)  NULL,       -- JSON array
    Urgency                 VARCHAR(20)     NULL,
    RiskLevel               VARCHAR(20)     NULL,
    SuggestedCategory       VARCHAR(50)     NULL,
    SuggestedSubject        NVARCHAR(500)   NULL,
    MissingInformation      NVARCHAR(2000)  NULL,       -- JSON array
    RiskFlags               NVARCHAR(2000)  NULL,       -- JSON array
    SuggestedRoute          NVARCHAR(2000)  NULL,       -- JSON array
    ReviewerChecklist       NVARCHAR(2000)  NULL,       -- JSON array
    SuggestedWorkflowMode   VARCHAR(10)     NULL,
    SuggestedLevels         INT             NULL,
    AnalyzedAt              DATETIME2(3)    NOT NULL  DEFAULT GETDATE(),
    ModelUsed               VARCHAR(50)     NULL,
    RawResponse             NVARCHAR(MAX)   NULL,

    CONSTRAINT PK_AiAnalysisResults PRIMARY KEY (Id),
    CONSTRAINT FK_AiAnalysis_MinuteSheet FOREIGN KEY (MinuteSheetId) REFERENCES dbo.MinuteSheetRequests(Id) ON DELETE CASCADE
);
GO


-- ============================================================================
-- 17. INDEXES
-- ============================================================================

-- Employees
CREATE NONCLUSTERED INDEX IX_Employees_ManagerPNo    ON dbo.Employees(ManagerPNo);
CREATE NONCLUSTERED INDEX IX_Employees_DepartmentId  ON dbo.Employees(DepartmentId);
CREATE NONCLUSTERED INDEX IX_Employees_IsActive      ON dbo.Employees(IsActive);
CREATE NONCLUSTERED INDEX IX_Employees_Name          ON dbo.Employees(Name);

-- Users
CREATE NONCLUSTERED INDEX IX_Users_EmployeePNo       ON dbo.Users(EmployeePNo);
CREATE NONCLUSTERED INDEX IX_Users_IsActive          ON dbo.Users(IsActive);

-- RefreshTokens
CREATE NONCLUSTERED INDEX IX_RefreshTokens_UserId    ON dbo.RefreshTokens(UserId);
CREATE NONCLUSTERED INDEX IX_RefreshTokens_Token     ON dbo.RefreshTokens(Token);

-- MinuteSheetRequests
CREATE NONCLUSTERED INDEX IX_MinuteSheet_RequesterPNo     ON dbo.MinuteSheetRequests(RequesterPNo);
CREATE NONCLUSTERED INDEX IX_MinuteSheet_CurrentActioner   ON dbo.MinuteSheetRequests(CurrentActionerPNo);
CREATE NONCLUSTERED INDEX IX_MinuteSheet_Status            ON dbo.MinuteSheetRequests(Status);
CREATE NONCLUSTERED INDEX IX_MinuteSheet_RequestTypeId     ON dbo.MinuteSheetRequests(RequestTypeId);
CREATE NONCLUSTERED INDEX IX_MinuteSheet_CreatedAt         ON dbo.MinuteSheetRequests(CreatedAt DESC);

-- WorkflowStages
CREATE NONCLUSTERED INDEX IX_WorkflowStages_MinuteSheet    ON dbo.WorkflowStages(MinuteSheetId);
CREATE NONCLUSTERED INDEX IX_WorkflowStages_Actioner       ON dbo.WorkflowStages(ActionerPNo);
CREATE NONCLUSTERED INDEX IX_WorkflowStages_Status         ON dbo.WorkflowStages(Status);

-- WorkflowHistory
CREATE NONCLUSTERED INDEX IX_WorkflowHistory_MinuteSheet   ON dbo.WorkflowHistory(MinuteSheetId);
CREATE NONCLUSTERED INDEX IX_WorkflowHistory_Timestamp     ON dbo.WorkflowHistory(Timestamp DESC);

-- WorkflowExceptions
CREATE NONCLUSTERED INDEX IX_WorkflowExceptions_MinuteSheet ON dbo.WorkflowExceptions(MinuteSheetId);
CREATE NONCLUSTERED INDEX IX_WorkflowExceptions_Employee    ON dbo.WorkflowExceptions(EmployeePNo);

-- AiAnalysisResults
CREATE NONCLUSTERED INDEX IX_AiAnalysis_MinuteSheet         ON dbo.AiAnalysisResults(MinuteSheetId);

GO

PRINT '====================================================================';
PRINT 'Schema creation complete. All 14 tables, constraints, and indexes created.';
PRINT '====================================================================';
PRINT '';
PRINT 'Data types aligned with FFC_Orbit_Employee_Template_Anonymized.xlsx:';
PRINT '  PNo/ManagerPNo  = VARCHAR(8)   (8-digit with leading zeros)';
PRINT '  DepartmentId    = INT          (e.g. 14002001)';
PRINT '  JobKey          = INT          (e.g. 50000001)';
PRINT '  PositionId      = INT          (e.g. 14000001)';
PRINT '  GroupId          = INT          (e.g. 14000001)';
PRINT '  PAreaId          = INT          (e.g. 9001)';
PRINT '  PSAreaId         = INT          (e.g. 9101)';
PRINT '  CostCenter       = INT          (e.g. 900001001)';
PRINT '  EmployeeGroup    = VARCHAR(5)   (e.g. M)';
PRINT '  EmployeeCategory = VARCHAR(5)   (e.g. AH, AG)';
PRINT '  Gender           = INT          (1, 2)';
PRINT '  MaritalStatus    = INT          (0, 1)';
PRINT '  AnnualLeaveBalance = INT        (whole number)';
PRINT '  IsActive         = BIT          (TRUE/FALSE)';
PRINT '====================================================================';
GO
