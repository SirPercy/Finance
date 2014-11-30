using System.ComponentModel.DataAnnotations;

namespace Finance.Models.EF
{
    public class Ticker
    {
        [Key]
        public int TickerId { get; set; }
        public string TickerName { get; set; }
        public string FullName { get; set; }
    }
}