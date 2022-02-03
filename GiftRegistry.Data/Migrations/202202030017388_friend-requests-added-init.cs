namespace GiftRegistry.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class friendrequestsaddedinit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Friend", "IsPending", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Friend", "IsPending");
        }
    }
}
