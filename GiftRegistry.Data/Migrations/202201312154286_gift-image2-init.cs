namespace GiftRegistry.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class giftimage2init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomImage",
                c => new
                    {
                        ImageID = c.Int(nullable: false, identity: true),
                        OwnerGUID = c.Guid(nullable: false),
                        ImageData = c.Binary(nullable: false),
                    })
                .PrimaryKey(t => t.ImageID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CustomImage");
        }
    }
}
