using System;

namespace Finance.Models.Entities
{
    public class BuyHistory
    {
        public DateTime Date { get; set; }
        public string Buyer { get; set; }
        public int Number { get; set; }
    }
}