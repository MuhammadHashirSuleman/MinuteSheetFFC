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
    [Authorize] // Requires Bearer / JWT token validation
    [RoutePrefix("api/dashboard")]
    public class DashboardController : ApiController
    {
        private readonly MinuteSheetDBEntities _context = new MinuteSheetDBEntities();

        // Helper to extract the Employee PNo from the logged-in identity claims
        private string GetUserPNo()
        {
            var identity = User.Identity as ClaimsIdentity;
            return identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        /// <summary>
        /// GET: api/dashboard/metrics
        /// </summary>
        [HttpGet]
        [Route("metrics")]
        public IHttpActionResult GetMetrics()
        {
            string userPNo = GetUserPNo();
            if (string.IsNullOrEmpty(userPNo)) return Unauthorized();

            // Uses navigation properties instead of raw foreign key columns
            var myRequests = _context.MinuteSheetRequests
                .Count(r => r.Employees != null && r.Employees.PNo == userPNo);

            var pendingActions = _context.MinuteSheetRequests
                .Count(r => r.Employees1 != null && r.Employees1.PNo == userPNo && r.Status == "IN_REVIEW");

            var drafts = _context.MinuteSheetRequests
                .Count(r => r.Employees != null && r.Employees.PNo == userPNo && r.Status == "DRAFT");

            var approved = _context.MinuteSheetRequests
                .Count(r => r.Employees != null && r.Employees.PNo == userPNo && r.Status == "APPROVED");

            return Ok(new
            {
                MyRequestsCount = myRequests,
                PendingActionsCount = pendingActions,
                DraftsCount = drafts,
                ApprovedCount = approved
            });
        }

        /// <summary>
        /// GET: api/dashboard/pending-actions
        /// </summary>
        [HttpGet]
        [Route("pending-actions")]
        public IHttpActionResult GetPendingActions()
        {
            string userPNo = GetUserPNo();
            if (string.IsNullOrEmpty(userPNo)) return Unauthorized();

            var pendingList = _context.MinuteSheetRequests
                .Where(r => r.Employees1 != null && r.Employees1.PNo == userPNo && r.Status == "IN_REVIEW")
                .Select(r => new
                {
                    ReferenceNumber = r.ReferenceNumber,
                    Subject = r.Subject,
                    RequesterName = r.Employees != null ? r.Employees.Name : "Unknown",
                    Priority = r.Priority
                })
                .Take(5)
                .ToList();

            return Ok(pendingList);
        }

        /// <summary>
        /// GET: api/dashboard/recent-requests
        /// </summary>
        [HttpGet]
        [Route("recent-requests")]
        public IHttpActionResult GetRecentRequests()
        {
            string userPNo = GetUserPNo();
            if (string.IsNullOrEmpty(userPNo)) return Unauthorized();

            var recentRequests = _context.MinuteSheetRequests
                .Where(r => r.Employees != null && r.Employees.PNo == userPNo)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .ToList();

            var result = recentRequests.Select(r => new
            {
                r.ReferenceNumber,
                r.Subject,
                CurrentHolder = r.Employees1 != null ? r.Employees1.Designation : "System",
                r.Status
            }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// GET: api/dashboard/aging-approvals
        /// </summary>
        [HttpGet]
        [Route("aging-approvals")]
        public IHttpActionResult GetAgingApprovals()
        {
            DateTime thresholdDate = DateTime.Now.AddDays(-3);

            var rawAgingData = _context.MinuteSheetRequests
                .Where(r => (r.Status == "IN_REVIEW" || r.Status == "SUBMITTED") && r.UpdatedAt < thresholdDate)
                .Select(r => new
                {
                    r.ReferenceNumber,
                    r.Subject,
                    TargetDate = r.UpdatedAt ?? r.CreatedAt
                })
                .ToList();

            var agingList = rawAgingData.Select(r => new
            {
                r.ReferenceNumber,
                r.Subject,
                DaysDelayed = (DateTime.Now - r.TargetDate).Days
            })
            .OrderByDescending(a => a.DaysDelayed)
            .Take(3)
            .ToList();

            return Ok(agingList);
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