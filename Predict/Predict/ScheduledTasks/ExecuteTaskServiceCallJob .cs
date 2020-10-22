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
                            //Do whatever stuff you want
                            var myContext = new ApplicationDbContext();
                            var poo = new SiteSetting
                            {
                                CreatedDateTime = DateTime.UtcNow,
                                ModifiedDateTime = DateTime.UtcNow,
                                SettingName = DateTime.UtcNow.ToShortDateString() + ' ' + DateTime.UtcNow.ToLongTimeString(),
                                SettingValue = DateTime.Today.ToShortDateString()
                            };
                            myContext.SiteSettings.Add(poo);
                            myContext.SaveChanges();
                            myContext.Dispose();
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