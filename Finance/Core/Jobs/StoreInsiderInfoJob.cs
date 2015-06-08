using System;
using System.Collections.Generic;
using System.Web;
using Finance.Core.Utilities;
using System.Linq;
using Finance.Models.EF;
using Finance.Repository;
using Quartz;

namespace Finance.Core.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class StoreInsiderInfoJob : IJob
    {
        private readonly IRepository _repository;
        public StoreInsiderInfoJob()
        {
            _repository = new Repository.Repository();
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                //delete last 10 days though insider info can be reported within 10 days
                if (!DeleteLastPost())
                    return;
               
                var latestPosts = GetLatestInsiderPost();
                var latestPost = latestPosts.OrderByDescending(p => p.Date).FirstOrDefault();
                

                var dateFrom = latestPost != null ? latestPost.Date.AddDays(1) : DateTime.Now.AddDays(-150);

                var daysToGet = (int) DateTime.Now.AddDays(-1).Subtract(dateFrom).TotalDays;
                if (daysToGet < 1)
                    return;

                var dateTo = daysToGet > 30 ? DateTime.Now.AddDays(-(daysToGet - 30)) : DateTime.Now.AddDays(-1);
                var latestInsiderData = new InsiderService().Get(dateFrom, dateTo);
                _repository.StoreInsiderInfo(latestInsiderData);
                HttpRuntime.Cache.Remove("insider");
                Logger.AddMessage("[OK] StoreInsiderInfoJob " + DateTime.Now);
 
            }
            catch (Exception ex) {
                Logger.AddMessage("[ERROR] StoreInsiderInfoJob " + ex.InnerException);

            }
        }
        private bool DeleteLastPost(int numberOfDaysToRemove = 20)
        {
            try
            {
                using (var db = new Models.EF.Context())
                {
                    var dateFrom = DateTime.Now.AddDays(-numberOfDaysToRemove);
                    var itemsToDelete = db.InsiderInfo.Where(x => x.Date > dateFrom);
                    db.InsiderInfo.RemoveRange(itemsToDelete);
                    db.SaveChanges();
                    return true;
                }
            }
            catch { return false; }
        }

        private List<InsiderInfo> GetLatestInsiderPost()
        {
            var list = _repository.GetInsiderList();
            var insiderPosts = new List<InsiderInfo>();

            foreach (var insiderInfo in list) {
                insiderPosts.AddRange(insiderInfo.InsiderInfoList);
            }
            return insiderPosts;
        }

    }
}