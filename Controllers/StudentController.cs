using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TutorLiveMentor.Models;

namespace TutorLiveMentor.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(StudentRegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Students.AnyAsync(s => s.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email is already registered.");
                    return View(model);
                }

                var student = new Student
                {
                    FullName = model.FullName,
                    RegdNumber = model.RegdNumber,
                    Year = model.Year,
                    Department = model.Department,
                    Email = model.Email,
                    Password = model.Password,
                    SelectedSubject = "" // Initialize property
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Registration successful! Please log in now.";
                return RedirectToAction("Login");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == model.Email && s.Password == model.Password);
            if (student == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Email or Password.");
                return View(model);
            }

            // Store student ID in session to track login state
            HttpContext.Session.SetInt32("StudentId", student.Id);

            return RedirectToAction("MainDashboard");
        }

        [HttpGet]
        public IActionResult MainDashboard()
        {
            // Protect this page from unauthorized access
            if (HttpContext.Session.GetInt32("StudentId") == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId == null)
            {
                return RedirectToAction("Login");
            }

            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.AssignedSubject)
                    .ThenInclude(asub => asub.Subject)
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.AssignedSubject)
                    .ThenInclude(asub => asub.Faculty)
                .FirstOrDefaultAsync(s => s.Id == studentId.Value);

            if (student == null)
            {
                return NotFound();
            }

            // Get all available subjects for the student's department and year
            var yearMap = new Dictionary<string, int> { { "I", 1 }, { "II", 2 }, { "III", 3 }, { "IV", 4 } };
            var studentYearKey = student.Year?.Replace(" Year", "")?.Trim() ?? "";
            
            var availableSubjects = new List<AssignedSubject>();
            if (yearMap.TryGetValue(studentYearKey, out int studentYear))
            {
                // Get subjects that are not full
                availableSubjects = await _context.AssignedSubjects
                   .Include(a => a.Subject)
                   .Include(a => a.Faculty)
                   .Where(a => a.Department == student.Department && a.Year == studentYear && a.SelectedCount < 60)
                   .ToListAsync();

                // Filter out subjects where student has already enrolled
                var enrolledSubjectIds = student.Enrollments?.Select(e => e.AssignedSubject.SubjectId).ToList() ?? new List<int>();
                availableSubjects = availableSubjects.Where(a => !enrolledSubjectIds.Contains(a.SubjectId)).ToList();
            }

            var viewModel = new StudentDashboardViewModel
            {
                Student = student,
                AvailableSubjectsGrouped = availableSubjects.GroupBy(s => s.Subject.Name)
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SelectSubject(int assignedSubjectId)
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId == null)
            {
                return RedirectToAction("Login");
            }

            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.AssignedSubject)
                    .ThenInclude(a => a.Subject)
                .FirstOrDefaultAsync(s => s.Id == studentId.Value);
            
            var assignedSubject = await _context.AssignedSubjects
                .Include(a => a.Subject)
                .Include(a => a.Faculty)
                .FirstOrDefaultAsync(a => a.AssignedSubjectId == assignedSubjectId);

            if (student == null || assignedSubject == null)
            {
                return NotFound();
            }

            // Check if student has already enrolled in this specific assigned subject (same faculty)
            if (student.Enrollments.Any(e => e.AssignedSubjectId == assignedSubjectId))
            {
                TempData["ErrorMessage"] = "You have already enrolled with this faculty for this subject.";
                return RedirectToAction("Dashboard");
            }

            // Check if student has already enrolled in this subject with any faculty
            if (student.Enrollments.Any(e => e.AssignedSubject.SubjectId == assignedSubject.SubjectId))
            {
                TempData["ErrorMessage"] = $"You have already enrolled in {assignedSubject.Subject.Name} with another faculty.";
                return RedirectToAction("Dashboard");
            }

            if (assignedSubject.SelectedCount >= 60)
            {
                TempData["ErrorMessage"] = "This subject is already full.";
                return RedirectToAction("Dashboard");
            }

            // Create a new enrollment
            var enrollment = new StudentEnrollment
            {
                StudentId = student.Id,
                AssignedSubjectId = assignedSubject.AssignedSubjectId
            };
            _context.StudentEnrollments.Add(enrollment);

            // Update selected subjects list (comma-separated)
            var enrolledSubjects = student.Enrollments?.Select(e => e.AssignedSubject.Subject.Name).ToList() ?? new List<string>();
            enrolledSubjects.Add(assignedSubject.Subject.Name);
            student.SelectedSubject = string.Join(", ", enrolledSubjects.Distinct());

            assignedSubject.SelectedCount++;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Successfully enrolled in {assignedSubject.Subject.Name} with {assignedSubject.Faculty.Name}.";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public async Task<IActionResult> UnenrollSubject(int assignedSubjectId)
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId == null)
            {
                return RedirectToAction("Login");
            }

            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.AssignedSubject)
                    .ThenInclude(a => a.Subject)
                .FirstOrDefaultAsync(s => s.Id == studentId.Value);

            if (student == null)
            {
                return NotFound();
            }

            var enrollment = student.Enrollments.FirstOrDefault(e => e.AssignedSubjectId == assignedSubjectId);
            if (enrollment == null)
            {
                TempData["ErrorMessage"] = "You are not enrolled in this subject.";
                return RedirectToAction("Dashboard");
            }

            var assignedSubject = await _context.AssignedSubjects
                .Include(a => a.Subject)
                .FirstOrDefaultAsync(a => a.AssignedSubjectId == assignedSubjectId);

            if (assignedSubject != null)
            {
                assignedSubject.SelectedCount = Math.Max(0, assignedSubject.SelectedCount - 1);
            }

            _context.StudentEnrollments.Remove(enrollment);

            // Update selected subjects list
            var remainingEnrollments = student.Enrollments.Where(e => e.AssignedSubjectId != assignedSubjectId).ToList();
            var enrolledSubjects = remainingEnrollments.Select(e => e.AssignedSubject.Subject.Name).ToList();
            student.SelectedSubject = enrolledSubjects.Any() ? string.Join(", ", enrolledSubjects.Distinct()) : "";

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Successfully unenrolled from {assignedSubject?.Subject.Name}.";
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId == null)
            {
                return RedirectToAction("Login");
            }
            var student = await _context.Students.FindAsync(studentId.Value);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Student model)
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId == null)
            {
                return RedirectToAction("Login");
            }

            // Ensure the model ID matches the logged-in student's ID
            if (model.Id != studentId.Value)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var studentToUpdate = await _context.Students.FindAsync(studentId.Value);
                if (studentToUpdate == null)
                {
                    return NotFound();
                }

                studentToUpdate.FullName = model.FullName;
                studentToUpdate.Year = model.Year;
                studentToUpdate.Department = model.Department;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Profile updated!";
                return RedirectToAction("Dashboard");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }

    // View model for the dashboard
    public class StudentDashboardViewModel
    {
        public Student Student { get; set; }
        public IEnumerable<IGrouping<string, AssignedSubject>> AvailableSubjectsGrouped { get; set; }
    }
}