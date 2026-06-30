using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;

namespace FFC.MinutesSheetAPI.Api.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        // Pulling your key details dynamically from the updated web.config appSettings
        private readonly string jwtKey = ConfigurationManager.AppSettings["Jwt:Key"] ?? "FallbackSecretKeyThatIsLongEnoughToMeetRequirements123!";
        private readonly string jwtIssuer = ConfigurationManager.AppSettings["Jwt:Issuer"] ?? "FFC_Internal_Auth_Server";
        private readonly string jwtAudience = ConfigurationManager.AppSettings["Jwt:Audience"] ?? "FFC_Minute_Sheet_App";

        /// <summary>
        /// Component: "Sign In" Button Submit
        /// Purpose: Validates credentials and returns a 2-day JWT token.
        /// </summary>
        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.EmployeeId) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Employee ID and Password are required.");
            }

            // Hardcoded check for validation testing. 
            // Soon you can hook this up to your 'MinuteSheetDBEntities' database context!
            bool isValidUser = (request.EmployeeId == "test-pno" && request.Password == "password123");

            if (!isValidUser)
            {
                return Unauthorized();
            }

            // Credentials Valid -> Generate JWT Token valid for exactly 2 Days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, request.EmployeeId),
                    new Claim(ClaimTypes.Role, "Employee")
                }),
                Expires = DateTime.UtcNow.AddDays(2), // Strict 2-day limit constraint
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtIssuer,
                Audience = jwtAudience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new AuthResponse
            {
                Token = tokenString,
                Expiration = tokenDescriptor.Expires.Value,
                EmployeeName = "John Doe"
            });
        }

        /// <summary>
        /// Component: "Forgot password?" Link
        /// </summary>
        [HttpPost]
        [Route("forgot-password")]
        public IHttpActionResult ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.EmployeeId))
            {
                return BadRequest("Employee ID is required.");
            }

            return Ok(new { message = "Password reset instructions sent to your registered contact channel." });
        }

        /// <summary>
        /// Component: "Contact IT Support" Link
        /// </summary>
        [HttpGet]
        [Route("support-info")]
        public IHttpActionResult GetSupportContact()
        {
            var supportDetails = new
            {
                Email = "itsupport@ffc.com.pk",
                Extension = "4444",
                Timings = "08:00 AM - 05:00 PM"
            };
            return Ok(supportDetails);
        }
    }


    public class LoginRequest
    {
        public string EmployeeId { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class ForgotPasswordRequest
    {
        public string EmployeeId { get; set; }
    }

    public class AuthResponse
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string EmployeeName { get; set; }
    }
}