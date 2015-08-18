namespace BuildVersioning.Data.Providers.EFProvider.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProjectConfig",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProjectId = c.Long(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 400),
                        GeneratedBuildNumberPosition = c.Int(nullable: false),
                        GeneratedVersionPart1 = c.Int(nullable: false),
                        GeneratedVersionPart2 = c.Int(nullable: false),
                        GeneratedVersionPart3 = c.Int(nullable: false),
                        GeneratedVersionPart4 = c.Int(nullable: false),
                        ProductVersionPart1 = c.Int(nullable: false),
                        ProductVersionPart2 = c.Int(nullable: false),
                        ProductVersionPart3 = c.Int(nullable: false),
                        ProductVersionPart4 = c.Int(nullable: false),
                        ReleaseType = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Project", t => t.ProjectId)
                .Index(t => new { t.ProjectId, t.Name }, unique: true, name: "UX_ProjectConfig_ProjectId_Name")
                .Index(t => t.ReleaseType, name: "IX_ProjectConfig_ReleaseType");
            
            CreateTable(
                "dbo.Project",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 400),
                        BuildNumber = c.Int(nullable: false),
                        DateBuildNumberUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "UX_Project_Name")
                .Index(t => t.DateBuildNumberUpdated, name: "IX_Project_DateBuildNumberUpdated");
            
            CreateTable(
                "dbo.VersionHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProjectId = c.Long(nullable: false),
                        ProjectName = c.String(nullable: false, maxLength: 100),
                        ProjectConfigId = c.Long(nullable: false),
                        ProjectConfigName = c.String(nullable: false, maxLength: 100),
                        Date = c.DateTime(nullable: false),
                        BuildNumber = c.Int(nullable: false),
                        Version = c.String(nullable: false, maxLength: 100),
                        SemanticVersion = c.String(nullable: false, maxLength: 100),
                        SemanticVersionSuffix = c.String(nullable: false, maxLength: 100),
                        ProductVersion = c.String(nullable: false, maxLength: 100),
                        ReleaseType = c.String(nullable: false, maxLength: 100),
                        BuildDefinitionName = c.String(nullable: false, maxLength: 100),
                        RequestedBy = c.String(nullable: false, maxLength: 100),
                        TeamProjectName = c.String(nullable: false, maxLength: 100),
                        GeneratedBuildNumberPosition = c.Int(nullable: false),
                        GeneratedVersionPart1 = c.Int(nullable: false),
                        GeneratedVersionPart2 = c.Int(nullable: false),
                        GeneratedVersionPart3 = c.Int(nullable: false),
                        GeneratedVersionPart4 = c.Int(nullable: false),
                        ProductVersionPart1 = c.Int(nullable: false),
                        ProductVersionPart2 = c.Int(nullable: false),
                        ProductVersionPart3 = c.Int(nullable: false),
                        ProductVersionPart4 = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ProjectId, name: "IX_VersionHistory_ProjectId")
                .Index(t => t.ProjectName, name: "IX_VersionHistory_ProjectName")
                .Index(t => t.ProjectConfigId, name: "IX_VersionHistory_ProjectConfigId")
                .Index(t => t.ProjectConfigName, name: "IX_VersionHistory_ProjectConfigName")
                .Index(t => t.Date, name: "IX_VersionHistory_Date")
                .Index(t => t.BuildNumber, name: "IX_VersionHistory_BuildNumber")
                .Index(t => t.Version, name: "IX_VersionHistory_Version")
                .Index(t => t.SemanticVersion, name: "IX_VersionHistory_SemanticVersion")
                .Index(t => t.SemanticVersionSuffix, name: "IX_VersionHistory_SemanticVersionSuffix")
                .Index(t => t.ProductVersion, name: "IX_VersionHistory_ProductVersion")
                .Index(t => t.ReleaseType, name: "IX_VersionHistory_ReleaseType")
                .Index(t => t.BuildDefinitionName, name: "IX_VersionHistory_BuildDefinitionName")
                .Index(t => t.RequestedBy, name: "IX_VersionHistory_RequestedBy")
                .Index(t => t.TeamProjectName, name: "IX_VersionHistory_TeamProjectName");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectConfig", "ProjectId", "dbo.Project");
            DropIndex("dbo.VersionHistory", "IX_VersionHistory_TeamProjectName");
            DropIndex("dbo.VersionHistory", "IX_VersionHistory_RequestedBy");
            DropIndex("dbo.VersionHistory", "IX_VersionHistory_BuildDefinitionName");
            DropIndex("dbo.VersionHistory", "IX_VersionHistory_ReleaseType");
            DropIndex("dbo.VersionHistory", "IX_VersionHistory_ProductVersion");
            DropIndex("dbo.VersionHistory", "IX_VersionHistory_SemanticVersionSuffix");
            DropIndex("dbo.VersionHistory", "IX_VersionHistory_SemanticVersion");
            DropIndex("dbo.VersionHistory", "IX_VersionHistory_Version");
            DropIndex("dbo.VersionHistory", "IX_VersionHistory_BuildNumber");
            DropIndex("dbo.VersionHistory", "IX_VersionHistory_Date");
            DropIndex("dbo.VersionHistory", "IX_VersionHistory_ProjectConfigName");
            DropIndex("dbo.VersionHistory", "IX_VersionHistory_ProjectConfigId");
            DropIndex("dbo.VersionHistory", "IX_VersionHistory_ProjectName");
            DropIndex("dbo.VersionHistory", "IX_VersionHistory_ProjectId");
            DropIndex("dbo.Project", "IX_Project_DateBuildNumberUpdated");
            DropIndex("dbo.Project", "UX_Project_Name");
            DropIndex("dbo.ProjectConfig", "IX_ProjectConfig_ReleaseType");
            DropIndex("dbo.ProjectConfig", "UX_ProjectConfig_ProjectId_Name");
            DropTable("dbo.VersionHistory");
            DropTable("dbo.Project");
            DropTable("dbo.ProjectConfig");
        }
    }
}
