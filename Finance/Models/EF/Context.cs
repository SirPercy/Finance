using System.Data.Entity;

namespace Finance.Models.EF
{
    public class Context : DbContext
    {
        public Context() : base("Finance")
        {
            
        }
        public DbSet<Quote> Quote { get; set; }
        public DbSet<InsiderInfo> InsiderInfo { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Ticker> TickerList { get; set; }
        public DbSet<Portfolio> Portfolio { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            //one-to-many 
            modelBuilder.Entity<InsiderInfo>().HasRequired<Quote>(s => s.Quote)
            .WithMany(s => s.InsiderInfoList).HasForeignKey(s => s.QuoteId);
     
        }
    }
}