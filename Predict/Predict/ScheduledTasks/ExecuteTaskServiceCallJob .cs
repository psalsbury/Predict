using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Predict.Models;
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
                            RapidApiHelper.GetRapidApiResults();
                            RapidApiHelper.DailyRapidApiLeagueCheck();
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