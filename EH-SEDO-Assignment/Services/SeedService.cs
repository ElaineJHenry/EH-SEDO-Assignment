using EH_SEDO_Assignment.Data;
using Microsoft.AspNetCore.Identity;

namespace EH_SEDO_Assignment.Services
{
    public class SeedService
    {
        public static async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            //initialise services
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<EH_SEDO_AssignmentContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedService>>();

            try
            {
                //ensure database exists
                logger.LogInformation("Ensuring database is created.");
                await context.Database.EnsureCreatedAsync();

                //add roles
                logger.LogInformation("Seeding roles.");
                await AddRoleAsync(roleManager, "Admin");
                await AddRoleAsync(roleManager, "User");

                //add admin user
                logger.LogInformation("Seeding Admin user");
                var adminEmail = "elainehenry1712@gmail.com";
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var adminUser = new ApplicationUser
                    {
                        FirstName = "Elaine",
                        LastName = "Henry",
                        UserName = adminEmail,
                        NormalizedUserName = adminEmail.ToUpperInvariant(),
                        Email = adminEmail,
                        NormalizedEmail = adminEmail.ToUpperInvariant(),
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString()

                    };

                    var result = await userManager.CreateAsync(adminUser, "Admin.Cr3d");
                    if (result.Succeeded)
                    {
                        logger.LogInformation("Assigning Admin role to Admin user");
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                    else
                    {
                        logger.LogError($"Failed to create Admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "An error occured while seeding the database");
            }
        }

        private static async Task AddRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if(!await roleManager.RoleExistsAsync(roleName)) 
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
