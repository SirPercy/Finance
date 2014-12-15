using System;
using System.Collections.Generic;
using Finance.Core.Utilities;
using System.Linq;
using Finance.Models.EF;
using Finance.Repository;
using Quartz;

namespace Finance.Core.Jobs
{
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
                var latestPost = GetLatestInsiderPost();
                var dateFrom = latestPost != null ? latestPost.Date.AddDays(1) : DateTime.Now.AddDays(-150);

                var daysToGet = (int) DateTime.Now.AddDays(-1).Subtract(dateFrom).TotalDays;
                if (daysToGet < 1)
                    return;

                var latestInsiderData = InsiderService.Get(dateFrom);
                _repository.StoreInsiderInfo(latestInsiderData);
                Logger.AddMessage("[OK] StoreInsiderInfoJob " + DateTime.Now);
 
            }
            catch (Exception ex) {
                Logger.AddMessage("[ERROR] StoreInsiderInfoJob " + ex.InnerException);

            }


        }

        private InsiderInfo GetLatestInsiderPost()
        {
            var list = _repository.GetInsiderList();
            var insiderPosts = new List<InsiderInfo>();

            foreach (var insiderInfo in list) {
                insiderPosts.AddRange(insiderInfo.InsiderInfoList);
            }
            return insiderPosts.OrderByDescending(p => p.Date).FirstOrDefault();
        }

    }
}