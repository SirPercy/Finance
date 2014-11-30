using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Models.EF
{
    [Table("Quotes")]
    public class Quote
    { 
        public Quote()
        {
            InsiderInfoList = new List<InsiderInfo>();
        }
        [Key]
        public int QuoteId { get; set; }
        public string Company { get; set; }
        public List<InsiderInfo> InsiderInfoList { get; set; }
 
    }
}