using System.Linq;
using System.Web;
using Predict.Models;

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
            UpdateKoSessionVar(context, session, eventId, userId);
        }

        public static void RefreshFixturePredictions(HttpSessionStateBase session, string userId)
        {
            var context = new ApplicationDbContext();
            var eventId = Predict.Helper.Cache.GetEventId();
            UpdateFixturesSessionVar(context, session, eventId, userId);
        }

        public static void SetUserSessionVariables(HttpSessionStateBase session, string userId)
        {
            var context = new ApplicationDbContext();
            var eventId = Predict.Helper.Cache.GetEventId();
            var player = context.Players.FirstOrDefault(p => p.Id == userId);
            if (player != null)
            {
                session["Player"] = player;
            }

            UpdateKoSessionVar(context, session, eventId, userId);
            UpdateFixturesSessionVar(context, session, eventId, userId);
            UpdatePositionHistorySessionVar(context, session, eventId, userId);
            UpdateBonusSessionVar(context, session, eventId, userId);
            session.Timeout = 252000; // 180 day

        }

        public static void ClearSessionVariables(HttpSessionStateBase session)
        {
            session.Clear();
        }

        public static void UpdatePositionHistorySessionVar(ApplicationDbContext context, HttpSessionStateBase session,
            short eventId, string userId)
        {

            var posnHistory = (from a in context.PoolPlayerPositionHistory
                join c in context.Pools on a.PoolId equals c.Id
                where c.EventId == eventId && a.PlayerId == userId
                select a).ToList();
            session["posnHistory"] = posnHistory;
        }

        private static void UpdateFixturesSessionVar(ApplicationDbContext context, HttpSessionStateBase session,
            short eventId, string userId)
        {
            var nbrFixturePredictions = (from a in context.FixturePredictions
                join c in context.Fixtures on a.FixtureId equals c.Id
                where c.EventId == eventId
                      && a.PlayerId == userId
                select a).Count();

            session["nbrFixturePredictions"] = nbrFixturePredictions;
        }

        private static void UpdateKoSessionVar(ApplicationDbContext context, HttpSessionStateBase session, short eventId, string userId)
        {
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

            var nbrWinningTeamPredictions = (from a in context.KoWinningTeamPredictions
                where a.EventId == eventId
                      && a.PlayerId == userId
                select a).Count();

            session["nbrKoPredictions"] = nbrKoPredictionsTeam1+ nbrKoPredictionsTeam2;
            session["nbrWinningTeamPredictions"] = nbrWinningTeamPredictions;
        }

        private static void UpdateBonusSessionVar(ApplicationDbContext context, HttpSessionStateBase session,
            short eventId, string userId)
        {

            var nbrBonusQuestionPredictions = (from a in context.BonusQuestions
                join c in context.BonusQuestionPredictions on a.Id equals c.BonusQuestionId
                where a.EventId == eventId
                select a).Count();

            session["nbrBonusQuestionPredictions"] = nbrBonusQuestionPredictions;

        }


    }



}