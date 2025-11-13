# TutorLiveMentor - Azure Deployment Guide

## ?? Default Admin Credentials

When the application is deployed for the first time, **default admin accounts are automatically created**. These credentials are configured in the `AdminSeeder.cs` service.

### ?? Default Admin Accounts

#### 1. CSE(DS) Department Admin (PRIMARY ACCOUNT)
```
Email:    cseds@rgmcet.edu.in
Password: 9059530688
Department: CSE(DS)
```
**Purpose:** Primary admin account for managing CSE(DS) department faculty, subjects, and students

---

#### 2. CSE Department Admin (Future Use)
```
Email:    admin.cse@rgmcet.edu.in
Password: CSEAdmin@2024
Department: CSE
```
**Purpose:** For future CSE department management
**?? CHANGE THIS PASSWORD AFTER FIRST LOGIN**

---

#### 3. Super Admin (System-Wide)
```
Email:    superadmin@rgmcet.edu.in
Password: SuperAdmin@2024
Department: CSE(DS) (can access all departments)
```
**Purpose:** System-wide administration and management
**?? CHANGE THIS PASSWORD AFTER FIRST LOGIN**

---

## ?? Security Instructions

### ?? **IMPORTANT: Protect Your Credentials!**

1. **CSE(DS) Primary Account:** Uses production credentials (cseds@rgmcet.edu.in / 9059530688)
   - This is your main admin account
   - Keep these credentials secure
   - Only share with authorized CSE(DS) administrators

2. **CSE & Super Admin Accounts:** Use temporary passwords
   - **Change these passwords immediately** after first login
   - These are placeholder accounts for future expansion

### How to Change Admin Password (After Login)

Once admin profile management is implemented:
1. Login with your credentials
2. Go to Admin Profile page
3. Use "Change Password" feature
4. Follow secure password guidelines

### Manual Password Change (Database)

If you need to change admin passwords directly in the database:

```sql
-- Connect to your Azure SQL Database
-- Update password for specific admin
UPDATE Admins 
SET Password = 'YourNewSecurePassword'
WHERE Email = 'admin.cse@rgmcet.edu.in';

-- Example: Change Super Admin password
UPDATE Admins 
SET Password = 'NewSuperAdminPass@2024'
WHERE Email = 'superadmin@rgmcet.edu.in';
```

**Note:** Do NOT change the CSE(DS) primary admin password unless necessary!

---

## ?? Azure Deployment Steps

### Prerequisites
- Azure Account
- Azure SQL Database
- Azure App Service (or Container)

### Step 1: Configure Connection String

Update `appsettings.json` or use Azure App Service Configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:your-server.database.windows.net,1433;Initial Catalog=TutorLiveMentorDb;Persist Security Info=False;User ID=your-username;Password=your-password;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

**For Azure App Service:**
1. Go to Azure Portal ? Your App Service
2. Navigate to **Configuration** ? **Connection strings**
3. Add connection string:
   - Name: `DefaultConnection`
   - Value: Your Azure SQL connection string
   - Type: `SQLServer`

### Step 2: Enable Database Migrations

The application automatically runs database migrations on startup:

```csharp
await context.Database.MigrateAsync();
```

This ensures all tables are created on first deployment.

### Step 3: Admin Account Seeding

On first startup, the application will:
1. ? Check if any admin accounts exist
2. ? If none exist, create the 3 default admin accounts
3. ? Log the credentials to the application logs

**View Logs in Azure:**
- Go to Azure Portal ? Your App Service
- Navigate to **Monitoring** ? **Log stream**
- Look for `[ADMIN SEEDER]` messages

### Step 4: Deploy Application

**Option A: Deploy via GitHub Actions (Recommended)**
```yaml
# .github/workflows/azure-deploy.yml
name: Deploy to Azure

on:
  push:
    branches: [ main ]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      
      - name: Set up .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '8.0.x'
      
      - name: Build
        run: dotnet build --configuration Release
      
      - name: Publish
        run: dotnet publish -c Release -o ${{env.DOTNET_ROOT}}/myapp
      
      - name: Deploy to Azure
        uses: azure/webapps-deploy@v2
        with:
          app-name: 'your-app-name'
          publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
          package: ${{env.DOTNET_ROOT}}/myapp
```

**Option B: Deploy via Azure CLI**
```bash
# Login to Azure
az login

# Create resource group (if needed)
az group create --name TutorLiveMentorRG --location eastus

# Create App Service plan
az appservice plan create --name TutorLiveMentorPlan --resource-group TutorLiveMentorRG --sku B1

# Create Web App
az webapp create --name TutorLiveMentor --resource-group TutorLiveMentorRG --plan TutorLiveMentorPlan --runtime "DOTNET|8.0"

# Deploy from local folder
az webapp up --name TutorLiveMentor --resource-group TutorLiveMentorRG
```

**Option C: Deploy via Visual Studio**
1. Right-click project ? **Publish**
2. Choose **Azure** ? **Azure App Service (Windows)**
3. Select your subscription and app service
4. Click **Publish**

---

## ?? Environment Variables (Azure App Service Configuration)

Configure these in Azure Portal ? App Service ? Configuration ? Application settings:

| Key | Value | Description |
|-----|-------|-------------|
| `ASPNETCORE_ENVIRONMENT` | `Production` | Set to Production for deployment |
| `AllowedHosts` | `yourdomain.com` | Your domain name |
| `ServerSettings__ApplicationName` | `TutorLiveMentor` | Application name |
| `ServerSettings__ServerMode` | `Cloud` | Server mode |

---

## ?? Post-Deployment Testing

