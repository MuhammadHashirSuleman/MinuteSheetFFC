using MinuteSheetBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace MinuteSheetBackEnd.Controllers
{
    [Authorize] // Protects request data via JWT verification
    [RoutePrefix("api/minutesheet")]
    public class MinuteSheetController : ApiController
    {
        private readonly MinuteSheetDBEntities _context = new MinuteSheetDBEntities();

        // Helper to extract the Employee PNo from identity claims
        private string GetUserPNo()
        {
            var identity = User.Identity as ClaimsIdentity;
            return identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        /// <summary>
        /// GET: api/minutesheet/all
        /// Binds all rendering data components on the Minute Sheets table screen
        /// </summary>
        [HttpGet]
        [Route("all")]
        public IHttpActionResult GetAllRequests(
            [FromUri] string search = null,
            [FromUri] string status = null,
            [FromUri] string type = null,
            [FromUri] DateTime? startDate = null,
            [FromUri] DateTime? endDate = null,
            [FromUri] int page = 1,
            [FromUri] int rowsPerPage = 10)
        {
            string userPNo = GetUserPNo();
            if (string.IsNullOrEmpty(userPNo)) return Unauthorized();

            // Start query context
            var query = _context.MinuteSheetRequests.AsQueryable();

            // 1. Component: Search Input (Filters via Reference Number or Subject)
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.ReferenceNumber.Contains(search) || r.Subject.Contains(search));
            }

            // 2. Component: Status Dropdown Filter
            if (!string.IsNullOrEmpty(status) && status != "All Statuses")
            {
                query = query.Where(r => r.Status == status);
            }

            // 3. Component: Type Dropdown Filter 
            if (!string.IsNullOrEmpty(type) && type != "All Types")
            {
                query = query.Where(r => r.RequestTypes != null && r.RequestTypes.Name == type);
            }

            // 4. Component: Date Range Picker
            if (startDate.HasValue)
            {
                query = query.Where(r => r.CreatedAt >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                DateTime adjustedEnd = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(r => r.CreatedAt <= adjustedEnd);
            }

            // Calculate total matching entries before pagination for counter display
            int totalEntries = query.Count();

            // 5. Component: Main Data Grid Rows (Pagination applied)
            var pagedData = query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * rowsPerPage)
                .Take(rowsPerPage)
                .ToList();

            // Project structured results safely using verified property names
            // Fixed: Changed r.RequestTypes.TypeName to r.RequestTypes.Name
            var renderedRows = pagedData.Select(r => new
            {
                ID = r.ReferenceNumber,
                Type = r.RequestTypes != null ? r.RequestTypes.Name : "GENERAL",
                Subject = r.Subject,
                CreationDate = r.CreatedAt.ToString("MMM dd, yyyy"), // e.g., Oct 24, 2023
                Status = r.Status,
                CurrentActioner = r.Employees1 != null ? new
                {
                    Name = r.Employees1.Name,
                    Designation = r.Employees1.Designation
                } : null
            }).ToList();

            return Ok(new
            {
                TotalEntries = totalEntries,
                RowsPerPage = rowsPerPage,
                Data = renderedRows
            });
        }
            /// GET: api/minutesheet/init-form
            /// 1. Component: Requester Profile Header Info (Displays Logged-in User Data)
            /// </summary>
            [HttpGet]
            [Route("init-form")]
            public IHttpActionResult GetFormInitializationData()
            {
                string userPNo = GetUserPNo();
                if (string.IsNullOrEmpty(userPNo)) return Unauthorized();

                // Fixed: Pulling DepartmentName directly from the Employee object properties safely
                var employee = _context.Employees
                    .Where(e => e.PNo == userPNo)
                    .Select(e => new
                    {
                        e.Name,
                        e.Designation,
                        DepartmentName = e.DepartmentName ?? "General Administration"
                    })
                    .FirstOrDefault();

                if (employee == null) return NotFound();

                // Fetch Sheet Type options from DB to populate the "Sheet Type" Dropdown menu component dynamically
                var sheetTypes = _context.RequestTypes
                    .Where(t => t.IsActive)
                    .Select(t => new { t.Id, t.Name })
                    .ToList();

                return Ok(new
                {
                    Requester = employee,
                    SheetTypes = sheetTypes
                });
            }

            /// <summary>
            /// POST: api/minutesheet/create
            /// 2. Component: "Save Draft" & "Submit Sheet" Form Processing Logic
            /// </summary>
            [HttpPost]
            [Route("create")]
            public IHttpActionResult CreateSheet([FromBody] CreateMinuteSheetRequest model, [FromUri] bool isDraft = false)
            {
                string userPNo = GetUserPNo();
                if (string.IsNullOrEmpty(userPNo)) return Unauthorized();
                if (model == null || string.IsNullOrEmpty(model.Subject)) return BadRequest("Invalid sheet payload details.");

                // Generate consecutive sequential running reference sequence string format
                string yearSuffix = DateTime.Now.Year.ToString();
                int runningCount = _context.MinuteSheetRequests.Count() + 1;
                string generatedReference = $"MS-{yearSuffix}-{runningCount:D3}";

                // Initialize entity context mapping blocks
                var newRequest = new MinuteSheetRequests
                {
                    ReferenceNumber = generatedReference,
                    Subject = model.Subject,
                    Body = model.Body,
                    Priority = model.Priority,
                    IsConfidential = model.IsConfidential,
                    WorkflowMode = model.WorkflowMode,
                    Status = isDraft ? "DRAFT" : "SUBMITTED",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // Link structural model navigation entities directly to fix missing raw property binding gaps
                newRequest.Employees = _context.Employees.FirstOrDefault(e => e.PNo == userPNo);
                newRequest.RequestTypes = _context.RequestTypes.FirstOrDefault(t => t.Id == model.RequestTypeId);

                // Save initial transaction state block
                _context.MinuteSheetRequests.Add(newRequest);
                _context.SaveChanges();

                // 3. Component: Link drag and drop file attachments if present
                if (model.AttachmentFileNames != null && model.AttachmentFileNames.Any())
                {
                    foreach (var fileName in model.AttachmentFileNames)
                    {
                        // Database Path relative layout structure saved to DB
                        string dbStoragePath = "~/Uploads/MinuteSheets/" + generatedReference + "/" + fileName;

                        // Fixed: Changed FilePath to StoragePath to match model requirements
                        var attachment = new MinuteSheetAttachments
                        {
                            FileName = fileName,
                            FileType = System.IO.Path.GetExtension(fileName),
                            StoragePath = dbStoragePath,
                            UploadedAt = DateTime.Now
                        };

                        attachment.MinuteSheetRequests = newRequest;
                        attachment.Employees = _context.Employees.FirstOrDefault(e => e.PNo == userPNo);

                        _context.MinuteSheetAttachments.Add(attachment);
                    }
                    _context.SaveChanges();
                }

                return Ok(new { ReferenceNumber = generatedReference, Status = newRequest.Status, Message = "Minute sheet created successfully." });
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                base.Dispose(disposing);
            }
        }
    }
public class MinuteSheetListResponse
{
    public int TotalEntries { get; set; }
    public int RowsPerPage { get; set; }
    public dynamic Data { get; set; } // Holds the structured anonymous query list
}

public class CreateMinuteSheetRequest
{
    public string Subject { get; set; }
    public int RequestTypeId { get; set; }
    public string Priority { get; set; }
    public bool IsConfidential { get; set; }
    public string Body { get; set; }
    public string WorkflowMode { get; set; }
    public List<string> AttachmentFileNames { get; set; } // Tracks uploaded file references
}