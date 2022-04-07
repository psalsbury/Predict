using Predict.Models;
using Predict.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Predict.Helper
{
    public static class SessionHelper
    {

        // this class is for session variable
        public static object GetPlayerSessionData(HttpSessionStateBase session, string sessionKey, string userId)
        {
            if (session[sessionKey] == null) SetUserSessionVariables(session, userId, false);
            return session[sessionKey];
        }

        public static void RefreshKoPredictions(HttpSessionStateBase session, string userId, short eventId)
        {
            var context = new ApplicationDbContext();
            UpdateKoPredictionsSessionVar(context, session, eventId, userId, true);
            context.Dispose();
        }

        public static void RefreshWinningTeamPredictions(HttpSessionStateBase session, string userId, short eventId)
        {
            var context = new ApplicationDbContext();
            UpdateWinningTeamPredictions(context, session, eventId, userId, true);
            context.Dispose();
        }

        public static void RefreshFixturePredictions(HttpSessionStateBase session, string userId, short eventId)
        {
            var context = new ApplicationDbContext();
            UpdateFixturePredictionsSessionVar(context, session, eventId, userId, true);
            context.Dispose();
        }

        public static void RefreshBonusQuestionPredictions(HttpSessionStateBase session, string userId, short eventId)
        {
            var context = new ApplicationDbContext();
            UpdateBonusPredictionsSessionVar(context, session, eventId, userId, true);
            context.Dispose();
        }

        public static void RefreshPlayerPoolInfo(HttpSessionStateBase session, string userId, short eventId)
        {
            var context = new ApplicationDbContext();
            UpdatePlayerPoolInfo(context, session, eventId, userId, true);
            context.Dispose();
        }


        public static void UpdateSessionForHomePage(ApplicationDbContext context, HttpSessionStateBase session, EventPlayer eventPlayer, bool forceRefresh, bool forcePoolRefresh)
        {
            var eventId = eventPlayer.EventId;
            var userId = eventPlayer.PlayerId;

            var thisEvent = Cache.GetCachedEvent(eventId);

            // Static fixtures --> These should be cached to application, not session!!
            UpdateFixturesSessionVar(context, session, eventId, forceRefresh);
            UpdateKoFixturesSessionVar(context, session, eventId, forceRefresh);
            UpdateBonusSessionVar(context, session, eventId, forceRefresh);

            //Predictions
            UpdateFixturePredictionsSessionVar(context, session, eventId, userId, forceRefresh);
            UpdateKoPredictionsSessionVar(context, session, eventId, userId, forceRefresh);
            UpdateWinningTeamPredictions(context, session, eventId, userId, forceRefresh);
            UpdateBonusPredictionsSessionVar(context, session, eventId, userId, forceRefresh);

            // Pool info
            UpdatePlayerPoolInfo(context, session, eventId, userId, forcePoolRefresh| forceRefresh ? true:false);

        }

        public static List<EventPlayer> GetOrderedEventsForPlayers(HttpSessionStateBase session)
        {
            var eventPlayers = (List<EventPlayer>)session["EventPlayers"];
            var newList = new List<EventPlayer>();

            // Order should by 1, unfinished by start date ASC, then 2. Finished (for 2 weeks) by start date ASC
            var unfinishedEventPlayers = eventPlayers.Where(a => a.Event.EndDateTime >= DateTime.UtcNow).OrderBy(a => a.Event.StartDateTime);
            foreach (var eventPlayer in unfinishedEventPlayers)
            {
                newList.Add(eventPlayer);
            }

            var finishedEventPlayers = eventPlayers.Where(a => a.Event.EndDateTime < DateTime.UtcNow && a.Event.EndDateTime >= DateTime.UtcNow.AddDays(-14)).OrderBy(a => a.Event.StartDateTime);
            foreach (var eventPlayer in finishedEventPlayers)
            {
                newList.Add(eventPlayer);
            }

            if (newList.Count == 0)
            {
                var eventPlayer = eventPlayers.OrderByDescending(a => a.Event.EventStarted).FirstOrDefault();
                newList.Add(eventPlayer);
            }

            return newList;
        }

        public static void SetUserSessionVariables(HttpSessionStateBase session, string userId, bool forceRefresh)
        {
            if (session["Player"] != null && forceRefresh == false)
            {
                return;
            }

            var context = new ApplicationDbContext();
            var forcePoolRefresh = forceRefresh;

            UpdatePlayerSessionVariable(context, session, userId, forceRefresh);
            UpdateEventPlayersSessionVariable(context, session, userId, forceRefresh);
            UpdatePlayerJokeCount(context, session, userId, forceRefresh);

            session.Timeout = 252000; // 180 day

            context.Dispose();
        }

        public static void ClearSessionVariables(HttpSessionStateBase session)
        {
            session.Clear();
        }

        public static void UpdatePlayerQuizQuestionCount(ApplicationDbContext context, HttpSessionStateBase session, string userId, bool forceRefresh)
        {
            const string sessionName = "PlayerQuizQuestions";
            if (session[sessionName] != null && !forceRefresh)
                return;

            // Get all the events that the logged on user is participating in
            var quizQuestionsCounts = context.QuizQuestions
                .Any(e => e.PlayerId == userId);

            session[sessionName] = quizQuestionsCounts;
        }


        public static void UpdatePlayerJokeCount(ApplicationDbContext context,
            HttpSessionStateBase session,
            string userId, bool forceRefresh)
        {
            const string sessionName = "PlayerJokes";
            if (session[sessionName] != null && !forceRefresh)
                return;

            // Get all the events that the logged on user is participating in
            var jokesCounts = context.Jokes
                .Any(e => e.PlayerId == userId);

            session[sessionName] = jokesCounts;
        }
        public static void UpdateEventPlayersSessionVariable(ApplicationDbContext context,
            HttpSessionStateBase session,
            string userId, bool forceRefresh)
        {
            const string sessionName = "EventPlayers";
            if (session[sessionName] != null && !forceRefresh)
                return;

            var twoWeeksAgo = DateTime.UtcNow.AddDays(-14);

            // Get all the events that the logged on user is participating in
            var eventPlayers = context.EventPlayers
                .Include(t => t.Event)
                .Where(e => e.PlayerId == userId && e.Enabled == true && e.Event.EndDateTime >= twoWeeksAgo)
                .OrderBy(a => a.Event.StartDateTime)
                .ToList();

            session[sessionName] = eventPlayers;
        }

        private static void UpdatePlayerPoolInfo(ApplicationDbContext context, HttpSessionStateBase session,
            short eventId, string userId, bool forceRefresh)
        {
            var sessionName = "PoolInfo*" + eventId; 
            if (session[sessionName] != null && !forceRefresh)
                return;

            var poolInfoViewModel = context.Database.SqlQuery<PoolInfoViewModel>(
                "spGetPlayerPoolInfo @intEventId, @strPlayerId"
                , new SqlParameter("intEventId", eventId)
                , new SqlParameter("@strPlayerId", userId)).ToList();

            session[sessionName] = poolInfoViewModel;
        }

        private static void UpdatePlayerSessionVariable(ApplicationDbContext context, HttpSessionStateBase session,
            string userId, bool forceRefresh)
        {
            const string sessionName = "Player";
            if (session[sessionName] != null && !forceRefresh)
                return;

            var player = context.Players.FirstOrDefault(p => p.Id == userId);
            if (player != null) session[sessionName] = player;
        }

        private static void UpdateFixturesSessionVar(ApplicationDbContext context, HttpSessionStateBase session,
            short eventId, bool forceRefresh)
        {
            var sessionName = "nbrFixtures*" + eventId;
            if (session[sessionName] != null && !forceRefresh)
                return;

            var nbrFixtures = context.EventFixtures.Count(e => e.EventId == eventId);
            session[sessionName] = nbrFixtures;
        }

        private static void UpdateKoFixturesSessionVar(ApplicationDbContext context, HttpSessionStateBase session,
            short eventId, bool forceRefresh)
        {
            var sessionName = "nbrKoFixtures*" + eventId;
            if (session[sessionName] != null && !forceRefresh)
                return;

            var nbrKoFixtures = context.KoFixtures.Count(e => e.EventId == eventId);
            session[sessionName] = nbrKoFixtures;
        }

        private static void UpdateBonusSessionVar(ApplicationDbContext context, HttpSessionStateBase session,
            short eventId, bool forceRefresh)
        {
            var sessionName = "nbrBonusQuestions*" + eventId;
            if (session[sessionName] != null && !forceRefresh)
                return;

            var nbrBonusQuestions = context.BonusQuestions.Count(e => e.EventId == eventId);

            session[sessionName] = nbrBonusQuestions;
        }


        private static void UpdateFixturePredictionsSessionVar(ApplicationDbContext context,
            HttpSessionStateBase session,
            short eventId, string userId, bool forceRefresh)
        {
            var sessionName = "nbrFixturePredictions*" + eventId;
            if (session[sessionName] != null && !forceRefresh)
                return;

            var nbrFixturePredictions = (from a in context.FixturePredictions
                                         join c in context.EventFixtures on a.FixtureId equals c.FixtureId
                                         where c.EventId == eventId
                                               && a.EventId == eventId
                                               && a.PlayerId == userId
                                         select a).Count();

            session[sessionName] = nbrFixturePredictions;
        }

        private static void UpdateKoPredictionsSessionVar(ApplicationDbContext context, HttpSessionStateBase session
            , short eventId, string userId, bool forceRefresh)
        {
            var sessionName = "nbrKoPredictions*" + eventId;
            if (session[sessionName] != null && !forceRefresh)
                return;

            var nbrKoPredictionsTeam1 = (from a in context.KoFixturePredictions
                                         join c in context.KoFixtures on a.KoFixtureId equals c.Id
                                         where c.EventId == eventId
                                               && a.PlayerId == userId
                                               && a.Team1Id != null
                                         select a).Count();

            var nbrKoPredictionsTeam2 = (from a in context.KoFixturePredictions
                                         join c in context.KoFixtures on a.KoFixtureId equals c.Id
                                         where c.EventId == eventId
                                               && a.PlayerId == userId
                                               && a.Team2Id != null
                                         select a).Count();

            session[sessionName] = nbrKoPredictionsTeam1 + nbrKoPredictionsTeam2;
        }

        private static void UpdateWinningTeamPredictions(ApplicationDbContext context, HttpSessionStateBase session,
            short eventId, string userId, bool forceRefresh)
        {
            var sessionName = "nbrWinningTeamPredictions*" + eventId;
            if (session[sessionName] != null && !forceRefresh)
                return;

            var nbrWinningTeamPredictions = (from a in context.KoWinningTeamPredictions
                                             where a.EventId == eventId
                                                   && a.PlayerId == userId
                                             select a).Count();

            session[sessionName] = nbrWinningTeamPredictions;
        }

        private static void UpdateBonusPredictionsSessionVar(ApplicationDbContext context, HttpSessionStateBase session,
            short eventId, string userId, bool forceRefresh)
        {
            var sessionName = "nbrBonusQuestionPredictions*" + eventId;
            if (session[sessionName] != null && !forceRefresh)
                return;

            var nbrBonusQuestionPredictions = (from a in context.BonusQuestions
                                               join c in context.BonusQuestionPredictions on a.Id equals c.BonusQuestionId
                                               where a.EventId == eventId && c.PlayerId == userId
                                               select a).Count();

            session[sessionName] = nbrBonusQuestionPredictions;
        }
    }
}