### 1. Health Check
```
https://your-app-name.azurewebsites.net/health
```
Should return: `Healthy`

### 2. Detailed Health Check
```
https://your-app-name.azurewebsites.net/health/ready
```
Should return JSON with database status

### 3. Admin Login
```
https://your-app-name.azurewebsites.net/Admin/Login
```
**Use the CSE(DS) PRIMARY credentials to login:**
- Email: `cseds@rgmcet.edu.in`
- Password: `9059530688`

### 4. Verify Admin Dashboard
After login, verify:
- ? Dashboard loads correctly
- ? Navigation works
- ? No JavaScript errors in console
- ? Session persists across page navigation

---

## ?? Monitoring Admin Seeding

Check Azure App Service logs to verify admin accounts were created:

```bash
# View live logs
az webapp log tail --name TutorLiveMentor --resource-group TutorLiveMentorRG

# Download logs
az webapp log download --name TutorLiveMentor --resource-group TutorLiveMentorRG
```

Look for these log messages:
```
[ADMIN SEEDER] Checking for default admin accounts...
[ADMIN SEEDER] No admins found. Creating default accounts...
[ADMIN SEEDER] ? Successfully created default admin accounts:
========================================
CSE(DS) Admin (PRIMARY):
  Email: cseds@rgmcet.edu.in
  Password: 9059530688
----------------------------------------
CSE Admin:
  Email: admin.cse@rgmcet.edu.in
  Password: CSEAdmin@2024
----------------------------------------
Super Admin:
  Email: superadmin@rgmcet.edu.in
  Password: SuperAdmin@2024
========================================
```

---

## ??? Troubleshooting

### Issue: Admin accounts not created

**Solution 1:** Check database connection
```bash
# Test database connection from Azure
az sql db show-connection-string --client ado.net --name TutorLiveMentorDb --server your-server
```

**Solution 2:** Restart App Service
```bash
az webapp restart --name TutorLiveMentor --resource-group TutorLiveMentorRG
```

**Solution 3:** Manually run migrations
```bash
# From local machine, connect to Azure SQL
dotnet ef database update --connection "your-azure-connection-string"
```

### Issue: Cannot login with default credentials

**Solution:** Check if admins exist in database
```sql
SELECT AdminId, Email, Department, CreatedDate, LastLogin 
FROM Admins
ORDER BY AdminId;
```

If no records exist, the seeding failed. Check application logs for errors.

### Issue: "Too many requests" error

The application has rate limiting enabled:
- **Login attempts:** 5 per 15 minutes per IP
- **API calls:** 100 per minute per IP

Wait for the rate limit window to expire, or disable rate limiting in `Program.cs` (not recommended for production).

---

## ?? Additional Admin Management

### Create Additional Admin (via Database)

```sql
INSERT INTO Admins (Email, Password, Department, CreatedDate)
VALUES ('new.admin@rgmcet.edu.in', 'SecurePassword@2024', 'CSE(DS)', GETDATE());
```

### List All Admins

```sql
SELECT 
    AdminId,
    Email,
    Department,
    CreatedDate,
    LastLogin
FROM Admins
ORDER BY Department, Email;
```

### Update Admin Password

```sql
UPDATE Admins 
SET Password = 'NewSecurePassword@2024'
WHERE Email = 'admin.cse@rgmcet.edu.in';
```

### Delete Admin (Use with Caution!)

```sql
-- Make sure at least one admin remains!
DELETE FROM Admins 
WHERE Email = 'admin.cse@rgmcet.edu.in';
```

---

## ?? Security Best Practices for Production

1. ? **Protect CSE(DS) primary credentials** - Keep them confidential
2. ? **Change temporary passwords** for CSE and Super Admin accounts
3. ? **Use strong passwords** for new accounts (minimum 12 characters)
4. ? **Enable Azure SQL firewall rules** to restrict database access
5. ? **Use Azure Key Vault** for storing sensitive configuration
6. ? **Enable Application Insights** for monitoring and logging
7. ? **Set up SSL/TLS** with proper certificates
8. ? **Configure CORS** properly for your domain only
9. ? **Enable Azure DDoS Protection**
10. ? **Regular security updates** and patches
11. ? **Implement 2FA** for admin accounts (future enhancement)

---

## ?? Support

For issues or questions:
- Check application logs in Azure Portal
- Review health check endpoints
- Contact system administrator at: cseds@rgmcet.edu.in

---

## ?? Quick Reference Card

**Print this and keep it safe!**

```
???????????????????????????????????????????????????????
?       TUTORLIVE MENTOR - ADMIN CREDENTIALS          ?
???????????????????????????????????????????????????????
?                                                     ?
?  CSE(DS) Admin (PRIMARY):                           ?
?  ?? cseds@rgmcet.edu.in                             ?
?  ?? 9059530688                                      ?
?  ?? Department: CSE(DS)                             ?
?                                                     ?
?  ????????????????????????????????????????????????  ?
?                                                     ?
?  CSE Admin (CHANGE PASSWORD):                       ?
?  ?? admin.cse@rgmcet.edu.in                         ?
?  ?? CSEAdmin@2024                                   ?
?                                                     ?
?  Super Admin (CHANGE PASSWORD):                     ?
?  ?? superadmin@rgmcet.edu.in                        ?
?  ?? SuperAdmin@2024                                 ?
?                                                     ?
?  ??  Keep CSE(DS) credentials secure!               ?
?  ??  Change temporary passwords after first login!  ?
?                                                     ?
???????????????????????????????????????????????????????
```

---

**Last Updated:** 2024
**Version:** 1.0
**Environment:** Azure Cloud Deployment
**Primary Contact:** cseds@rgmcet.edu.in
