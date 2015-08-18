using System;
using System.Configuration;
using System.Linq;
using BuildVersioning.Data.Providers.EFProvider;
using BuildVersioning.Entities;

namespace BuildVersioning.Commands
{
	public abstract class BaseTest
	{
		protected static readonly string TestBuildDefinitionName = "TestBuildDefinition";
		protected static readonly string TestProjectName = "TestProject";
		protected static readonly string TestProjectConfigName = "TestProjectConfig";
		protected static readonly string TestRequestedBy = "TestUser";
		protected static readonly string TestTeamProjectName = "TestTeamProject";

		protected virtual CreateVersionCommand CreateNewCreateVersionCommand(
			string buildDefinitionName = null,
			string connectionString = null,
			string projectName = null,
			string projectConfigName = null,
			string requestedBy = null,
			string teamProjectName = null
			)
		{
			if (string.IsNullOrWhiteSpace(buildDefinitionName))
				buildDefinitionName = TestBuildDefinitionName;

			if (string.IsNullOrWhiteSpace(connectionString))
				connectionString = GetConfiguredConnectionString();

			if (string.IsNullOrWhiteSpace(projectName))
				projectName = TestProjectName;

			if (string.IsNullOrWhiteSpace(projectConfigName))
				projectConfigName = TestProjectConfigName;

			if (string.IsNullOrWhiteSpace(requestedBy))
				requestedBy = TestRequestedBy;

			if (string.IsNullOrWhiteSpace(teamProjectName))
				teamProjectName = TestTeamProjectName;

			var log = new NullCommandLog();

			var command =
				new CreateVersionCommand
				{
					BuildDefinitionName = buildDefinitionName,
					ConnectionString = connectionString,
					CommandLog = log,
					ProjectName = projectName,
					ProjectConfigName = projectConfigName,
					RequestedBy = requestedBy,
					TeamProjectName = teamProjectName
				};
			return command;
		}

		protected virtual void Initialize(string connectionString = null)
		{
			if (string.IsNullOrWhiteSpace(connectionString))
				connectionString = GetConfiguredConnectionString();

			using (var db = new BuildVersioningDataContext(connectionString))
			{
				DeleteTestProjectAndTestProjectConfig(db);

				var project =
					new Project
					{
						Name = TestProjectName,
						BuildNumber = 0,
						DateBuildNumberUpdated = DateTime.Now
					};
				db.Projects.Add(project);
				db.SaveChanges();

				var projectConfig =
					new ProjectConfig
					{
						ProjectId = project.Id,
						Name = TestProjectConfigName,
						GeneratedBuildNumberPosition = 3,
						GeneratedVersionPart1 = 1,
						GeneratedVersionPart2 = 0,
						GeneratedVersionPart3 = 0,
						GeneratedVersionPart4 = 0,
						ProductVersionPart1 = 1,
						ProductVersionPart2 = 0,
						ProductVersionPart3 = 0,
						ProductVersionPart4 = 0,
						ReleaseType = ReleaseType.PreRelease
					};
				db.ProjectConfigs.Add(projectConfig);
				db.SaveChanges();
			}
		}

		protected string GetConfiguredConnectionString()
		{
			return ConfigurationManager.ConnectionStrings["BuildVersions"].ConnectionString;
		}

		protected Project GetTestProject(string connectionString = null)
		{
			if (string.IsNullOrWhiteSpace(connectionString))
				connectionString = GetConfiguredConnectionString();

			using (var db = new BuildVersioningDataContext(connectionString))
			{
				return db.Projects.SingleOrDefault(p => p.Name == TestProjectName);
			}
		}

		protected ProjectConfig GetTestProjectConfig(string connectionString = null)
		{
			if (string.IsNullOrWhiteSpace(connectionString))
				connectionString = GetConfiguredConnectionString();

			using (var db = new BuildVersioningDataContext(connectionString))
			{
				return db.ProjectConfigs.SingleOrDefault(p => p.Name == TestProjectConfigName);
			}
		}

		private static void DeleteTestProjectAndTestProjectConfig(BuildVersioningDataContext db)
		{
			var projectExists = db.Projects.Any(p => p.Name == TestProjectName);

			if (projectExists)
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{

						#region " Delete Test VersionHistoryItems SQL "

						var sql = string.Format(@"DELETE FROM [dbo].[VersionHistory]
WHERE [ProjectConfigId] IN (
	SELECT PC.[Id]
	FROM [dbo].[ProjectConfig] PC
	INNER JOIN [dbo].[Project] P ON PC.[ProjectId] = P.[Id]
	WHERE P.[Name] = N'{0}'
);", TestProjectName);

						#endregion

						db.Database.ExecuteSqlCommand(sql);

						sql = string.Format("DELETE FROM [dbo].[ProjectConfig] WHERE [ProjectId] = (SELECT [Id] FROM [dbo].[Project] WHERE [Name] = N'{0}');", TestProjectName);
						db.Database.ExecuteSqlCommand(sql);

						sql = string.Format("DELETE FROM [dbo].[Project] WHERE [Name] = N'{0}';", TestProjectName);
						db.Database.ExecuteSqlCommand(sql);

						transaction.Commit();
					}
					catch
					{
						transaction.Rollback();
						throw;
					}
				}
			}
		}
	}
}