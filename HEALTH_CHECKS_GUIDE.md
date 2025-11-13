# ?? Health Check Endpoints - Setup Guide

## What We Added

Your TutorLiveMentor application now has **health check endpoints** that Azure (and other platforms) can use to monitor your application's health automatically.

---

## ?? Available Endpoints

### 1. **Basic Health Check**
**URL:** `https://yourdomain.com/health`

**Purpose:** Quick check if the app is running

**Response:**
```
? HTTP 200 OK = "App is healthy"
? HTTP 503 Service Unavailable = "App has problems"
```

**Example:**
```bash
curl https://localhost:5001/health
# Returns: Healthy
```

---

### 2. **Detailed Health Check**
**URL:** `https://yourdomain.com/health/ready`

**Purpose:** Shows status of each component (database, app, etc.)

**Response (JSON):**
```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "database",
      "status": "Healthy",
      "description": "Successfully connected to SQL Server",
      "duration": 45.2
    },
    {
      "name": "self",
      "status": "Healthy",
      "description": "Application is running",
      "duration": 0.1
    }
  ],
  "totalDuration": 45.3
}
```

---

### 3. **Visual Health Status Page**
**URL:** `https://yourdomain.com/Home/HealthStatus`

**Purpose:** User-friendly page showing real-time health status

**Features:**
- ? Real-time health monitoring
- ? Auto-refresh every 30 seconds
- ? Visual indicators (green = healthy, red = unhealthy)
- ? Component-level details
- ? Response time metrics

---

## ?? Testing Locally

### Test the endpoints:

1. **Start your application:**
   ```bash
   dotnet run
   ```

2. **Test basic health check:**
   - Browser: `https://localhost:5001/health`
   - Or command line: `curl https://localhost:5001/health`
   - Expected: "Healthy"

3. **Test detailed health check:**
   - Browser: `https://localhost:5001/health/ready`
   - Expected: JSON with component statuses

4. **View visual health page:**
   - Browser: `https://localhost:5001/Home/HealthStatus`
   - Expected: Nice dashboard showing all health checks

---

## ?? Configuring in Azure App Service

Once you deploy to Azure:

### Step 1: Enable Health Check
1. Go to **Azure Portal** ? Your App Service
2. Click **Health check** in the left menu
3. Toggle **Enable** to ON

### Step 2: Configure Settings
- **Path:** `/health`
- **Interval:** `2` minutes
- **Unhealthy threshold:** `3` consecutive failures
- **Load balancing:** Enable "Remove unhealthy instances"

### Step 3: Test
- Click **Test health check**
- Should show: ? "Health check passed"

### Step 4: Set Up Alerts (Optional)
1. Go to **Monitoring** ? **Alerts**
2. Create new alert rule
3. Condition: Health check status = Unhealthy
4. Action: Send email/SMS to you

---

## ?? What Each Check Does

### **Database Check**
- **Tests:** Can the app connect to SQL Server?
- **How:** Attempts to open a database connection
- **Fails when:** Database is offline, credentials wrong, firewall blocking

### **Self Check**
- **Tests:** Is the web server responding?
- **How:** Simple ping to the application
- **Fails when:** Application crashed or hung

---

## ?? What Happens When Health Check Fails?

### **In Azure:**
1. **First failure:** Azure notes it (no action)
2. **3rd consecutive failure:** 
   - Azure marks instance as "unhealthy"
   - Removes it from load balancer (no traffic sent)
   - Starts auto-healing process
3. **Auto-healing actions:**
   - Restarts the app
   - Recycles the instance
   - Sends alert to you

### **In Your App:**
- Users are automatically routed to healthy instances
- You get notified of the problem
- Azure attempts automatic recovery

---

## ?? Monitoring Tips

### **View Health Check History:**
```
Azure Portal ? App Service ? Diagnose and solve problems ? Availability and Performance
```

### **Check Logs:**
```
Azure Portal ? App Service ? Log stream
```
Look for:
```
? Health Checks: ENABLED at /health and /health/ready
```

### **Application Insights Integration:**
If you enable Application Insights, health check failures will show in:
- Failures dashboard
- Alert rules
- Availability tests

---

## ??? Troubleshooting

### "Health check returning 503"

**Possible causes:**
1. **Database connection failed**
   - Check connection string in Azure Configuration
   - Verify firewall rules allow Azure services
   - Test connection in SQL Management Studio

2. **Application crashed**
   - Check Application logs
   - Review recent deployments
   - Check for unhandled exceptions

### "Can't access /health endpoint"

1. Verify app is running: `https://yourdomain.com`
2. Check deployment succeeded
3. Review build logs for errors

---

## ?? Best Practices

### Development:
- ? Visit `/Home/HealthStatus` regularly to monitor local health
- ? Test health checks before deploying
- ? Ensure database migrations ran successfully

### Production (Azure):
- ? Set health check interval to 2-5 minutes
- ? Configure alerts for 3+ consecutive failures
- ? Enable Application Insights for detailed metrics
- ? Review health check logs weekly

### During Deployment:
- ? Health checks validate app started correctly
- ? Azure won't route traffic until `/health` returns 200 OK
- ? Failed deployments are caught early

---

## ?? Quick Reference

| Endpoint | Purpose | Azure Uses It? |
|----------|---------|----------------|
| `/health` | Basic health check | ? YES |
| `/health/ready` | Detailed component status | ? No (for monitoring tools) |
| `/Home/HealthStatus` | Visual dashboard | ? No (for admins) |

---

## ?? Why This Matters for Your Faculty Selection App

### Scenario: Enrollment Rush (500 students selecting at once)

**WITHOUT health checks:**
- Database gets overloaded
- App crashes
- Students see errors
- You manually restart server
- 15+ minutes downtime

**WITH health checks:**
- Database gets overloaded
- Health check detects `/health` returning 503
- Azure auto-restarts app (30 seconds)
- Students temporarily see "loading"
- 2-3 minutes total disruption
- You get alert to investigate

### Benefits:
1. ? **Auto-recovery** - Azure restarts app automatically
2. ? **Early detection** - Know about problems before users complain
3. ? **Deployment safety** - Bad deploys caught immediately
4. ? **Professional monitoring** - Real-time health visibility

---

## ?? Additional Resources

- [Azure Health Check Documentation](https://learn.microsoft.com/en-us/azure/app-service/monitor-instances-health-check)
- [ASP.NET Core Health Checks](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)

---

**Your app is now production-ready with health monitoring!** ??

Visit `https://localhost:5001/Home/HealthStatus` to see it in action.
