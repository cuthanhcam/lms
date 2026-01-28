using System.Security.Claims;

namespace LMS.API.Extensions
{
    /// <summary>
    /// Extension methods for ClaimsPrincipal
    /// Helps extract user information from JWT token claims easily
    /// 
    /// Usage in Controller:
    ///   var userId = User.GetUserId();
    ///   var userRole = User.GetUserRole();
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Get User ID from JWT token claims
        /// Claim type: ClaimTypes.NameIdentifier
        /// </summary>
        /// <param name="user">ClaimsPrincipal from Controller (User property)</param>
        /// <returns>User ID (Guid) or Guid.Empty if not found</returns>
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        /// <summary>
        /// Get User Role from JWT token claims
        /// Claim type: ClaimTypes.Role
        /// </summary>
        /// <param name="user">ClaimsPrincipal from Controller (User property)</param>
        /// <returns>Role string (Admin/Instructor/Student) or empty string</returns>
        public static string GetUserRole(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        }

        /// <summary>
        /// Get User Email from JWT token claims
        /// Claim type: ClaimTypes.Email
        /// </summary>
        /// <param name="user">ClaimsPrincipal from Controller (User property)</param>
        /// <returns>Email string or empty string</returns>
        public static string GetUserEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        }

        /// <summary>
        /// Get UserName from JWT token claims
        /// Claim type: ClaimTypes.Name
        /// </summary>
        /// <param name="user">ClaimsPrincipal from Controller (User property)</param>
        /// <returns>UserName string or empty string</returns>
        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
        }
    }
}

