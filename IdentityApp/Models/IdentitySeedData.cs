using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Models
{
    public class IdentitySeedData
    {
        // Constants for the admin user credentials
        private const string adminUser = "admin";
        // Constants for the admin password
        private const string adminPassword = "Admin@123";

        public static async void IdentityTestUser(IApplicationBuilder app)
        {
            // Create a scope to resolve services
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<IdentityContext>();

            // Check migrations and apply them if any are pending
            if (context.Database.GetAppliedMigrations().Any())
            {
                context.Database.Migrate();
            }

            
            var userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            
            var user = await userManager.FindByNameAsync(adminUser);
            
            if(user == null)
            {
                user = new AppUser
                {
                    FullName = "LeBron James",
                    UserName = adminUser,
                    Email = "info@lebronjames.com",
                    PhoneNumber = "1234567890",
                };
                // Create the user if it doesn't exist
                await userManager.CreateAsync(user, adminPassword);
            }
            
        }
    }
}
