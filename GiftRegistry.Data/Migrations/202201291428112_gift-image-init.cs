namespace GiftRegistry.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class giftimageinit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Gift", "ProductImage", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Gift", "ProductImage");
        }
    }
}
