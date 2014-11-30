using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Models.EF
{
    [Table("InsiderInfo")]
    public class InsiderInfo
    {
        [Key]
        public int InsiderInfoId { get; set; }
        public string CompanyName { get; set; }
        public DateTime Date { get; set; }
        public string Person { get; set; }
        public string Position { get; set; }
        public string Transaction { get; set; }
        public string Type { get; set; }
        public string Number { get; set; }
        public int Amount { get; set; }
        public int QuoteId { get; set; }
        public virtual Quote Quote { get; set; }

    }
}