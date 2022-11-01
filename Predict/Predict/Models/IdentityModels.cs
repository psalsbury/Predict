using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

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
            : base(Environment.MachineName == "PETESXPS" ? "DefaultConnection" : "GoDaddyConnection", false)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<EmailLead> EmailLead { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventPlayer> EventPlayers { get; set; }
        public DbSet<EventPool> EventPools { get; set; }
        public DbSet<EventKo> EventKos { get; set; }
        public DbSet<Fixture> Fixtures { get; set; }
        public DbSet<EventFixture> EventFixtures { get; set; }
        public DbSet<EventGeneration> EventGenerations { get; set; }
        public DbSet<FixturePrediction> FixturePredictions { get; set; }
        public DbSet<FixtureOddsByScore> FixtureOddsByScores{ get; set; }
        public DbSet<FixtureOddsByResult> FixtureOddsByResults { get; set; }
        public DbSet<BonusQuestion> BonusQuestions { get; set; }
        public DbSet<BonusQuestionPrediction> BonusQuestionPredictions { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<LeagueSubLeague> LeagueSubLeagues { get; set; }
        public DbSet<LeagueSubLeagueTeam> LeagueSubLeagueTeams { get; set; }
        public DbSet<LeagueEventGeneration> LeagueEventGenerations { get; set; }
        public DbSet<Pool> Pools { get; set; }
        public DbSet<PoolPlayer> PoolPlayers { get; set; }
        public DbSet<EventPoolPlayer> EventPoolPlayers { get; set; }
        public DbSet<EventPoolPlayerPositionHistory> EventPoolPlayerPositionHistory { get; set; }
        public DbSet<KoFixture> KoFixtures { get; set; }
        public DbSet<KoFixturePrediction> KoFixturePredictions { get; set; }
        public DbSet<KoWinningTeamPrediction> KoWinningTeamPredictions { get; set; }
        public DbSet<SiteSetting> SiteSettings { get; set; }
        public DbSet<ForumTopic> ForumTopics { get; set; }
        public DbSet<ForumMessage> ForumMessages { get; set; }
        public DbSet<Joke> Jokes { get; set; }
        public DbSet<JokeRating> JokeRatings { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<QuizQuestionAnswer> QuizQuestionAnswers { get; set; }
        public DbSet<QuizQuestionPlayerAnswer> QuizQuestionPlayerAnswers { get; set; }
        public DbSet<RapidApiV3League> RapidApiV3Leagues { get; set; }
        public DbSet<RapidApiV3LeagueSeason> RapidApiV3LeagueSeasons { get; set; }
        public DbSet<RapidApiV3Country> RapidApiV3Countries { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}