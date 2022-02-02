namespace GiftRegistry.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notificationsaddedinit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notification",
                c => new
                    {
                        NotificationID = c.Int(nullable: false, identity: true),
                        NotificationType = c.Int(nullable: false),
                        Message = c.String(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        RecipientID = c.Int(nullable: false),
                        SenderID = c.Int(),
                    })
                .PrimaryKey(t => t.NotificationID)
                .ForeignKey("dbo.Person", t => t.RecipientID, cascadeDelete: true)
                .ForeignKey("dbo.Person", t => t.SenderID)
                .Index(t => t.RecipientID)
                .Index(t => t.SenderID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notification", "SenderID", "dbo.Person");
            DropForeignKey("dbo.Notification", "RecipientID", "dbo.Person");
            DropIndex("dbo.Notification", new[] { "SenderID" });
            DropIndex("dbo.Notification", new[] { "RecipientID" });
            DropTable("dbo.Notification");
        }
    }
}
