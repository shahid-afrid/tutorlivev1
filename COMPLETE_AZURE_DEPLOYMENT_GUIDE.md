# ?? TutorLiveMentor - Complete Azure Deployment Guide

## ? **DEPLOYMENT READINESS STATUS**

**Build Status:** ? **SUCCESSFUL**  
**Code Quality:** ? **PRODUCTION READY**  
**Admin Seeding:** ? **CONFIGURED**  
**Database Migrations:** ? **READY**  
**Security:** ? **ENABLED**  
**Health Checks:** ? **CONFIGURED**

**?? YOUR APPLICATION IS READY FOR AZURE DEPLOYMENT! ??**

---

## ?? **Pre-Deployment Checklist**

### ? **What's Already Done:**

1. ? **Admin Seeder** - Automatic admin account creation
2. ? **Security Middleware** - HTTPS, HSTS, Security Headers
3. ? **Health Checks** - `/health` and `/health/ready` endpoints
4. ? **Rate Limiting** - Login and API protection
5. ? **Session Management** - Secure, HTTP-only cookies
6. ? **Database Migrations** - Auto-apply on startup
7. ? **SignalR** - Real-time notifications
8. ? **Error Handling** - Production-safe error pages
9. ? **Logging** - Configured for Azure
10. ? **Production Config** - `appsettings.Production.json` created

### ?? **What You Need to Prepare:**

1. ?? Azure Subscription (required)
2. ?? Azure SQL Database connection string
3. ?? Custom domain (optional)
4. ?? SSL certificate (Azure provides free)

---

## ?? **Step-by-Step Azure Deployment**

### **Step 1: Create Azure Resources**

#### **Option A: Using Azure Portal (Recommended for Beginners)**

##### 1.1 Create Resource Group
```
1. Login to Azure Portal: https://portal.azure.com
2. Click "Resource groups" ? "Create"
3. Settings:
   - Subscription: Your subscription
   - Resource group name: TutorLiveMentorRG
   - Region: East US (or closest to you)
4. Click "Review + create" ? "Create"
```

##### 1.2 Create Azure SQL Database
```
1. Click "Create a resource" ? "SQL Database"
2. Basics:
   - Resource group: TutorLiveMentorRG
   - Database name: TutorLiveMentorDB
   - Server: Click "Create new"
     - Server name: tutorlive-sql-server (must be globally unique)
     - Location: Same as resource group
     - Authentication: SQL authentication
     - Server admin login: sqladmin
     - Password: Create a strong password (save it!)
   - Compute + storage: Click "Configure database"
     - Service tier: Basic (for testing) or Standard (for production)
     - Click "Apply"
3. Networking:
   - Connectivity method: Public endpoint
   - Allow Azure services: Yes
   - Add current client IP address: Yes
4. Click "Review + create" ? "Create"
5. Wait for deployment (2-3 minutes)
```

##### 1.3 Get SQL Connection String
```
1. Go to your SQL Database (TutorLiveMentorDB)
2. Click "Connection strings" (left menu)
3. Copy the ADO.NET connection string
4. Replace {your_password} with your actual password
5. Save this connection string - you'll need it!
```

Example connection string:
```
Server=tcp:tutorlive-sql-server.database.windows.net,1433;Initial Catalog=TutorLiveMentorDB;Persist Security Info=False;User ID=sqladmin;Password=YOUR_PASSWORD;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

##### 1.4 Create App Service Plan
```
1. Click "Create a resource" ? "App Service Plan"
2. Basics:
   - Resource group: TutorLiveMentorRG
   - Name: TutorLiveMentorPlan
   - Operating System: Windows
   - Region: Same as resource group
   - Pricing tier: 
     - Free F1 (for testing only)
     - Basic B1 (recommended for small production)
     - Standard S1 (for production with SSL)
