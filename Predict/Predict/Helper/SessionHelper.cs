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
            if(session[sessionKey] == null)
            {
                SetUserSessionVariables(session,userId);
            }
            return session[sessionKey];
        }

        public static void RefreshKoPredictions(HttpSessionStateBase session, string userId)
        {
            var context = new ApplicationDbContext();
            var eventId = Predict.Helper.Cache.GetEventId();
            UpdateKoSessionVar(context, session, eventId, userId, true);
        }

        public static void RefreshWinningTeamPredictions(HttpSessionStateBase session, string userId)
        {
            var context = new ApplicationDbContext();
            var eventId = Predict.Helper.Cache.GetEventId();
            UpdateWinningTeamPredictions(context, session, eventId, userId, true);
        }

        public static void RefreshFixturePredictions(HttpSessionStateBase session, string userId)
        {
            var context = new ApplicationDbContext();
            var eventId = Predict.Helper.Cache.GetEventId();
            UpdateFixturesSessionVar(context, session, eventId, userId, true);
        }

        public static void SetUserSessionVariables(HttpSessionStateBase session, string userId)
        {
            var context = new ApplicationDbContext();
            var eventId = Predict.Helper.Cache.GetEventId();

            UpdatePlayerSessionVariable(context, session, userId, false);
            UpdateFixturesSessionVar(context, session, eventId, userId, false);
            UpdateKoSessionVar(context, session, eventId, userId, false);
            UpdateWinningTeamPredictions(context, session, eventId, userId, false);
            UpdateBonusSessionVar(context, session, eventId, userId, false);
            UpdatePlayerPoolInfo(context, session, eventId, userId, false);
            session.Timeout = 252000; // 180 day

        }

        public static void ClearSessionVariables(HttpSessionStateBase session)
        {
            session.Clear();
        }

        private static void UpdatePlayerPoolInfo(ApplicationDbContext context, HttpSessionStateBase session,
            short eventId, string userId, bool forceRefresh)
        {
            const string sessionName = "PoolInfo";
            if (session[sessionName] != null && !forceRefresh)
                return;

            var poolInfoViewModel = context.Database.SqlQuery<PoolInfoViewModel>(
                "spGetPlayerPoolInfo @intEventId, @strPlayerId"
                , new SqlParameter("@intEventId", eventId)
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
            if (player != null)
            {
                session[sessionName] = player;
            }
        }

        private static void UpdateFixturesSessionVar(ApplicationDbContext context, HttpSessionStateBase session,
            short eventId, string userId, bool forceRefresh)
        {
            const string sessionName = "nbrFixturePredictions";
            if (session[sessionName] != null && !forceRefresh)
                return;

            var nbrFixturePredictions = (from a in context.FixturePredictions
                join c in context.Fixtures on a.FixtureId equals c.Id
                where c.EventId == eventId
                      && a.PlayerId == userId
                select a).Count();

            session[sessionName] = nbrFixturePredictions;
        }

        private static void UpdateKoSessionVar(ApplicationDbContext context, HttpSessionStateBase session
            , short eventId, string userId, bool forceRefresh)
        {
            const string sessionName = "nbrKoPredictions";
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

            session[sessionName] = nbrKoPredictionsTeam1+ nbrKoPredictionsTeam2;
            
        }

        private static void UpdateWinningTeamPredictions(ApplicationDbContext context, HttpSessionStateBase session,
            short eventId, string userId, bool forceRefresh)
        {
            const string sessionName = "nbrWinningTeamPredictions";
            if (session[sessionName] != null && !forceRefresh)
                return;

            var nbrWinningTeamPredictions = (from a in context.KoWinningTeamPredictions
                where a.EventId == eventId
                      && a.PlayerId == userId
                select a).Count();

            session[sessionName] = nbrWinningTeamPredictions;
        }

        private static void UpdateBonusSessionVar(ApplicationDbContext context, HttpSessionStateBase session,
            short eventId, string userId, bool forceRefresh)
        {

            const string sessionName = "nbrBonusQuestionPredictions";
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