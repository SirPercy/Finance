using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Finance.App_Start;
using Quartz;
using Quartz.Impl;

namespace Finance
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            TriggerScheduler();

     
        }
        protected void Application_BeginRequest() {
            //if (HttpContext.Current.Request.Url.ToString().ToLower().Contains("www.insider.se")) {
            //    HttpContext.Current.Response.Status = "301 Moved Permanently";
            //    HttpContext.Current.Response.AddHeader("Location", Request.Url.ToString().ToLower().Replace("www.insider.se/", "insider.se/"));
            //}
        }

        private static void TriggerScheduler()
        {
            var job = new JobDetailImpl("StoreInsiderInfoScheduler", typeof(Core.Jobs.StoreInsiderInfoJob));
            var trigger = TriggerBuilder.Create();
            //trigger.WithSchedule(SimpleScheduleBuilder.RepeatMinutelyForever(1));
            trigger.WithSchedule(CronScheduleBuilder.CronSchedule(new CronExpression("0 0 9-10 * * ?")));
            trigger.StartNow();

            var jobTwo = new JobDetailImpl("StoreActionScheduler", typeof(Core.Jobs.FindAndStoreActionJob));
            var triggerTwo = TriggerBuilder.Create();
            //triggerTwo.WithSchedule(SimpleScheduleBuilder.RepeatMinutelyForever(2));
            triggerTwo.WithSchedule(CronScheduleBuilder.CronSchedule(new CronExpression("0 0 10-11 * * ?")));
            triggerTwo.StartNow();


            var jobThree = new JobDetailImpl("UpdatePortfolioScheduler", typeof(Core.Jobs.UpdatePortfolioJob));
            var triggerThree = TriggerBuilder.Create();
            //triggerThree.WithSchedule(SimpleScheduleBuilder.RepeatMinutelyForever(1));
            triggerThree.WithSchedule(CronScheduleBuilder.CronSchedule(new CronExpression("0 0 17-18 * * ?")));
            triggerThree.StartNow();
 

            var schedFact = new StdSchedulerFactory();
            var sched = schedFact.GetScheduler();
            sched.ScheduleJob(job, trigger.Build());
            sched.ScheduleJob(jobTwo, triggerTwo.Build());
            sched.ScheduleJob(jobThree, triggerThree.Build());
            sched.Start();

        }
    }
}