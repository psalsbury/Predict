using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Predict.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base(Environment.MachineName == "THINKPAD" ? "DefaultConnection" : "GoDaddyConnection", false)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventPlayer> EventPlayers { get; set; }

        public DbSet<EventKo> EventKos { get; set; }
        public DbSet<Fixture> Fixtures { get; set; }
        public DbSet<EventFixture> EventFixtures { get; set; }
        public DbSet<FixturePrediction> FixturePredictions { get; set; }
        public DbSet<BonusQuestion> BonusQuestions { get; set; }
        public DbSet<BonusQuestionPrediction> BonusQuestionPredictions { get; set; }

        public DbSet<League> Leagues { get; set; }

        public DbSet<Pool> Pools { get; set; }
        public DbSet<PoolPlayer> PoolPlayers { get; set; }
        public DbSet<PoolPlayerPositionHistory> PoolPlayerPositionHistory { get; set; }

        public DbSet<KoFixture> KoFixtures { get; set; }
        public DbSet<KoFixturePrediction> KoFixturePredictions { get; set; }
        public DbSet<KoWinningTeamPrediction> KoWinningTeamPredictions { get; set; }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Reply> Replies { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}