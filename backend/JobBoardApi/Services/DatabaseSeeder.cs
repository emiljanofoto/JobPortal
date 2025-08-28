using Microsoft.EntityFrameworkCore;
using JobBoardApi.Data;
using JobBoardApi.Models;

namespace JobBoardApi.Services
{
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public DatabaseSeeder(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task SeedDatabaseAsync()
        {
            try
            {
                // Ensure database is created
                _context.Database.EnsureCreated();
                
                await SeedAdminUserAsync();
                
                Console.WriteLine("✅ Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error during database seeding: {ex.Message}");
                throw;
            }
        }

        private async Task SeedAdminUserAsync()
        {
            var adminEmail = "admin@jobboard.com";
            var adminPassword = "admin123";
            
            var existingAdmin = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == adminEmail);
            
            if (existingAdmin == null)
            {
                var adminUser = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    Role = "admin",
                    CreatedAt = DateTime.UtcNow,
                    PasswordHash = _authService.HashPassword(adminPassword)
                };
                
                _context.Users.Add(adminUser);
                await _context.SaveChangesAsync();
                
                Console.WriteLine($"✅ Created default admin user: {adminEmail}");
                Console.WriteLine($"   Password: {adminPassword}");
            }
            else
            {
                Console.WriteLine($"ℹ️ Admin user already exists: {adminEmail}");
                
                // Optionally reset password to ensure it's always "admin123"
                if (!_authService.VerifyPassword(adminPassword, existingAdmin.PasswordHash))
                {
                    existingAdmin.PasswordHash = _authService.HashPassword(adminPassword);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"🔄 Updated admin password to default");
                }
            }
        }
    }
}