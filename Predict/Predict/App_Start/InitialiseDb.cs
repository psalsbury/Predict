using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Predict.Models;

namespace Predict.App_Start
{
    public class InitialiseDb
    {
        public static void CreateRolesAndUsers()
        {
            var context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // In Startup iam creating first Admin Role and creating a default Admin User 
            if (!roleManager.RoleExists("Admin"))
            {
                // first we create Admin role
                var role = new IdentityRole
                {
                    Name = "Admin"
                };
                roleManager.Create(role);
            }

            // creating Creating Player role 
            if (!roleManager.RoleExists("Player"))
            {
                var role = new IdentityRole
                {
                    Name = "Player"
                };
                roleManager.Create(role);
            }
        }
    }
}