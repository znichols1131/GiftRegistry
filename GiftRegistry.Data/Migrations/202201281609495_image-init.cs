namespace GiftRegistry.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class imageinit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "ProfilePicture", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Person", "ProfilePicture");
        }
    }
}
