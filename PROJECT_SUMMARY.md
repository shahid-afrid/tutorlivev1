# TutorLiveMentor - Complete Project Summary

## ?? Project Overview
**TutorLiveMentor** is an ASP.NET Core Razor Pages application that allows students to select their preferred faculty for each subject. The system features real-time updates, department filtering, and competitive faculty selection with enrollment limits.

### Technology Stack
| Component | Version |
|-----------|---------|
| .NET SDK | 9.0.305 |
| Target Framework | .NET 8.0 |
| C# Version | 12.0 |
| Entity Framework Core | 9.0.9 |
| SQL Server | LocalDB/Express |
| Visual Studio | 2022 17.14.13 |
| SignalR | Real-time updates |

---

## ?? Core Features

### 1. **User Roles & Authentication**
- **Admin**: Full system management, reports, faculty/student/subject CRUD
- **Faculty**: Profile management, view assigned subjects, see enrolled students
- **Student**: Subject selection, enrollment tracking, profile management

### 2. **Real-Time Updates (SignalR)**
- Live enrollment count updates across all connected clients
- Instant faculty selection availability updates
- No page refresh required for seat count changes

### 3. **Department Management**
- **Supported Departments**: CSE, CSE(DS)
- Automatic department normalization (CSE-DS ? CSE(DS))
- Department-specific filtering for students and faculty

### 4. **Enrollment System**
- **70-student limit** per faculty per subject
- Real-time seat availability tracking
- Enrollment timestamps (server-side UTC)
- Prevent duplicate enrollments

### 5. **Faculty Selection Schedule**
- Admin-controlled selection windows
- Start/End date-time enforcement
- Prevents selections outside allowed periods

### 6. **Security Features**
- Password hashing (BCrypt)
- Role-based authorization attributes
- Security headers middleware
- CSRF protection
- Session management with proper logout

---

## ?? Project Structure

```
TutorLiveMentor/
??? Controllers/
?   ??? AdminController.cs          # Admin dashboard & management
?   ??? AdminReportsController.cs   # Export & reporting functionality
?   ??? FacultyController.cs        # Faculty features
?   ??? StudentController.cs        # Student features
?   ??? DiagnosticController.cs     # System diagnostics
??? Models/
?   ??? Student.cs                  # Student entity
?   ??? Faculty.cs                  # Faculty entity
?   ??? Subject.cs                  # Subject entity
?   ??? AssignedSubject.cs         # Faculty-Subject mapping
?   ??? StudentEnrollment.cs       # Enrollment tracking
?   ??? FacultySelectionSchedule.cs # Selection window control
?   ??? AppDbContext.cs            # EF Core DbContext
??? Views/
?   ??? Admin/                     # Admin views
?   ??? Faculty/                   # Faculty views
?   ??? Student/                   # Student views
??? Services/
?   ??? PasswordHashService.cs     # BCrypt password hashing
?   ??? SignalRService.cs          # Real-time updates
??? Hubs/
?   ??? SelectionHub.cs            # SignalR hub
??? Middleware/
?   ??? SecurityHeadersMiddleware.cs # Security headers
??? Attributes/
?   ??? AuthorizationAttributes.cs # Role-based auth
??? Helpers/
?   ??? DepartmentNormalizer.cs    # Department standardization
??? Migrations/                    # EF Core migrations
```

---

## ?? Setup Instructions

### 1. Prerequisites
```bash
# Verify versions
dotnet --version          # Should be 9.0.305
dotnet ef --version       # Should be 9.0.9
```

### 2. Clone & Restore
```bash
git clone https://github.com/shahid-afrid/Working2
cd working2
dotnet restore
```

### 3. Database Configuration
Update `appsettings.json`:

**Option A - LocalDB (Development)**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TutorLiveMentorDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

**Option B - SQL Server Express**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=TutorLiveMentorDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### 4. Apply Migrations
```bash
dotnet ef database update
```

