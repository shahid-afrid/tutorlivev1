using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TutorLiveMentor.Attributes
{
    /// <summary>
    /// Authorization attribute for Student access
    /// Validates session and ensures user is authenticated as a student
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class StudentAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check if action has [AllowAnonymous] attribute
            if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            {
                return; // Skip authorization
            }

            var studentId = context.HttpContext.Session.GetString("StudentId");
            var lastActivity = context.HttpContext.Session.GetString("LastActivity");

            // Check if session exists
            if (string.IsNullOrEmpty(studentId))
            {
                context.Result = new RedirectToActionResult("Login", "Student", null);
                return;
            }

            // Check session timeout (30 minutes of inactivity)
            if (!string.IsNullOrEmpty(lastActivity))
            {
                if (DateTime.TryParse(lastActivity, out DateTime lastActivityTime))
                {
                    if (DateTime.Now.Subtract(lastActivityTime).TotalMinutes > 30)
                    {
                        // Session expired
                        context.HttpContext.Session.Clear();
                        context.Result = new RedirectToActionResult("Login", "Student", new { expired = true });
                        return;
                    }
                }
            }

            // Update last activity timestamp
            context.HttpContext.Session.SetString("LastActivity", DateTime.Now.ToString("o"));
        }
    }

    /// <summary>
    /// Authorization attribute for Faculty access
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class FacultyAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check if action has [AllowAnonymous] attribute
            if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            var facultyId = context.HttpContext.Session.GetInt32("FacultyId");
            var lastActivity = context.HttpContext.Session.GetString("LastActivity");

            if (facultyId == null)
            {
                context.Result = new RedirectToActionResult("Login", "Faculty", null);
                return;
            }

            // Check session timeout
            if (!string.IsNullOrEmpty(lastActivity))
            {
                if (DateTime.TryParse(lastActivity, out DateTime lastActivityTime))
                {
                    if (DateTime.Now.Subtract(lastActivityTime).TotalMinutes > 30)
                    {
                        context.HttpContext.Session.Clear();
                        context.Result = new RedirectToActionResult("Login", "Faculty", new { expired = true });
                        return;
                    }
                }
            }

            context.HttpContext.Session.SetString("LastActivity", DateTime.Now.ToString("o"));
        }
    }

    /// <summary>
    /// Authorization attribute for Admin access
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AdminAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check if action has [AllowAnonymous] attribute
            if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            var adminId = context.HttpContext.Session.GetInt32("AdminId");
            var lastActivity = context.HttpContext.Session.GetString("LastActivity");

            if (adminId == null)
            {
                context.Result = new RedirectToActionResult("Login", "Admin", null);
                return;
            }

            // Check session timeout
            if (!string.IsNullOrEmpty(lastActivity))
            {
                if (DateTime.TryParse(lastActivity, out DateTime lastActivityTime))
                {
                    if (DateTime.Now.Subtract(lastActivityTime).TotalMinutes > 30)
                    {
                        context.HttpContext.Session.Clear();
                        context.Result = new RedirectToActionResult("Login", "Admin", new { expired = true });
                        return;
                    }
                }
            }

            context.HttpContext.Session.SetString("LastActivity", DateTime.Now.ToString("o"));
        }
    }

    /// <summary>
    /// Authorization attribute for CSEDS department admin access
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class CSEDSAdminAuthorizeAttribute : AdminAuthorizeAttribute
    {
        public new void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check if action has [AllowAnonymous] attribute
            if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            // First check admin authorization
            base.OnAuthorization(context);

            // If already redirected, return
            if (context.Result != null)
                return;

            // Check department
            var department = context.HttpContext.Session.GetString("AdminDepartment");
            if (string.IsNullOrEmpty(department) || 
                (department != "CSEDS" && department != "CSE(DS)"))
            {
                var controller = context.RouteData.Values["controller"]?.ToString() ?? "Admin";
                context.Result = new RedirectToActionResult("Login", controller, new { accessDenied = true });
            }
        }
    }

    /// <summary>
    /// Allows anonymous access to an action when the controller requires authorization
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AllowAnonymousAttribute : Attribute, IFilterMetadata
    {
        // This attribute is recognized by our authorization attributes
        // When present, they will skip authorization checks
    }
}
