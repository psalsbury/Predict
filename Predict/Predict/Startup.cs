using System;
using System.Data.Entity.Migrations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using Predict.Models;

[assembly: OwinStartupAttribute(typeof(Predict.Startup))]
namespace Predict
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesAndUsers();
        }

        private void CreateRolesAndUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // In Startup iam creating first Admin Role and creating a default Admin User 
            if (!roleManager.RoleExists("Admin"))
            {

                // first we create Admin role
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole
                {
                    Name = "Admin"
                };
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website				
                var user = new ApplicationUser
                {
                    UserName = "Pete",
                    Email = "pete@salsbury.co.uk"
                };

                string userPWD = "Kcalb267wc!";

                var chkUser = userManager.Create(user, userPWD);

                var player = new Player
                {
                    Id = user.Id, 
                    DisplayName = user.UserName,
                    PlayerName = "Pete Salsbury",
                    CreatedDateTime = DateTime.Now,
                    ModifiedDateTime = DateTime.Now,
                };
                context.Players.Add(player);

                var myEvent = new Event()
                {
                    EventName = "Think Social Comp 1",
                    CreatedDateTime = DateTime.Now,
                    ModifiedDateTime = DateTime.Now,
                    EventStartDateTime = System.DateTime.Parse("27 jun 2020 11:30")
                };
                context.Events.Add(myEvent);

                var pool = new Pool()
                {
                    PoolName = "Think Money Comp 1",
                    EventId = myEvent.Id,
                    AdminPlayerId = user.Id,
                    CorrectScorePoints = 3,
                    CorrectResultPoints = 1,
                    EmailNotifications = true,
                    KoLast16Points = 1,
                    KoLast8Points = 2,
                    KoLast4Points = 4,
                    KoLast2Points = 6,
                    KoLast1Points = 10,
                    CreatedDateTime = DateTime.Now,
                    ModifiedDateTime = DateTime.Now
                };
                context.Pools.Add(pool);

                myEvent.DefaultPoolId = pool.Id;
                context.Events.AddOrUpdate(myEvent);

                // Add the PoolPlayer entry for newly created admin to the global pool
                var globalPoolPlayer = new PoolPlayer()
                {
                    PoolId = System.Convert.ToInt32(pool.Id),
                    PlayerId = user.Id,
                    CreatedDateTime = DateTime.Now,
                    ModifiedDateTime = DateTime.Now
                };
                context.PoolPlayers.Add(globalPoolPlayer);

                //Add newly created user to the Admin
                if (chkUser.Succeeded)
                {
                    var result1 = userManager.AddToRole(user.Id, "Admin");

                }
            }

            // creating Creating Player role 
            if (!roleManager.RoleExists("Player"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole
                {
                    Name = "Player"
                };
                roleManager.Create(role);

            }
        }
    }
}



 
