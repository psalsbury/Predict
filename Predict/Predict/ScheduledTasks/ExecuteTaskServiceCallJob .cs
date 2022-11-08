using System;
using System.Threading.Tasks;
using Predict.RapidApi;
using Quartz;

namespace Predict.ScheduledTasks
{
    public class Test
    {
        public class ExecuteTaskServiceCallJob : IJob
        {
            public static readonly string SchedulingStatus = System.Configuration.ConfigurationManager.AppSettings["ExecuteTaskServiceCallSchedulingStatus"];
            public Task Execute(IJobExecutionContext context)
            {
                var task = Task.Run(() =>
                {
                    if (SchedulingStatus.Equals("ON"))
                    {
                        try
                        {
                            // Check if result is needed to be checked
                            RapidApiHelper.GetRapidApiResults();        // Check to see if any fixtures have finished. IF they have, make a call to get the result
                            RapidApiHelper.DailyRapidApiLeagueCheck();  // Called once per day to ensure that all fixtures are up to date
                            Predict.Helper.EmailLeadProcessor.SendLeadEmail();
                            Predict.Helper.EmailRequestToJoinProcessor.SendRequestToJoinEmail();
                        }
                        catch (Exception ex)
                        {
                            
                            // Do nothing
                        }
                    }
                });
                return task;
            }
        }
    }
}