namespace GiftRegistry.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init_wishlist : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WishList", "OwnerGUID", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WishList", "OwnerGUID");
        }
    }
}
