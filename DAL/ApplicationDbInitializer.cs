using ImageSharingWithCloud.Models;
using Microsoft.AspNetCore.Identity;

namespace ImageSharingWithCloud.DAL
{
    public  class ApplicationDbInitializer(ApplicationDbContext db, 
        IImageStorage imageStorage,
        ILogger<ApplicationDbInitializer> logger)
    {
        private const string InitAdminUser = "jfk@example.org";

        public async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            if (! await IsEmptyDatabase(serviceProvider))
            {
                return;
            }
            /*
             * Initialize databases.
             */
            logger.LogDebug("Clearing the database...");

            await imageStorage.InitImageStorage();

            /*
             * Clear any existing data from the databases.
             */
            var images = await imageStorage.GetAllImagesInfoAsync();
            foreach (var image in images)
            {
                await imageStorage.RemoveImageAsync(image);
            }

            // db.RemoveRange(db.Users);
            await db.SaveChangesAsync();

            logger.LogDebug("Adding role: User");
            var idResult = await CreateRole(serviceProvider, "User");
            if (!idResult.Succeeded)
            {
                logger.LogDebug("Failed to create User role!");
            }
            
            // TODO add other roles
            await CreateRole(serviceProvider, "Supervisor");
            await CreateRole(serviceProvider, "Administrator");


            logger.LogDebug("Adding user: jfk");
            idResult = await CreateAccount(serviceProvider, InitAdminUser, "jfk123", "Admin");
            if (!idResult.Succeeded)
            {
                logger.LogDebug("Failed to create jfk user!");
            }

            logger.LogDebug("Adding user: nixon");
            idResult = await CreateAccount(serviceProvider, "nixon@example.org", "nixon123", "User");
            if (!idResult.Succeeded)
            {
                logger.LogDebug("Failed to create nixon user!");
            }
            
            
            logger.LogDebug("Adding user: lbj");
            idResult = await CreateAccount(serviceProvider, "lbj@example.org", "lbj123", "User");
            if (!idResult.Succeeded)
            {
                logger.LogDebug("Failed to create lbj user!");
            }

            // TODO add other users and assign more roles
            await CreateAccount(serviceProvider, "admin@example.org", "Admin@123", "Administrator");
            await CreateAccount(serviceProvider, "super@example.org", "Super@123", "Supervisor");
            await CreateAccount(serviceProvider, "user1@example.org",  "User@123",  "User");



            await db.SaveChangesAsync();

        }

        private static async Task<bool> IsEmptyDatabase(IServiceProvider provider)
        {
            UserManager<ApplicationUser> userManager = provider
                .GetRequiredService
                    <UserManager<ApplicationUser>>();
            return await userManager.FindByNameAsync(InitAdminUser) == null;
        }

        private static async Task<IdentityResult> CreateRole(IServiceProvider provider,
                                                            string role)
        {
            RoleManager<IdentityRole> roleManager = provider
                .GetRequiredService
                       <RoleManager<IdentityRole>>();
            var idResult = IdentityResult.Success;
            if (await roleManager.FindByNameAsync(role) == null)
            {
                idResult = await roleManager.CreateAsync(new IdentityRole(role));
            }
            return idResult;
        }

        private static async Task<IdentityResult> CreateAccount(IServiceProvider provider,
                                                               string email, 
                                                               string password,
                                                               string role)
        {
            UserManager<ApplicationUser> userManager = provider
                .GetRequiredService
                       <UserManager<ApplicationUser>>();
            var idResult = IdentityResult.Success;

            if (await userManager.FindByNameAsync(email) == null)
            {
                var user = new ApplicationUser { UserName = email, Email = email };
                idResult = await userManager.CreateAsync(user, password);

                if (idResult.Succeeded)
                {
                    idResult = await userManager.AddToRoleAsync(user, role);
                }
            }

            return idResult;
        }

    }
}