### 5. Run Application
```bash
dotnet run
```
or press `F5` in Visual Studio.

---

## ?? Database Schema

### Key Tables

**Students**
- StudentId (string, PK)
- FullName, Email, Password
- Year (int), Department (string)
- Enrollments (navigation)

**Faculty**
- FacultyId (int, PK)
- Name, Email, Password, Department
- AssignedSubjects (navigation)

**Subjects**
- SubjectId (int, PK)
- Name, Code
- AssignedSubjects (navigation)

**AssignedSubjects**
- AssignedSubjectId (int, PK)
- FacultyId, SubjectId
- Department, Year
- SelectedCount (int) - live enrollment tracking
- MaxStudents (30)

**StudentEnrollments**
- EnrollmentId (int, PK)
- StudentId, AssignedSubjectId
- EnrolledAt (DateTime UTC)

**FacultySelectionSchedule**
- ScheduleId (int, PK)
- StartDate, EndDate (DateTime)
- IsActive (bool)
- Description

---

## ?? Key Features & Fixes Implemented

### 1. **Real-Time Updates (SignalR)**
- ? Live enrollment count updates
- ? Instant seat availability refresh
- ? Multi-client synchronization
- **Hub**: `SelectionHub.cs`
- **Service**: `SignalRService.cs`

### 2. **Department Normalization**
- ? Automatic CSE-DS ? CSE(DS) conversion
- ? DepartmentNormalizer helper class
- ? Prevents subject visibility issues
- **File**: `Helpers/DepartmentNormalizer.cs`

### 3. **Enrollment Limits**
- ? 70-student maximum per faculty
- ? Real-time capacity checking
- ? Prevents over-enrollment
- **Validation**: StudentController.EnrollSubject

### 4. **Security Implementation**
- ? BCrypt password hashing
- ? Role-based authorization attributes
- ? Security headers middleware
- ? Proper logout functionality
- **Files**: 
  - `Services/PasswordHashService.cs`
  - `Attributes/AuthorizationAttributes.cs`
  - `Middleware/SecurityHeadersMiddleware.cs`

### 5. **Timestamp Handling**
- ? Server-side UTC timestamps
- ? Local time conversion in views
- ? Prevents timezone issues
- **Format**: `EnrolledAt.ToLocalTime().ToString("MMM dd, yyyy hh:mm:ss.fff tt")`

### 6. **Faculty Selection Schedule**
- ? Admin-controlled selection windows
- ? DateTime range enforcement
- ? UI blocking outside schedule
- **Model**: `FacultySelectionSchedule.cs`
- **View**: `Views/Admin/ManageFacultySelectionSchedule.cshtml`

### 7. **UI/UX Enhancements**
- ? Font Awesome icons (replaced emojis)
- ? Responsive design
- ? Glass-morphism styling
- ? Live count indicators
- ? Department-specific dashboards

### 8. **Admin Features**
- ? Department-specific dashboards (CSE, CSE(DS))
- ? Excel export functionality
- ? Faculty assignment management
- ? Student enrollment reports
- ? Real-time statistics

---

## ?? Testing Checklist

### Student Features
- [ ] Register new student with CSE(DS) department
- [ ] Login and verify dashboard
- [ ] View available subjects (filtered by dept/year)
- [ ] Select faculty for a subject
- [ ] Verify real-time count update
- [ ] Check enrollment in "My Selected Subjects"
- [ ] Try enrolling when at 30-student limit

### Faculty Features
- [ ] Login as faculty
- [ ] View profile and assigned subjects
- [ ] See list of enrolled students
- [ ] Verify student count accuracy

### Admin Features
- [ ] Login to admin dashboard
- [ ] Create/edit faculty selection schedule
- [ ] Manage subject assignments
- [ ] Export enrollment reports to Excel
- [ ] View department-specific dashboards

### Real-Time Testing
1. Open two browser windows
2. Login as different students
3. Both select same faculty
4. Verify both see count update instantly

---

## ?? Deployment Options

