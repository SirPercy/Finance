namespace Finance.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InsiderInfo",
                c => new
                    {
                        InsiderInfoId = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(),
                        Date = c.DateTime(nullable: false),
                        Person = c.String(),
                        Position = c.String(),
                        Transaction = c.String(),
                        Type = c.String(),
                        Number = c.String(),
                        Amount = c.Int(nullable: false),
                        QuoteId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InsiderInfoId)
                .ForeignKey("dbo.Quotes", t => t.QuoteId, cascadeDelete: true)
                .Index(t => t.QuoteId);
            
            CreateTable(
                "dbo.Quotes",
                c => new
                    {
                        QuoteId = c.Int(nullable: false, identity: true),
                        Company = c.String(),
                    })
                .PrimaryKey(t => t.QuoteId);
            
            CreateTable(
                "dbo.Tickers",
                c => new
                    {
                        TickerId = c.Int(nullable: false, identity: true),
                        TickerName = c.String(),
                        FullName = c.String(),
                    })
                .PrimaryKey(t => t.TickerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InsiderInfo", "QuoteId", "dbo.Quotes");
            DropIndex("dbo.InsiderInfo", new[] { "QuoteId" });
            DropTable("dbo.Tickers");
            DropTable("dbo.Quotes");
            DropTable("dbo.InsiderInfo");
        }
    }
}
