using Microsoft.EntityFrameworkCore;
using TutorLiveMentor.Models;

namespace TutorLiveMentor.Services
{
    /// <summary>
    /// Service to seed default admin accounts for deployment
    /// </summary>
    public class AdminSeeder
    {
        /// <summary>
        /// Seeds default admin accounts if they don't exist
        /// </summary>
        public static async Task SeedDefaultAdmins(AppDbContext context, ILogger logger)
        {
            try
            {
                logger.LogInformation("========================================");
                logger.LogInformation("[ADMIN SEEDER] Checking for default admin accounts...");

                // Check if any admins exist
                var adminCount = await context.Admins.CountAsync();
                
                if (adminCount > 0)
                {
                    logger.LogInformation($"[ADMIN SEEDER] Found {adminCount} existing admin(s). Skipping seed.");
                    logger.LogInformation("========================================");
                    return;
                }

                logger.LogInformation("[ADMIN SEEDER] No admins found. Creating default accounts...");

                // Create default admin accounts
                var defaultAdmins = new List<Admin>
                {
                    // CSE(DS) Department Admin - PRIMARY ACCOUNT
                    new Admin
                    {
                        Email = "cseds@rgmcet.edu.in",
                        Password = "9059530688",
                        Department = "CSE(DS)",
                        CreatedDate = DateTime.Now
                    },
                    
                    // CSE Department Admin (for future use)
                    new Admin
                    {
                        Email = "admin.cse@rgmcet.edu.in",
                        Password = "CSEAdmin@2024",  // Change this in production!
                        Department = "CSE",
                        CreatedDate = DateTime.Now
                    },
                    
                    // Super Admin (for all departments)
                    new Admin
                    {
                        Email = "superadmin@rgmcet.edu.in",
                        Password = "SuperAdmin@2024",  // Change this in production!
                        Department = "CSE(DS)",  // Can access all departments
                        CreatedDate = DateTime.Now
                    }
                };

                await context.Admins.AddRangeAsync(defaultAdmins);
                await context.SaveChangesAsync();

                logger.LogInformation("[ADMIN SEEDER] ? Successfully created default admin accounts:");
                logger.LogInformation("========================================");
                logger.LogInformation("CSE(DS) Admin (PRIMARY):");
                logger.LogInformation("  Email: cseds@rgmcet.edu.in");
                logger.LogInformation("  Password: 9059530688");
                logger.LogInformation("----------------------------------------");
                logger.LogInformation("CSE Admin:");
                logger.LogInformation("  Email: admin.cse@rgmcet.edu.in");
                logger.LogInformation("  Password: CSEAdmin@2024");
                logger.LogInformation("----------------------------------------");
                logger.LogInformation("Super Admin:");
                logger.LogInformation("  Email: superadmin@rgmcet.edu.in");
                logger.LogInformation("  Password: SuperAdmin@2024");
                logger.LogInformation("========================================");
                logger.LogWarning("[SECURITY] CSE(DS) admin uses production credentials.");
                logger.LogWarning("[SECURITY] Please change CSE and Super Admin passwords after first login!");
                logger.LogInformation("========================================");
            }
            catch (Exception ex)
            {
                logger.LogError($"[ADMIN SEEDER] ? Error seeding admin accounts: {ex.Message}");
                logger.LogError($"[ADMIN SEEDER] Stack trace: {ex.StackTrace}");
                // Don't throw - allow app to start even if seeding fails
            }
        }

        /// <summary>
        /// Creates a custom admin account (for manual setup)
        /// </summary>
        public static async Task<bool> CreateCustomAdmin(
            AppDbContext context, 
            ILogger logger,
            string email, 
            string password, 
            string department)
        {
            try
            {
                // Check if admin already exists
                var existingAdmin = await context.Admins
                    .FirstOrDefaultAsync(a => a.Email == email);

                if (existingAdmin != null)
                {
                    logger.LogWarning($"[ADMIN SEEDER] Admin with email {email} already exists.");
                    return false;
                }

                var newAdmin = new Admin
                {
                    Email = email,
                    Password = password,
                    Department = department,
                    CreatedDate = DateTime.Now
                };

                await context.Admins.AddAsync(newAdmin);
                await context.SaveChangesAsync();

                logger.LogInformation($"[ADMIN SEEDER] ? Successfully created admin account: {email}");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError($"[ADMIN SEEDER] ? Error creating admin account: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Updates an admin password
        /// </summary>
        public static async Task<bool> UpdateAdminPassword(
            AppDbContext context,
            ILogger logger,
            string email,
            string newPassword)
        {
            try
            {
                var admin = await context.Admins
                    .FirstOrDefaultAsync(a => a.Email == email);

                if (admin == null)
                {
                    logger.LogWarning($"[ADMIN SEEDER] Admin with email {email} not found.");
                    return false;
                }

                admin.Password = newPassword;
                await context.SaveChangesAsync();

                logger.LogInformation($"[ADMIN SEEDER] ? Successfully updated password for: {email}");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError($"[ADMIN SEEDER] ? Error updating password: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Lists all admin accounts (for debugging)
        /// </summary>
        public static async Task ListAllAdmins(AppDbContext context, ILogger logger)
        {
            try
            {
                var admins = await context.Admins
                    .OrderBy(a => a.Department)
                    .ThenBy(a => a.Email)
                    .ToListAsync();

                logger.LogInformation("========================================");
                logger.LogInformation($"[ADMIN SEEDER] Total Admin Accounts: {admins.Count}");
                logger.LogInformation("========================================");

                foreach (var admin in admins)
                {
                    logger.LogInformation($"ID: {admin.AdminId}");
                    logger.LogInformation($"  Email: {admin.Email}");
                    logger.LogInformation($"  Department: {admin.Department}");
                    logger.LogInformation($"  Created: {admin.CreatedDate:yyyy-MM-dd HH:mm:ss}");
                    logger.LogInformation($"  Last Login: {(admin.LastLogin.HasValue ? admin.LastLogin.Value.ToString("yyyy-MM-dd HH:mm:ss") : "Never")}");
                    logger.LogInformation("----------------------------------------");
                }

                logger.LogInformation("========================================");
            }
            catch (Exception ex)
            {
                logger.LogError($"[ADMIN SEEDER] ? Error listing admins: {ex.Message}");
            }
        }
    }
}
