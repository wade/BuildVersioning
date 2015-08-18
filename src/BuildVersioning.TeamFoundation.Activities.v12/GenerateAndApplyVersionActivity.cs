using System;
using System.Activities;
using System.IO;
using BuildVersioning.Commands;
using Microsoft.TeamFoundation.Build.Client;

namespace BuildVersioning.TeamFoundation.Activities
{
	/// <summary>
	/// Generates the next version and updates the build to use the version and optionally updated the drop location directory name to use the date and version.
	/// </summary>
	/// <remarks>
	/// This task generates the next version and optionally sets the TFS IBuildDetails.BuildNumber and/or IBuildDetails.DropLocation properties.
	/// The generated version details
	/// </remarks>
	[BuildActivity(HostEnvironmentOption.All)]
	public class GenerateAndApplyVersionActivity : CodeActivity<BuildVersionDetails>
	{
		/// <summary>
		/// Gets or sets the database connection string.
		/// </summary>
		/// <value>
		/// The database connection string.
		/// </value>
		[RequiredArgument]
		public InArgument<string> DatabaseConnectionString { get; set; }

		/// <summary>
		/// Gets or sets the lock timeout in seconds.
		/// </summary>
		/// <value>
		/// The lock timeout in seconds.
		/// </value>
		/// <remarks>
		/// The lock timeout value is the maximum amount of time that the synchronization lock will
		/// wait to be acquired, after which a timeout occurs and the command execution will fail.
		/// <para>If the value is less than one, the default lock timeout value of 60 seconds will be used.</para>
		/// </remarks>
		public InArgument<int> LockTimeoutSeconds { get; set; }

		/// <summary>
		/// Gets or sets the name of the project configuration for which the version will be obtained.
		/// </summary>
		/// <value>
		/// The name of the project configuration.
		/// </value>
		/// <remarks>
		/// The <see cref="ProjectConfigName"/> property is required and defaults to null if not set, which is not valid for task execution.
		/// The config name value must be valid for the project specified by the <see cref="ProjectName"/> property.
		/// </remarks>
		[RequiredArgument]
		public InArgument<string> ProjectConfigName { get; set; }

		/// <summary>
		/// Gets or sets the name of the project for which the version will be obtained.
		/// </summary>
		/// <value>
		/// The name of the project.
		/// </value>
		/// <remarks>
		/// The <see cref="ProjectName"/> property is required and defaults to null if not set, which is not valid for task execution.
		/// The project name value must be the name of a valid project that is configured on the Build Versioning database.
		/// </remarks>
		[RequiredArgument]
		public InArgument<string> ProjectName { get; set; }

		/// <summary>
		/// Gets or sets a value that determines if the current build number should be overridden with the generated version.
		/// </summary>
		/// <value>
		/// <c>true</c> if the current build number should be overridden with the generated version; otherwise, <c>false</c>
		/// </value>
		public InArgument<bool> ShouldSetBuildNumber { get; set; }

		/// <summary>
		/// Gets or sets a value that determines if the current drop location folder name should be overridden with this activity's format as follows: BuildDefinitionName_DateTime_Version.
		/// </summary>
		/// <value>
		/// <c>true</c> if the current drop location folder name should be overridden with this activity's format as follows: BuildDefinitionName_DateTime_Version; otherwise, <c>false</c>
		/// </value>
		public InArgument<bool> ShouldSetDropLocation { get; set; }

		/// <summary>
		/// Caches the metadata.
		/// </summary>
		/// <param name="metadata">The metadata.</param>
		protected override void CacheMetadata(CodeActivityMetadata metadata)
		{
			base.CacheMetadata(metadata);
			metadata.RequireExtension(typeof(IBuildDetail));
		}

		/// <summary>
		/// Executes the activity.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>
		/// A <see cref="BuildVersionDetails"/> instance representing the generated version.
		/// </returns>
		protected override BuildVersionDetails Execute(CodeActivityContext context)
		{
			// Validate required input properties.
			var connectionString = DatabaseConnectionString.Get(context);
			if (string.IsNullOrWhiteSpace(connectionString))
				throw new InvalidOperationException("The DatabaseConnectionString property is required and cannot be null, empty or whitespace.");

			var projectName = ProjectName.Get(context);
			if (string.IsNullOrWhiteSpace(projectName))
				throw new InvalidOperationException("The ProjectName property is required and cannot be null, empty or whitespace.");

			var projectConfigName = ProjectConfigName.Get(context);
			if (string.IsNullOrWhiteSpace(projectConfigName))
				throw new InvalidOperationException("The ProjectConfigName property is required and cannot be null, empty or whitespace.");

			var buildDetail = context.GetExtension<IBuildDetail>();
			var buildDefinitionName = buildDetail.BuildDefinition.Name;
			var commandLog = new CodeActivityContextCommandLog(context);
			var lockTimeoutSeconds = LockTimeoutSeconds.Get(context);
			var requestedBy = buildDetail.RequestedBy;
			var teamProject = buildDetail.TeamProject;

			var command =
				new CreateVersionCommand
				{
					BuildDefinitionName = buildDefinitionName,
					CommandLog = commandLog,
					ConnectionString = connectionString,
					LockTimeoutSeconds = lockTimeoutSeconds,
					ProjectConfigName = projectConfigName,
					ProjectName = projectName,
					RequestedBy = requestedBy,
					TeamProjectName = teamProject
				};

			var versionDetails = command.Execute();

			// Execute optional features.
			SetBuildNumber(context, buildDetail, versionDetails);
			SetDropLocation(context, buildDetail, versionDetails, projectName);

			// Save any changes to the build detail instance.
			buildDetail.Save();

			// Map the IVersionDetails instance to an instance of BuildVersionDetails.
			var buildVersionDetails = BuildVersionDetails.FromIVersionDetails(versionDetails);

			// Return the result.
			return buildVersionDetails;
		}

		private void SetBuildNumber(ActivityContext context, IBuildDetail buildDetail, IVersionDetails versionDetails)
		{
			var shouldSetBuildNumber = ShouldSetBuildNumber.Get(context);

			if (false == shouldSetBuildNumber)
				return;

			var tfsBuildNumber =
				string.Format("{0}_{1}_{2}",
					buildDetail.BuildDefinition.Name,
					versionDetails.Version,
					buildDetail.StartTime.ToString("yyyyMMdd-HHmm")
					);

			buildDetail.BuildNumber = tfsBuildNumber;
		}

		private void SetDropLocation(ActivityContext context, IBuildDetail buildDetail, IVersionDetails versionDetails, string projectName)
		{
			var shouldSetDropLocation = ShouldSetDropLocation.Get(context);

			if (false == shouldSetDropLocation)
				return;

			var dropLocationProjectDirPath = Path.Combine(buildDetail.DropLocationRoot, projectName);

			var dropLocationDirName =
				string.Format("{0}_{1}_{2}",
				buildDetail.StartTime.ToString("yyyyMMdd-HHmm"),
				buildDetail.BuildDefinition.Name,
				versionDetails.Version
				);

			var dropLocation = Path.Combine(dropLocationProjectDirPath, dropLocationDirName);


			buildDetail.DropLocation = dropLocation;
		}
	}
}