using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using BuildVersioning.Data.Providers.EFProvider.Migrations;
using BuildVersioning.Entities;

namespace BuildVersioning.Data.Providers.EFProvider
{
	/// <summary>
	/// The data context.
	/// </summary>
	public class BuildVersioningDataContext : DbContext
	{
		private const string DefaultConnectionStringName = "BuildVersions";

		/// <summary>
		/// Initializes the <see cref="BuildVersioningDataContext"/> class.
		/// </summary>
		static BuildVersioningDataContext()
		{
			Database.SetInitializer(new MigrateDatabaseToLatestVersion<BuildVersioningDataContext, Configuration>());
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BuildVersioningDataContext"/> class.
		/// </summary>
		public BuildVersioningDataContext()
			: base(DefaultConnectionStringName)
		{
			Database.SetInitializer(new MigrateDatabaseToLatestVersion<BuildVersioningDataContext, Configuration>());
			Configuration.LazyLoadingEnabled = false;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BuildVersioningDataContext"/> class.
		/// </summary>
		/// <param name="nameOrConnectionString">The name or connection string.</param>
		public BuildVersioningDataContext(string nameOrConnectionString)
			: base(nameOrConnectionString)
		{
			Database.SetInitializer(new MigrateDatabaseToLatestVersion<BuildVersioningDataContext, Configuration>());
			Configuration.LazyLoadingEnabled = false;
		}

		/// <summary>
		/// Gets or sets the projects.
		/// </summary>
		/// <value>
		/// The projects.
		/// </value>
		public IDbSet<Project> Projects { get; set; }

		/// <summary>
		/// Gets or sets the project configs.
		/// </summary>
		/// <value>
		/// The project configs.
		/// </value>
		public IDbSet<ProjectConfig> ProjectConfigs { get; set; }

		/// <summary>
		/// Gets or sets the version history items.
		/// </summary>
		/// <value>
		/// The version history items.
		/// </value>
		public IDbSet<VersionHistoryItem> VersionHistoryItems { get; set; }

		/// <summary>
		/// This method is called when the model for a derived context has been initialized, but
		/// before the model has been locked down and used to initialize the context.  The default
		/// implementation of this method does nothing, but it can be overridden in a derived class
		/// such that the model can be further configured before it is locked down.
		/// </summary>
		/// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
		/// <remarks>
		/// Typically, this method is called only once when the first instance of a derived context
		/// is created.  The model for that context is then cached and is for all further instances of
		/// the context in the app domain.  This caching can be disabled by setting the ModelCaching
		/// property on the given ModelBuidler, but note that this can seriously degrade performance.
		/// More control over caching is provided through use of the DbModelBuilder and DbContextFactory
		/// classes directly.
		/// </remarks>
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			ConfigureProject(modelBuilder);
			ConfigureProjectConfig(modelBuilder);
			ConfigureVersionHistoryItem(modelBuilder);
		}

		/// <summary>
		/// Configures the project entity.
		/// </summary>
		/// <param name="modelBuilder">The model builder.</param>
		protected void ConfigureProject(DbModelBuilder modelBuilder)
		{
			var entityConfig = modelBuilder.Entity<Project>();
			entityConfig.ToTable("Project").HasKey(p => p.Id);

			entityConfig.Property(p => p.Id).HasColumnOrder(0).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			entityConfig.Property(p => p.BuildNumber).HasColumnOrder(30).IsRequired();
			entityConfig.Property(p => p.DateBuildNumberUpdated).HasColumnOrder(40).IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Project_DateBuildNumberUpdated") { IsUnique = false }));
			entityConfig.Property(p => p.Description).HasColumnOrder(20).IsOptional().HasMaxLength(400);
			entityConfig.Property(p => p.Name).HasColumnOrder(10).IsRequired().HasMaxLength(100).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UX_Project_Name") { IsUnique = true }));
		}

		/// <summary>
		/// Configures the project configuration entity.
		/// </summary>
		/// <param name="modelBuilder">The model builder.</param>
		protected void ConfigureProjectConfig(DbModelBuilder modelBuilder)
		{
			var entityConfig = modelBuilder.Entity<ProjectConfig>();
			entityConfig.ToTable("ProjectConfig").HasKey(p => p.Id);

			entityConfig.Ignore(p => p.ReleaseType);

			entityConfig
				.HasRequired(p => p.Project)
				.WithMany(p => p.ProjectConfigs)
				.HasForeignKey(p => p.ProjectId)
				.WillCascadeOnDelete(false);

			entityConfig.Property(p => p.Id).HasColumnOrder(0).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			entityConfig.Property(p => p.Description).HasColumnOrder(30).IsOptional().HasMaxLength(400);
			entityConfig.Property(p => p.Name).HasColumnOrder(20).IsRequired().HasMaxLength(100).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UX_ProjectConfig_ProjectId_Name") { IsUnique = true, Order = 2 }));
			entityConfig.Property(p => p.ProjectId).HasColumnOrder(10).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UX_ProjectConfig_ProjectId_Name") { IsUnique = true, Order = 1 }));
			entityConfig.Property(p => p.GeneratedBuildNumberPosition).HasColumnOrder(90).IsRequired();

			entityConfig.Property(p => p.GeneratedVersionPart1).HasColumnOrder(110).IsRequired();
			entityConfig.Property(p => p.GeneratedVersionPart2).HasColumnOrder(120).IsRequired();
			entityConfig.Property(p => p.GeneratedVersionPart3).HasColumnOrder(130).IsRequired();
			entityConfig.Property(p => p.GeneratedVersionPart4).HasColumnOrder(140).IsRequired();

			entityConfig.Property(p => p.ProductVersionPart1).HasColumnOrder(210).IsRequired();
			entityConfig.Property(p => p.ProductVersionPart2).HasColumnOrder(220).IsRequired();
			entityConfig.Property(p => p.ProductVersionPart3).HasColumnOrder(230).IsRequired();
			entityConfig.Property(p => p.ProductVersionPart4).HasColumnOrder(240).IsRequired();

			entityConfig.Property(p => p.ReleaseTypeString).HasColumnOrder(300).IsRequired().HasMaxLength(100).HasColumnName("ReleaseType").HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_ProjectConfig_ReleaseType") { IsUnique = false }));
		}

		/// <summary>
		/// Configures the version history item entity.
		/// </summary>
		/// <param name="modelBuilder">The model builder.</param>
		protected void ConfigureVersionHistoryItem(DbModelBuilder modelBuilder)
		{
			// NOTE: There are no foreign keys from VersionHistory to any other table by design.
			//       VersionHistory is a log table and as such must not have any direct references to other tables
			//       so that if a Project or ProjectConfig is deleted, it history remains intact.

			var entityConfig = modelBuilder.Entity<VersionHistoryItem>();
			entityConfig.ToTable("VersionHistory").HasKey(p => p.Id);

			entityConfig.Ignore(p => p.ReleaseType);

			entityConfig.Property(p => p.Id).HasColumnOrder(0).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			entityConfig.Property(p => p.BuildDefinitionName).HasColumnOrder(50).IsRequired().HasMaxLength(100).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_VersionHistory_BuildDefinitionName") { IsUnique = false }));
			entityConfig.Property(p => p.BuildNumber).HasColumnOrder(29).IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_VersionHistory_BuildNumber") { IsUnique = false }));
			entityConfig.Property(p => p.Date).HasColumnOrder(20).IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_VersionHistory_Date") { IsUnique = false }));
			entityConfig.Property(p => p.ProductVersion).HasColumnOrder(40).IsRequired().HasMaxLength(100).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_VersionHistory_ProductVersion") { IsUnique = false }));
			entityConfig.Property(p => p.ProjectId).HasColumnOrder(8).IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_VersionHistory_ProjectId") { IsUnique = false }));
			entityConfig.Property(p => p.ProjectName).HasColumnOrder(9).IsRequired().HasMaxLength(100).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_VersionHistory_ProjectName") { IsUnique = false }));
			entityConfig.Property(p => p.ProjectConfigId).HasColumnOrder(10).IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_VersionHistory_ProjectConfigId") { IsUnique = false }));
			entityConfig.Property(p => p.ProjectConfigName).HasColumnOrder(11).IsRequired().HasMaxLength(100).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_VersionHistory_ProjectConfigName") { IsUnique = false }));
			entityConfig.Property(p => p.ReleaseTypeString).HasColumnOrder(48).IsRequired().HasMaxLength(100).HasColumnName("ReleaseType").HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_VersionHistory_ReleaseType") { IsUnique = false }));
			entityConfig.Property(p => p.RequestedBy).HasColumnOrder(60).IsRequired().HasMaxLength(100).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_VersionHistory_RequestedBy") { IsUnique = false }));
			entityConfig.Property(p => p.TeamProjectName).HasColumnOrder(70).IsRequired().HasMaxLength(100).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_VersionHistory_TeamProjectName") { IsUnique = false }));
			entityConfig.Property(p => p.SemanticVersion).HasColumnOrder(32).IsRequired().HasMaxLength(100).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_VersionHistory_SemanticVersion") { IsUnique = false }));
			entityConfig.Property(p => p.SemanticVersionSuffix).HasColumnOrder(34).IsRequired().HasMaxLength(100).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_VersionHistory_SemanticVersionSuffix") { IsUnique = false }));
			entityConfig.Property(p => p.Version).HasColumnOrder(30).IsRequired().HasMaxLength(100).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_VersionHistory_Version") { IsUnique = false }));

			entityConfig.Property(p => p.GeneratedBuildNumberPosition).HasColumnOrder(100).IsRequired();
			entityConfig.Property(p => p.GeneratedVersionPart1).HasColumnOrder(110).IsRequired();
			entityConfig.Property(p => p.GeneratedVersionPart2).HasColumnOrder(120).IsRequired();
			entityConfig.Property(p => p.GeneratedVersionPart3).HasColumnOrder(130).IsRequired();
			entityConfig.Property(p => p.GeneratedVersionPart4).HasColumnOrder(140).IsRequired();

			entityConfig.Property(p => p.ProductVersionPart1).HasColumnOrder(210).IsRequired();
			entityConfig.Property(p => p.ProductVersionPart2).HasColumnOrder(220).IsRequired();
			entityConfig.Property(p => p.ProductVersionPart3).HasColumnOrder(230).IsRequired();
			entityConfig.Property(p => p.ProductVersionPart4).HasColumnOrder(240).IsRequired();
		}
	}
}