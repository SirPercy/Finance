using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Models.EF
{
    [Table("Portfolio")]
    public class Portfolio
    {
        [Key]
        public int Id { get; set; }

        public string Stock { get; set; }
        public DateTime BuyDate { get; set; }
        public double BuyNumber{ get; set; }
        public double BuyPrice { get; set; }
        public double BuyAmount { get; set; }
        public DateTime SellDate { get; set; }
        public double SellPrice { get; set; }
        public double SellAmount { get; set; }
        public double Result { get; set; }
    }
}