3. Click "Review + create" ? "Create"
```

##### 1.5 Create Web App
```
1. Click "Create a resource" ? "Web App"
2. Basics:
   - Resource group: TutorLiveMentorRG
   - Name: tutorlive-app (must be globally unique)
     - URL will be: https://tutorlive-app.azurewebsites.net
   - Publish: Code
   - Runtime stack: .NET 8 (LTS)
   - Operating System: Windows
   - Region: Same as resource group
   - App Service Plan: TutorLiveMentorPlan
3. Monitoring:
   - Enable Application Insights: Yes (recommended)
   - Application Insights: Create new ? TutorLiveMentorInsights
4. Click "Review + create" ? "Create"
5. Wait for deployment (1-2 minutes)
```

#### **Option B: Using Azure CLI**

```bash
# Login to Azure
az login

# Set variables
RESOURCE_GROUP="TutorLiveMentorRG"
LOCATION="eastus"
SQL_SERVER="tutorlive-sql-server"
SQL_DB="TutorLiveMentorDB"
SQL_ADMIN="sqladmin"
SQL_PASSWORD="YourStrongPassword123!"
APP_PLAN="TutorLiveMentorPlan"
WEB_APP="tutorlive-app"

# Create resource group
az group create --name $RESOURCE_GROUP --location $LOCATION

# Create SQL Server
az sql server create \
  --name $SQL_SERVER \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --admin-user $SQL_ADMIN \
  --admin-password $SQL_PASSWORD

# Configure firewall (allow Azure services)
az sql server firewall-rule create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

# Create SQL Database
az sql db create \
  --resource-group $RESOURCE_GROUP \
  --server $SQL_SERVER \
  --name $SQL_DB \
  --service-objective Basic

# Create App Service Plan
az appservice plan create \
  --name $APP_PLAN \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --sku B1

# Create Web App
az webapp create \
  --name $WEB_APP \
  --resource-group $RESOURCE_GROUP \
  --plan $APP_PLAN \
  --runtime "DOTNET|8.0"

# Get connection string
az sql db show-connection-string \
  --client ado.net \
  --server $SQL_SERVER \
  --name $SQL_DB
```

---

### **Step 2: Configure Application Settings**

#### 2.1 Set Connection String

**Via Azure Portal:**
```
1. Go to your Web App (tutorlive-app)
2. Click "Configuration" (left menu)
3. Click "Connection strings" tab
4. Click "+ New connection string"
5. Settings:
   - Name: DefaultConnection
   - Value: [Paste your SQL connection string from Step 1.3]
   - Type: SQLServer
6. Click "OK"
7. Click "Save" at the top
8. Click "Continue" to restart the app
```

**Via Azure CLI:**
```bash
CONNECTION_STRING="Server=tcp:tutorlive-sql-server.database.windows.net,1433;Initial Catalog=TutorLiveMentorDB;Persist Security Info=False;User ID=sqladmin;Password=YOUR_PASSWORD;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

az webapp config connection-string set \
  --name $WEB_APP \
  --resource-group $RESOURCE_GROUP \
  --connection-string-type SQLServer \
  --settings DefaultConnection="$CONNECTION_STRING"
```

#### 2.2 Set Application Settings

**Via Azure Portal:**
```
1. Still in "Configuration"
2. Click "Application settings" tab
3. Add these settings (click "+ New application setting" for each):

   Setting 1:
   - Name: ASPNETCORE_ENVIRONMENT
   - Value: Production

   Setting 2:
   - Name: ServerSettings__ServerMode
   - Value: Cloud

   Setting 3:
   - Name: AllowedHosts
   - Value: * (or your custom domain)

4. Click "Save" at the top
5. Click "Continue" to restart
```

**Via Azure CLI:**
```bash
az webapp config appsettings set \
  --name $WEB_APP \
  --resource-group $RESOURCE_GROUP \
  --settings \
    ASPNETCORE_ENVIRONMENT="Production" \
    ServerSettings__ServerMode="Cloud" \
    AllowedHosts="*"
