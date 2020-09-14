using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Predict.Models;
using Predict.ViewModels;

namespace Predict.Helper
{
    public static class SessionHelper
    {        

        // this class is for session variable
        public static object GetPlayerSessionData(HttpSessionStateBase session, string sessionKey, string userId)
        {
            if (session[sessionKey] == null) SetUserSessionVariables(session, userId,false);
            return session[sessionKey];
        }

        public static void RefreshKoPredictions(HttpSessionStateBase session, string userId, short eventId)
        {
            var context = new ApplicationDbContext();
            UpdateKoPredictionsSessionVar(context, session, eventId, userId, true);
        }

        public static void RefreshWinningTeamPredictions(HttpSessionStateBase session, string userId, short eventId)
        {
            var context = new ApplicationDbContext();
            UpdateWinningTeamPredictions(context, session, eventId, userId, true);
        }

        public static void RefreshFixturePredictions(HttpSessionStateBase session, string userId, short eventId)
        {
            var context = new ApplicationDbContext();
            UpdateFixturePredictionsSessionVar(context, session, eventId, userId, true);
        }

        public static void RefreshBonusQuestionPredictions(HttpSessionStateBase session, string userId, short eventId)
        {
            var context = new ApplicationDbContext();
            UpdateBonusPredictionsSessionVar(context, session, eventId, userId, true);
        }

        public static void SetUserSessionVariables(HttpSessionStateBase session, string userId, bool forceRefresh)
        {
            if(session["Player"] != null && forceRefresh==false)
            {
                return;
            }

            var context = new ApplicationDbContext();
            var forcePoolRefresh = forceRefresh;
            UpdatePlayerSessionVariable(context, session, userId, forceRefresh);
            UpdateEventPlayersSessionVariable(context, session, userId, forceRefresh);
            var eventPlayers = ((List<EventPlayer>) session["Events"]).FindAll(v => v.Event.EndDateTime >= DateTime.Today.AddDays(-14));

            foreach (var eventPlayer in eventPlayers)
            {
                var eventId = eventPlayer.EventId;

                var thisEvent = Cache.GetCachedEvent(eventPlayer.Event.Id);

                if(eventPlayer.Event.ModifiedDateTime < thisEvent.ModifiedDateTime)
                {
                    forcePoolRefresh = true;
                }

                // Static fixtures --> These should be cached to application, not session!!
                UpdateFixturesSessionVar(context, session, eventId, forceRefresh);
                UpdateKoFixturesSessionVar(context, session, eventId, forceRefresh);
                UpdateBonusSessionVar(context, session, eventId, forceRefresh);

                //Predictions
                UpdateFixturePredictionsSessionVar(context, session, eventId, userId, forceRefresh);
                UpdateKoPredictionsSessionVar(context, session, eventId, userId, forceRefresh);
                UpdateWinningTeamPredictions(context, session, eventId, userId, forceRefresh);
                UpdateBonusPredictionsSessionVar(context, session, eventId, userId, forceRefresh);
            }

            // Pool info
            UpdatePlayerPoolInfo(context, session, userId, forcePoolRefresh);
            if (forcePoolRefresh)
            {
                UpdateEventPlayersSessionVariable(context, session, userId, forceRefresh);
            }

            session.Timeout = 252000; // 180 day
        }

        public static void ClearSessionVariables(HttpSessionStateBase session)
        {
            session.Clear();
        }

        public static void UpdateEventPlayersSessionVariable(ApplicationDbContext context,
            HttpSessionStateBase session,
            string userId, bool forceRefresh)
        {
            const string sessionName = "Events";
            if (session[sessionName] != null && !forceRefresh)
                return;

            var eventPlayers = context.EventPlayers
                .Include(t => t.Event)
                .Where(e => e.PlayerId == userId && e.Enabled==true)   
                .OrderBy(a => a.Event.StartDateTime)
                .ToList();

            session[sessionName] = eventPlayers;
        }

        private static void UpdatePlayerPoolInfo(ApplicationDbContext context, HttpSessionStateBase session,
            string userId, bool forceRefresh)
        {
            const string sessionName = "PoolInfo";
            if (session[sessionName] != null && !forceRefresh)
                return;

            var poolInfoViewModel = context.Database.SqlQuery<PoolInfoViewModel>(
                "spGetPlayerPoolInfo @strPlayerId"
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