### Local Network (LAN)
```bash
# Run on LAN (accessible to devices on same network)
.\run_on_lan.bat
```

### Internet Access (ngrok)
```bash
# Run with ngrok tunnel
.\run_on_internet.bat

# Or use cloudflared alternative
.\run_on_cloudflare.bat
```

### Production Deployment
1. Update `appsettings.json` with production connection string
2. Set `ASPNETCORE_ENVIRONMENT=Production`
3. Run migrations on production database
4. Deploy to IIS/Azure/AWS

---

## ?? Common Issues & Solutions

### Issue: No subjects showing for students
**Solution**: 
- Verify department format (CSE-DS vs CSE(DS))
- Run department normalization
- Check AssignedSubjects table has matching records

### Issue: Real-time updates not working
**Solution**:
- Check SignalR connection in browser console
- Verify `SelectionHub` is registered in `Program.cs`
- Ensure JavaScript is enabled

### Issue: Enrollment fails at limit
**Solution**:
- This is expected behavior when 70 students enrolled
- Check `SelectedCount` in AssignedSubjects table
- Admin can adjust MaxStudents if needed

### Issue: Login fails
**Solution**:
- Verify password hashing is working
- Check database has user records
- Ensure session middleware is configured

### Issue: Timestamps showing wrong time
**Solution**:
- Use `EnrolledAt.ToLocalTime()` in views
- Database stores UTC, converts on display

---

## ?? Database Management

### Create Migration
```bash
dotnet ef migrations add MigrationName
```

### Update Database
```bash
dotnet ef database update
```

### Remove Last Migration
```bash
dotnet ef migrations remove
```

### Reset Database (Fresh Start)
```bash
# Delete database
dotnet ef database drop

# Recreate from migrations
dotnet ef database update
```

---

## ?? Security Best Practices

1. **Passwords**: Always use `PasswordHashService.HashPassword()` before saving
2. **Authorization**: Apply `[AdminOnly]`, `[FacultyOnly]`, `[StudentOnly]` attributes
3. **Session**: Configure timeout appropriately
4. **HTTPS**: Use HTTPS in production
5. **Connection Strings**: Use environment variables for sensitive data

---

## ?? Performance Optimizations

1. **SignalR**: Uses persistent connections for real-time updates
2. **EF Core**: Eager loading with `.Include()` for related data
3. **Caching**: Consider Redis for large-scale deployments
4. **Indexing**: Database indexes on frequently queried columns

---

## ?? Student Department Formats

### Accepted Input Formats
- `CSE-DS` ? Normalized to `CSE(DS)`
- `CSE DS` ? Normalized to `CSE(DS)`
- `CSEDS` ? Normalized to `CSE(DS)`
- `CSE(DS)` ? Already correct
- `CSE` ? Remains `CSE`

**Note**: Registration now automatically normalizes department formats.

---

## ?? Support & Contribution

### Repository
- **Main**: https://github.com/shahid-afrid/Working2
- **Alternative**: https://github.com/shahid-afrid/working3

### Contributing
1. Fork the repository
2. Create feature branch
3. Make changes
4. Submit pull request

---

## ?? Version History

### Latest Updates
- ? Real-time SignalR updates
- ? Department normalization (CSE-DS ? CSE(DS))
- ? 70-student enrollment limit
- ? Faculty selection schedule enforcement
- ? Security enhancements (BCrypt, headers)
- ? Timestamp UTC handling
- ? Excel export for reports
- ? Font Awesome icon integration
- ? Proper logout functionality

---

## ?? Quick Start Commands

```bash
# Full reset and run
dotnet ef database drop
dotnet ef database update
dotnet run

# Check versions
dotnet --version
dotnet ef --version
dotnet list package

# Run with internet access
.\run_on_internet.bat

# Run on local network
.\run_on_lan.bat
```

---

**Last Updated**: 2025  
**Project Status**: ? Production Ready  
**Maintainer**: Shahid Afrid