```

---

### **Step 3: Deploy Your Application**

#### **Option A: Deploy from Visual Studio (Easiest)**

```
1. Open your project in Visual Studio 2022
2. Right-click on the project ? "Publish"
3. Click "Add a publish profile"
4. Choose "Azure" ? "Next"
5. Choose "Azure App Service (Windows)" ? "Next"
6. Sign in with your Azure account
7. Select your subscription
8. Select your Web App (tutorlive-app)
9. Click "Finish"
10. Click "Publish" button
11. Wait for deployment (2-5 minutes)
12. Browser will open automatically when done!
```

#### **Option B: Deploy via Visual Studio Code**

```
1. Install "Azure App Service" extension
2. Press Ctrl+Shift+P
3. Type "Azure App Service: Deploy to Web App"
4. Select your subscription
5. Select your Web App
6. Choose your project folder
7. Confirm deployment
8. Wait for completion
```

#### **Option C: Deploy via Azure CLI**

```bash
# Build and publish locally
dotnet publish -c Release -o ./publish

# Create deployment package
cd publish
zip -r ../deploy.zip .
cd ..

# Deploy to Azure
az webapp deployment source config-zip \
  --name $WEB_APP \
  --resource-group $RESOURCE_GROUP \
  --src deploy.zip
```

#### **Option D: Deploy via GitHub Actions**

Create `.github/workflows/azure-deploy.yml`:

```yaml
name: Deploy to Azure

on:
  push:
    branches: [ main ]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v2
    
    - name: Setup .NET
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
        app-name: 'tutorlive-app'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ${{env.DOTNET_ROOT}}/myapp
```

To get publish profile:
```
1. Go to your Web App in Azure Portal
2. Click "Get publish profile" at the top
3. Save the downloaded file
4. Go to GitHub repository ? Settings ? Secrets
5. Add new secret: AZURE_WEBAPP_PUBLISH_PROFILE
6. Paste the contents of the publish profile file
```

---

### **Step 4: Verify Deployment**

#### 4.1 Check Application Status

**Your app URL:**
```
https://tutorlive-app.azurewebsites.net
```

**Test these URLs:**

1. **Home Page:**
   ```
   https://tutorlive-app.azurewebsites.net
   ```
   Should show: TutorLiveMentor homepage

2. **Health Check (Basic):**
   ```
   https://tutorlive-app.azurewebsites.net/health
   ```
   Should show: `Healthy`

3. **Health Check (Detailed):**
   ```
   https://tutorlive-app.azurewebsites.net/health/ready
   ```
   Should show: JSON with database status

4. **Admin Login:**
   ```
   https://tutorlive-app.azurewebsites.net/Admin/Login
   ```
   Should show: Admin login page

#### 4.2 Test Admin Login

```
1. Go to: https://tutorlive-app.azurewebsites.net/Admin/Login
2. Login with:
   Email: cseds@rgmcet.edu.in
   Password: 9059530688
3. Should redirect to CSE(DS) Dashboard
4. ? Success! Admin accounts were seeded automatically!
```

#### 4.3 Check Logs

**Via Azure Portal:**
```
1. Go to your Web App
2. Click "Log stream" (left menu)
3. Look for:
   [ADMIN SEEDER] Successfully created default admin accounts
4. Should see your 3 admin accounts listed
```

**Via Azure CLI:**
```bash
az webapp log tail \
  --name $WEB_APP \
  --resource-group $RESOURCE_GROUP
