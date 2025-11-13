# ?? Quick Start - Admin Login

## Your Admin Credentials (Ready to Use!)

### Primary CSE(DS) Admin Account

```
??????????????????????????????????????????
  LOGIN URL: https://localhost:5001/Admin/Login
??????????????????????????????????????????

  ?? Email:    cseds@rgmcet.edu.in
  ?? Password: 9059530688
  ?? Department: CSE(DS)

??????????????????????????????????????????
```

---

## ?? How to Login (First Time)

### Step 1: Start the Application

**Option A: Using PowerShell Script**
```powershell
.\run_on_lan.ps1
```

**Option B: Using Command Line**
```bash
dotnet run
```

**Option C: Visual Studio**
- Press F5 or click "Run"

### Step 2: Wait for Admin Seeding

Watch the console output for this message:

```
========================================
[ADMIN SEEDER] Checking for default admin accounts...
[ADMIN SEEDER] No admins found. Creating default accounts...
[ADMIN SEEDER] ? Successfully created default admin accounts:
========================================
CSE(DS) Admin (PRIMARY):
  Email: cseds@rgmcet.edu.in
  Password: 9059530688
========================================
```

### Step 3: Open Admin Login Page

Navigate to:
```
https://localhost:5001/Admin/Login
```

Or:
```
http://localhost:5000/Admin/Login
```

### Step 4: Enter Credentials

1. **Email:** `cseds@rgmcet.edu.in`
2. **Password:** `9059530688`
3. Click **Login**

### Step 5: Access Dashboard

After successful login, you'll be redirected to:
```
https://localhost:5001/Admin/CSEDSDashboard
```

---

## ?? What You'll See

### CSE(DS) Admin Dashboard Features

1. **?? Statistics**
   - Student count
   - Faculty count
   - Subject count
   - Enrollment count

2. **?? Faculty Management**
   - View all faculty
   - Add new faculty
   - Edit faculty details
   - Assign subjects to faculty

3. **?? Subject Management**
   - View all subjects
   - Add new subjects
   - Edit subject details
   - Manage faculty assignments

4. **????? Student Management**
   - View all students
   - Add new students
   - Edit student details
   - View enrollments

5. **?? Reports & Analytics**
   - Enrollment reports
   - Faculty assignments
   - Subject statistics

---

## ?? If Admins Already Exist

If you've run the application before and admins already exist:

```
[ADMIN SEEDER] Found 3 existing admin(s). Skipping seed.
```

**This means:**
- ? Admin accounts already exist in database
- ? Seeding is automatically skipped
- ? You can login with existing credentials

---

## ??? Troubleshooting

### Problem: "Invalid admin credentials"

**Solution 1:** Check if admins were created
```sql
SELECT * FROM Admins;
```

**Solution 2:** Manually create admin
```sql
INSERT INTO Admins (Email, Password, Department, CreatedDate)
VALUES ('cseds@rgmcet.edu.in', '9059530688', 'CSE(DS)', GETDATE());
```

### Problem: Application won't start

**Solution:** Check if database exists and connection string is correct in `appsettings.json`

### Problem: "Database not found"

**Solution:** Run migrations
```bash
dotnet ef database update
```

### Problem: Port already in use

**Solution:** Change ports in `appsettings.json`:
```json
"Kestrel": {
  "Endpoints": {
    "Http": { "Url": "http://0.0.0.0:5002" },
    "Https": { "Url": "https://0.0.0.0:5003" }
  }
}
```

---

## ?? Additional Admin Accounts (Optional)

### CSE Admin (Not Yet Active)
```
Email:    admin.cse@rgmcet.edu.in
Password: CSEAdmin@2024
Department: CSE
```
**Note:** Change this password after first login!

### Super Admin (System-Wide)
```
Email:    superadmin@rgmcet.edu.in
Password: SuperAdmin@2024
Department: CSE(DS)
```
**Note:** Change this password after first login!

---

## ?? Quick Access URLs

### Local Development
```
Admin Login:    https://localhost:5001/Admin/Login
CSE(DS) Dashboard: https://localhost:5001/Admin/CSEDSDashboard
Health Check:   https://localhost:5001/health
```

### Azure Production (After Deployment)
```
Admin Login:    https://your-app.azurewebsites.net/Admin/Login
CSE(DS) Dashboard: https://your-app.azurewebsites.net/Admin/CSEDSDashboard
Health Check:   https://your-app.azurewebsites.net/health
```

---

## ? Success Indicators

You'll know everything is working when:

1. ? Console shows admin seeding success message
2. ? Login page loads without errors
3. ? You can login with `cseds@rgmcet.edu.in` / `9059530688`
4. ? Dashboard loads and shows statistics
5. ? Navigation menu is accessible

---

## ?? Need Help?

1. **Check Console Logs** - Most issues are visible in startup logs
2. **Check Database** - Verify Admins table has records
3. **Check Connection String** - Ensure database connection is correct
4. **Review Implementation Summary** - See `ADMIN_SEEDER_IMPLEMENTATION_SUMMARY.md`
5. **Review Deployment Guide** - See `AZURE_DEPLOYMENT_ADMIN_GUIDE.md`

---

## ?? You're All Set!

Your admin credentials are built-in and ready to use. Just start the application and login!

**Primary Credentials:**
```
cseds@rgmcet.edu.in
9059530688
```

**Happy Managing! ??**

---

**Last Updated:** 2024
**Status:** ? Ready to Use
**Build Status:** ? Successful
