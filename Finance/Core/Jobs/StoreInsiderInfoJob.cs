using System;
using Finance.Core.Utilities;

namespace Finance.Core.Jobs
{
    public class StoreInsiderInfoJob
    {

        public void Execute()
        {
            for (var i = 1; i < 6; i++)
            {
                var dateToGetData = DateTime.Now.AddDays(-i);
                var latestInsiderData = InsiderService.Get(dateToGetData);
                var repostory = new Repository.Repository();
                repostory.StoreInsiderInfo(latestInsiderData, dateToGetData);
                
            }

        }

    }
}