```

---

### **Step 5: Configure Custom Domain (Optional)**

#### 5.1 Add Custom Domain

```
1. Go to your Web App
2. Click "Custom domains" (left menu)
3. Click "+ Add custom domain"
4. Enter your domain (e.g., tutorlive.yourdomain.com)
5. Follow DNS verification steps
6. Click "Validate"
7. Click "Add custom domain"
```

#### 5.2 Add SSL Certificate

**Option A: Free Azure Managed Certificate**
```
1. Still in "Custom domains"
2. Click on your custom domain
3. Click "Add binding"
4. Choose "Azure managed certificate" (Free!)
5. Click "Add"
6. Wait 5-10 minutes for certificate provisioning
```

**Option B: Upload Your Certificate**
```
1. Click "TLS/SSL settings" (left menu)
2. Click "Private Key Certificates (.pfx)"
3. Click "+ Upload certificate"
4. Upload your .pfx file
5. Enter password
6. Go back to "Custom domains"
7. Add SSL binding using your certificate
```

---

## ?? **Post-Deployment Security**

### 1. Change Default Admin Passwords

After first login, change these passwords:

```sql
-- Connect to Azure SQL Database
-- You can use Azure Data Studio or SSMS

-- Change CSE Admin password
UPDATE Admins 
SET Password = 'YourNewSecurePassword'
WHERE Email = 'admin.cse@rgmcet.edu.in';

-- Change Super Admin password
UPDATE Admins 
SET Password = 'YourNewSecurePassword'
WHERE Email = 'superadmin@rgmcet.edu.in';
```

### 2. Configure Firewall Rules

**Restrict database access:**
```
1. Go to your SQL Server (not database)
2. Click "Firewalls and virtual networks"
3. Remove "Allow Azure services" if not needed
4. Add specific IP addresses:
   - Your office IP
   - Your home IP
   - Any other trusted IPs
4. Click "Save"
```

### 3. Enable Application Insights

**If not enabled during creation:**
```
1. Go to your Web App
2. Click "Application Insights" (left menu)
3. Click "Turn on Application Insights"
4. Click "Create new resource"
5. Click "Apply"
```

### 4. Set up Backup

```
1. Go to your Web App
2. Click "Backups" (left menu)
3. Configure storage account
4. Set backup schedule (e.g., daily)
5. Click "Configure"
6. Click "Backup Now" for first backup
```

---

## ?? **Monitoring Your Application**

### Application Insights Queries

**View Application Map:**
```
1. Go to Application Insights resource
2. Click "Application Map"
3. See all components and dependencies
```

**View Live Metrics:**
```
1. Click "Live Metrics"
2. See real-time requests, failures, performance
```

**Query Logs:**
```
1. Click "Logs"
2. Run queries like:

// Failed requests
requests
| where success == false
| order by timestamp desc

// Slow requests
requests
| where duration > 1000
| order by duration desc

// Admin logins
traces
| where message contains "Admin Login"
| order by timestamp desc
```

---

## ?? **Troubleshooting**

### Issue: "Database connection failed"

**Solution:**
```
1. Check connection string in Configuration
2. Verify SQL Server firewall allows Azure services
3. Test connection from Azure Portal:
   - Go to SQL Database
   - Click "Query editor"
   - Login with SQL credentials
   - If works, connection string is correct
```

### Issue: "Admin accounts not created"

**Solution:**
```
1. Check logs in Log Stream
2. Look for [ADMIN SEEDER] messages
3. If not found, restart the app:
   - Go to Web App
   - Click "Restart" at top
   - Wait 2 minutes
   - Check logs again
```

### Issue: "Application returns 500 error"

**Solution:**
```
1. Enable detailed errors:
   - Go to Configuration
   - Add setting: ASPNETCORE_DETAILEDERRORS = true
   - Save and restart
