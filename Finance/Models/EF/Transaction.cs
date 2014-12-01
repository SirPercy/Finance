using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finance.Models.EF
{
    [Table("Transaction")]
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public string Stock { get; set; }
        public DateTime Date { get; set; }
        public string TransactionType { get; set; }
        public double Number{ get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public double Result { get; set; }
     }
}