2. Check Application Insights for exception details
3. Check Log Stream for error messages
```

### Issue: "SSL/HTTPS not working"

**Solution:**
```
1. Verify HTTPS redirect is enabled in code (it is)
2. Check if custom domain has SSL binding
3. Wait 5-10 minutes after adding SSL (provisioning time)
4. Clear browser cache and try again
```

---

## ?? **Cost Estimation**

### Free Tier (Testing Only):
```
- App Service: Free F1 tier
- SQL Database: None (use local DB)
- Total: $0/month
- Limitations: Limited resources, no SSL, 60 min/day
```

### Basic Tier (Small Production):
```
- App Service: Basic B1 = $13/month
- SQL Database: Basic (2GB) = $5/month
- Application Insights: Free tier (5GB/month)
- Total: ~$18/month
```

### Standard Tier (Production):
```
- App Service: Standard S1 = $69/month
- SQL Database: Standard S0 (10GB) = $15/month
- Application Insights: Included
- Total: ~$84/month
```

### Premium (High Traffic):
```
- App Service: Premium P1V2 = $100/month
- SQL Database: Standard S3 (250GB) = $150/month
- Total: ~$250/month
```

---

## ?? **Deployment Checklist**

### ? **Pre-Deployment**
- [x] Code compiles successfully
- [x] Admin seeder configured
- [x] Database migrations ready
- [x] Security features enabled
- [x] Health checks working
- [x] appsettings.Production.json created

### ? **Azure Setup**
- [ ] Azure subscription active
- [ ] Resource group created
- [ ] SQL Database created
- [ ] Connection string obtained
- [ ] App Service created
- [ ] Connection string configured
- [ ] Application settings configured

### ? **Deployment**
- [ ] Application published
- [ ] Deployment successful
- [ ] Home page loads
- [ ] Health checks pass
- [ ] Admin login works
- [ ] Database seeded

### ? **Post-Deployment**
- [ ] Default passwords changed
- [ ] Firewall configured
- [ ] Application Insights enabled
- [ ] Backups configured
- [ ] Monitoring set up
- [ ] Custom domain added (optional)
- [ ] SSL certificate added

---

## ?? **Success Criteria**

Your deployment is successful when:

1. ? Home page loads at https://your-app.azurewebsites.net
2. ? `/health` returns "Healthy"
3. ? `/health/ready` shows database connected
4. ? Admin can login with: cseds@rgmcet.edu.in / 9059530688
5. ? Dashboard displays correctly
6. ? All features work (faculty, subjects, students)
7. ? No console errors
8. ? Logs show successful admin seeding

---

## ?? **Support Resources**

### Azure Documentation:
- https://docs.microsoft.com/azure/app-service/
- https://docs.microsoft.com/azure/sql-database/

### Your Application URLs:
```
Production: https://tutorlive-app.azurewebsites.net
Admin Login: https://tutorlive-app.azurewebsites.net/Admin/Login
Health Check: https://tutorlive-app.azurewebsites.net/health
```

### Your Admin Credentials:
```
Primary: cseds@rgmcet.edu.in / 9059530688
CSE: admin.cse@rgmcet.edu.in / CSEAdmin@2024
Super: superadmin@rgmcet.edu.in / SuperAdmin@2024
```

---

## ?? **Quick Deploy Script**

Save this as `deploy-to-azure.ps1`:

```powershell
# Azure Deployment Script
Write-Host "?? TutorLiveMentor Azure Deployment" -ForegroundColor Cyan

# Build
Write-Host "?? Building application..." -ForegroundColor Yellow
dotnet build --configuration Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "? Build failed!" -ForegroundColor Red
    exit 1
}

# Publish
Write-Host "?? Publishing application..." -ForegroundColor Yellow
dotnet publish -c Release -o ./publish

# Create zip
Write-Host "?? Creating deployment package..." -ForegroundColor Yellow
Compress-Archive -Path ./publish/* -DestinationPath ./deploy.zip -Force

# Deploy
Write-Host "?? Deploying to Azure..." -ForegroundColor Yellow
az webapp deployment source config-zip `
  --name "tutorlive-app" `
  --resource-group "TutorLiveMentorRG" `
  --src "./deploy.zip"

Write-Host "? Deployment complete!" -ForegroundColor Green
Write-Host "?? URL: https://tutorlive-app.azurewebsites.net" -ForegroundColor Cyan
```

Run with: `.\deploy-to-azure.ps1`

---

**?? YOU'RE READY TO DEPLOY TO AZURE! ??**

**Status:** ? **100% READY**  
**Estimated Deployment Time:** 15-30 minutes  
**Difficulty:** ??? (Moderate)  

**Good luck with your deployment! ??**
