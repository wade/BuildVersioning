using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using BuildVersioning.Data;
using BuildVersioning.Entities;

namespace BuildVersioning.Commands
{
	/// <summary>
	/// Command that creates a new version for the specified <see cref="Project"/> and <see cref="ProjectConfig"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This command is thread-safe and may also be used concurrently across process boundaries, such as
	/// in multiple Team Build agents on the same or different build servers. Synchronization is handled by
	/// SQL Server using an application lock (sp_getapplock). This synchronization ensures that each version
	/// that is created by this command is unique and never repeated or duplicated.
	/// </para>
	/// </remarks>
	public class CreateVersionCommand : ICommand<VersionHistoryItem>
	{
		// **************************************************************************
		// NOTE: Read the _ReadMe.txt file in this project before making any changes.
		// **************************************************************************

		private const string SqlAppLockResourceName = "BuildVersioningForTfs_GetNextVersionCommand_Lock";
		private const string SqlAppLockMode = "Exclusive";
		private const string SqlAppLockOwner = "Transaction"; // Possible values: Transaction or Session
		private const int DefaultSqlAppLockTimeoutMilliseconds = 60000;

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <returns>
		/// A <see cref="VersionHistoryItem"/> instance that contains the details of the new version that was generated.
		/// </returns>
		/// <exception cref="System.InvalidOperationException">
		/// The ProjectName property cannot be null, empty or whitespace.
		/// or
		/// The ProjectName property cannot be null, empty or whitespace.
		/// </exception>
		public VersionHistoryItem Execute()
		{
			ValidateProperties();

			using (var connection = new SqlConnection(ConnectionString))
			{
				connection.Open();
				using (var transaction = connection.BeginTransaction())
				{
					try
					{
						// Get the application lock before doing ANY work - this is VERY important for proper synchronization.
						AcquireSqlApplicationLock(connection, transaction);

						var projectConfig = GetProjectConfig(connection, transaction);
						var project = GetProject(connection, transaction, projectConfig);

						IncrementBuildNumber(project);

						var versionHistoryItem = CreateAndPersistVersionHistoryItem(connection, transaction, project, projectConfig);

						UpdateProjectBuildNumber(connection, transaction, project, versionHistoryItem);

						transaction.Commit();
						CommandLog.Message("Generated build number {0}.", versionHistoryItem.BuildNumber);
						return versionHistoryItem;
					}
					catch
					{
						transaction.Rollback();
						throw;
					}
				}
			}
		}

		#region " Public Properties "

		/// <summary>
		/// Gets or sets the name of the build definition.
		/// </summary>
		/// <value>
		/// The name of the build definition.
		/// </value>
		public string BuildDefinitionName { get; set; }

		/// <summary>
		/// Gets or sets the SQL Server database connection string pointing to the BuildVersions database.
		/// </summary>
		/// <value>
		/// The SQL Server database connection string pointing to the BuildVersions database.
		/// </value>
		public string ConnectionString { get; set; }

		/// <summary>
		/// Gets or sets the command log.
		/// </summary>
		/// <value>
		/// The command log.
		/// </value>
		/// <remarks>
		/// The concrete command log allows specific log entries to be tracked depending upon
		/// the environment in which the command is executed. For example, when excuting within
		/// a Team Build custom action class, an adapter implementation may be used so that log
		/// entries are logged in the TFS build log.
		/// </remarks>
		public ICommandLog CommandLog { get; set; }

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
		public int LockTimeoutSeconds { get; set; }

		/// <summary>
		/// Gets or sets the name of the Build Versioning project under which a new version will be created.
		/// </summary>
		/// <value>
		/// The name of the Build Versioning project under which a new version will be created.
		/// </value>
		public string ProjectName { get; set; }

		/// <summary>
		/// Gets or sets the name of the Build Versioning project's 'project configuration' that defines the creation rules for the version.
		/// </summary>
		/// <value>
		/// The name of the Build Versioning project's 'project configuration' that defines the creation rules for the version.
		/// </value>
		public string ProjectConfigName { get; set; }

		/// <summary>
		/// Gets or sets the the name of the user account that requested the version to be created.
		/// </summary>
		/// <value>
		/// The name of the user account that requested the version to be created.
		/// </value>
		public string RequestedBy { get; set; }

		/// <summary>
		/// Gets or sets the name of the team project under which the new version request was issued.
		/// </summary>
		/// <value>
		/// The name of the team project under which the new version request was issued.
		/// </value>
		public string TeamProjectName { get; set; }

		#endregion

		#region " Implementation Methods "

		private void AcquireSqlApplicationLock(SqlConnection connection, SqlTransaction transaction)
		{
			var lockTimeoutMilliseconds =
				(LockTimeoutSeconds < 1)
					? DefaultSqlAppLockTimeoutMilliseconds
					: LockTimeoutSeconds * 1000;

			#region " SQL "

			var sql =
				string.Format(@"DECLARE @RC int;
EXEC @RC = sp_getapplock @Resource='{0}', @LockMode='{1}', @LockOwner='{2}', @LockTimeout='{3}';
SELECT @RC;",
					SqlAppLockResourceName,
					SqlAppLockMode,
					SqlAppLockOwner,
					lockTimeoutMilliseconds
					);

#endregion

			using (var command = new SqlCommand(sql, connection, transaction) {CommandType = CommandType.Text})
			{
				var obj = command.ExecuteScalar();
				var returnCode = (null != obj && obj != DBNull.Value) ? (int)obj : -979899;

				if (returnCode < 0)
					throw new Exception(string.Format("Failed to get the SQL application lock. The return code was {0}.", returnCode));
			}
		}

		private VersionHistoryItem CreateAndPersistVersionHistoryItem(SqlConnection connection, SqlTransaction transaction, Project project, ProjectConfig projectConfig)
		{
			var versionHistoryItem =
				new VersionHistoryItem
				{
					BuildDefinitionName = BuildDefinitionName,
					BuildNumber = project.BuildNumber,
					Date = project.DateBuildNumberUpdated,
					GeneratedBuildNumberPosition = projectConfig.GeneratedBuildNumberPosition,
					GeneratedVersionPart1 = projectConfig.GeneratedVersionPart1,
					GeneratedVersionPart2 = projectConfig.GeneratedVersionPart2,
					GeneratedVersionPart3 = projectConfig.GeneratedVersionPart3,
					GeneratedVersionPart4 = projectConfig.GeneratedVersionPart4,
					ProductVersionPart1 = projectConfig.ProductVersionPart1,
					ProductVersionPart2 = projectConfig.ProductVersionPart2,
					ProductVersionPart3 = projectConfig.ProductVersionPart3,
					ProductVersionPart4 = projectConfig.ProductVersionPart4,
					ProjectId = project.Id,
					ProjectName = project.Name,
					ProjectConfigId = projectConfig.Id,
					ProjectConfigName = projectConfig.Name,
					ReleaseType = projectConfig.ReleaseType,
					RequestedBy = RequestedBy,
					TeamProjectName = TeamProjectName
				};

			SetGeneratedVersionProperties(versionHistoryItem);

			#region " SQL "

			const string sql = @"INSERT INTO [dbo].[VersionHistory] (
            [ProjectId]
           ,[ProjectName]
           ,[ProjectConfigId]
           ,[ProjectConfigName]
           ,[Date]
           ,[BuildNumber]
           ,[Version]
           ,[SemanticVersion]
           ,[SemanticVersionSuffix]
           ,[ProductVersion]
           ,[ReleaseType]
           ,[BuildDefinitionName]
           ,[RequestedBy]
           ,[TeamProjectName]
           ,[GeneratedBuildNumberPosition]
           ,[GeneratedVersionPart1]
           ,[GeneratedVersionPart2]
           ,[GeneratedVersionPart3]
           ,[GeneratedVersionPart4]
           ,[ProductVersionPart1]
           ,[ProductVersionPart2]
           ,[ProductVersionPart3]
           ,[ProductVersionPart4]
           ) VALUES (
            @ProjectId
           ,@ProjectName
           ,@ProjectConfigId
           ,@ProjectConfigName
           ,@Date
           ,@BuildNumber
           ,@Version
           ,@SemanticVersion
           ,@SemanticVersionSuffix
           ,@ProductVersion
           ,@ReleaseType
           ,@BuildDefinitionName
           ,@RequestedBy
           ,@TeamProjectName
           ,@GeneratedBuildNumberPosition
           ,@GeneratedVersionPart1
           ,@GeneratedVersionPart2
           ,@GeneratedVersionPart3
           ,@GeneratedVersionPart4
           ,@ProductVersionPart1
           ,@ProductVersionPart2
           ,@ProductVersionPart3
           ,@ProductVersionPart4
           )";

			#endregion

			using (var command = new SqlCommand(sql, connection, transaction) {CommandType = CommandType.Text})
			{
				command.Parameters.Add(new SqlParameter("@ProjectId", SqlDbType.BigInt, 8, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.ProjectId));
				command.Parameters.Add(new SqlParameter("@ProjectName", SqlDbType.NVarChar, 100, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.ProjectName));
				command.Parameters.Add(new SqlParameter("@ProjectConfigId", SqlDbType.BigInt, 8, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.ProjectConfigId));
				command.Parameters.Add(new SqlParameter("@ProjectConfigName", SqlDbType.NVarChar, 100, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.ProjectConfigName));

				command.Parameters.Add(new SqlParameter("@Date", SqlDbType.DateTime, 8, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.Date));
				command.Parameters.Add(new SqlParameter("@BuildNumber", SqlDbType.Int, 4, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.BuildNumber));
				command.Parameters.Add(new SqlParameter("@Version", SqlDbType.NVarChar, 100, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.Version));
				command.Parameters.Add(new SqlParameter("@SemanticVersion", SqlDbType.NVarChar, 100, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.SemanticVersion));
				command.Parameters.Add(new SqlParameter("@SemanticVersionSuffix", SqlDbType.NVarChar, 100, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.SemanticVersionSuffix));
				command.Parameters.Add(new SqlParameter("@ProductVersion", SqlDbType.NVarChar, 100, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.ProductVersion));
				command.Parameters.Add(new SqlParameter("@ReleaseType", SqlDbType.NVarChar, 100, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.ReleaseTypeString));
				command.Parameters.Add(new SqlParameter("@BuildDefinitionName", SqlDbType.NVarChar, 100, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.BuildDefinitionName));
				command.Parameters.Add(new SqlParameter("@RequestedBy", SqlDbType.NVarChar, 100, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.RequestedBy));
				command.Parameters.Add(new SqlParameter("@TeamProjectName", SqlDbType.NVarChar, 100, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.TeamProjectName));
				command.Parameters.Add(new SqlParameter("@GeneratedBuildNumberPosition", SqlDbType.Int, 4, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.GeneratedBuildNumberPosition));

				command.Parameters.Add(new SqlParameter("@GeneratedVersionPart1", SqlDbType.Int, 4, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.GeneratedVersionPart1));
				command.Parameters.Add(new SqlParameter("@GeneratedVersionPart2", SqlDbType.Int, 4, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.GeneratedVersionPart2));
				command.Parameters.Add(new SqlParameter("@GeneratedVersionPart3", SqlDbType.Int, 4, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.GeneratedVersionPart3));
				command.Parameters.Add(new SqlParameter("@GeneratedVersionPart4", SqlDbType.Int, 4, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.GeneratedVersionPart4));

				command.Parameters.Add(new SqlParameter("@ProductVersionPart1", SqlDbType.Int, 4, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.ProductVersionPart1));
				command.Parameters.Add(new SqlParameter("@ProductVersionPart2", SqlDbType.Int, 4, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.ProductVersionPart2));
				command.Parameters.Add(new SqlParameter("@ProductVersionPart3", SqlDbType.Int, 4, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.ProductVersionPart3));
				command.Parameters.Add(new SqlParameter("@ProductVersionPart4", SqlDbType.Int, 4, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, versionHistoryItem.ProductVersionPart4));

				command.ExecuteNonQuery();
			}

			return versionHistoryItem;
		}

		private Project GetProject(SqlConnection connection, SqlTransaction transaction, ProjectConfig projectConfig)
		{
			#region " SQL "

			const string sql = @"SELECT [Id]
      ,[Name]
      ,[Description]
      ,[BuildNumber]
      ,[DateBuildNumberUpdated]
FROM [dbo].[Project]
WHERE [Name] = @Name;";

			#endregion

			using (var command = new SqlCommand(sql, connection, transaction) { CommandType = CommandType.Text })
			{
				command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 100, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, ProjectName));

				using (var reader = command.ExecuteReader())
				{
					var hasRow = reader.Read();

					if (false == hasRow)
						throw new InvalidOperationException(string.Format("The Project for the specified project name '{0}' does not exist.", ProjectConfigName));

					var project =
						new Project
						{
							Id = reader.ReadInt64("Id"),
							Name = reader.ReadString("Name"),
							Description = reader.ReadString("Description"),
							BuildNumber = reader.ReadInt32("BuildNumber"),
							DateBuildNumberUpdated = reader.GetDateTime(reader.GetOrdinal("DateBuildNumberUpdated"))
						};

					if (projectConfig.ProjectId != project.Id)
						throw new InvalidOperationException(string.Format("The project for the specified project name does exist but is not the parent of the ProjectConfig with the specified name '{0}'.", ProjectConfigName));

					projectConfig.Project = project;

					return project;
				}
			}
		}

		private ProjectConfig GetProjectConfig(SqlConnection connection, SqlTransaction transaction)
		{
			#region " SQL "

			const string sql = @"SELECT [Id]
      ,[ProjectId]
      ,[Name]
      ,[Description]
      ,[GeneratedBuildNumberPosition]
      ,[GeneratedVersionPart1]
      ,[GeneratedVersionPart2]
      ,[GeneratedVersionPart3]
      ,[GeneratedVersionPart4]
      ,[ProductVersionPart1]
      ,[ProductVersionPart2]
      ,[ProductVersionPart3]
      ,[ProductVersionPart4]
      ,[ReleaseType]
FROM [dbo].[ProjectConfig]
WHERE [Name] = @Name;";

			#endregion

			using (var command = new SqlCommand(sql, connection, transaction) {CommandType = CommandType.Text})
			{
				command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 100, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, ProjectConfigName));

				using (var reader = command.ExecuteReader())
				{
					var hasRow = reader.Read();

					if (false == hasRow)
						throw new InvalidOperationException(string.Format("The ProjectConfig for the specified ProjectConfig name '{0}' does not exist.", ProjectConfigName));

					return
						new ProjectConfig
						{
							Id = reader.ReadInt64("Id"),
							ProjectId = reader.ReadInt64("ProjectId"),
							Name = reader.ReadString("Name"),
							Description = reader.ReadString("Description"),
							GeneratedBuildNumberPosition = reader.ReadInt32("GeneratedBuildNumberPosition"),
							GeneratedVersionPart1 = reader.ReadInt32("GeneratedVersionPart1"),
							GeneratedVersionPart2 = reader.ReadInt32("GeneratedVersionPart2"),
							GeneratedVersionPart3 = reader.ReadInt32("GeneratedVersionPart3"),
							GeneratedVersionPart4 = reader.ReadInt32("GeneratedVersionPart4"),
							ProductVersionPart1 = reader.ReadInt32("ProductVersionPart1"),
							ProductVersionPart2 = reader.ReadInt32("ProductVersionPart2"),
							ProductVersionPart3 = reader.ReadInt32("ProductVersionPart3"),
							ProductVersionPart4 = reader.ReadInt32("ProductVersionPart4"),
							ReleaseTypeString = reader.ReadString("ReleaseType")
						};
				}
			}
		}

		private static string GetSemanticVersionSuffix(ReleaseType releaseType)
		{
			switch (releaseType)
			{
				case ReleaseType.PreRelease:
					return "-pre";

				case ReleaseType.ReleaseCandidate:
					return "-rc";

				case ReleaseType.Release:
					return string.Empty;

				default:
					throw new Exception(string.Format("The configured ReleaseType '{0}' is not supported.", releaseType));
			}
		}

		private static void IncrementBuildNumber(Project project)
		{
			project.BuildNumber++;
			project.DateBuildNumberUpdated = DateTime.Now;
		}

		private static void SetGeneratedVersionProperties(VersionHistoryItem versionHistoryItem)
		{
			var buildNumber = versionHistoryItem.BuildNumber;

			switch (versionHistoryItem.GeneratedBuildNumberPosition)
			{
				case 1:
					versionHistoryItem.GeneratedVersionPart1 = buildNumber;
					break;

				case 2:
					versionHistoryItem.GeneratedVersionPart2 = buildNumber;
					break;

				case 3:
					versionHistoryItem.GeneratedVersionPart3 = buildNumber;
					break;

				case 4:
					versionHistoryItem.GeneratedVersionPart4 = buildNumber;
					break;

				default:
					throw new Exception(string.Format("The GeneratedBuildNumberPosition value '{0}' is out of the allowed range for ProjectConfig {1}.", versionHistoryItem.GeneratedBuildNumberPosition, versionHistoryItem.ProjectConfigId));
			}

			versionHistoryItem.SemanticVersionSuffix = GetSemanticVersionSuffix(versionHistoryItem.ReleaseType);

			versionHistoryItem.Version =
				string.Format("{0}.{1}.{2}.{3}",
					versionHistoryItem.GeneratedVersionPart1,
					versionHistoryItem.GeneratedVersionPart2,
					versionHistoryItem.GeneratedVersionPart3,
					versionHistoryItem.GeneratedVersionPart4
					);

			versionHistoryItem.ProductVersion =
				string.Format("{0}.{1}.{2}.{3}",
					versionHistoryItem.ProductVersionPart1,
					versionHistoryItem.ProductVersionPart2,
					versionHistoryItem.ProductVersionPart3,
					versionHistoryItem.ProductVersionPart4
					);

			// Determine the semantic version string.
			var sb = new StringBuilder(100);

			switch (versionHistoryItem.GeneratedBuildNumberPosition)
			{
				case 1:
					sb.Append(versionHistoryItem.GeneratedVersionPart1);
					break;

				case 2:
					sb.AppendFormat("{0}.{1}", versionHistoryItem.GeneratedVersionPart1, versionHistoryItem.GeneratedVersionPart2);
					break;

				case 3:
					sb.AppendFormat("{0}.{1}.{2}", versionHistoryItem.GeneratedVersionPart1, versionHistoryItem.GeneratedVersionPart2, versionHistoryItem.GeneratedVersionPart3);
					break;

				//case 4:
				default:
					sb.Append(versionHistoryItem.Version);
					break;
			}

			// Append the semantic version suffix.
			sb.Append(versionHistoryItem.SemanticVersionSuffix);

			// Set the semantic version property.
			versionHistoryItem.SemanticVersion = sb.ToString();

		}

		private static void UpdateProjectBuildNumber(SqlConnection connection, SqlTransaction transaction, Project project, IVersionDetails versionDetails)
		{
			#region " SQL "

			const string sql = @"UPDATE [dbo].[Project]
SET [BuildNumber] = @BuildNumber,
    [DateBuildNumberUpdated] = @DateBuildNumberUpdated
WHERE [Id] = @Id;";

			#endregion

			var projectId = project.Id;
			var date = versionDetails.Date;
			var buildNumber = versionDetails.BuildNumber;

			using (var command = new SqlCommand(sql, connection, transaction) {CommandType = CommandType.Text})
			{
				command.Parameters.Add(new SqlParameter("@Id", SqlDbType.BigInt, 8, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, projectId));
				command.Parameters.Add(new SqlParameter("@BuildNumber", SqlDbType.Int, 4, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, buildNumber));
				command.Parameters.Add(new SqlParameter("@DateBuildNumberUpdated", SqlDbType.DateTime, 8, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, date));
				command.ExecuteNonQuery();
			}
		}

		private void ValidateProperties()
		{
			const string nullPropertyExceptionmessageFormat = "The {0} property is null, empty or contains only whitespace characters which is not allowed.";

			if (string.IsNullOrWhiteSpace(BuildDefinitionName))
				throw new InvalidOperationException(string.Format(nullPropertyExceptionmessageFormat, "BuildDefinitionName"));

			if (string.IsNullOrWhiteSpace(ConnectionString))
				throw new InvalidOperationException(string.Format(nullPropertyExceptionmessageFormat, "ConnectionString"));

			if (null == CommandLog)
				throw new InvalidOperationException("The Commandlog property is null which is not allowed.");

			if (string.IsNullOrWhiteSpace(ProjectName))
				throw new InvalidOperationException(string.Format(nullPropertyExceptionmessageFormat, "ProjectName"));

			if (string.IsNullOrWhiteSpace(ProjectConfigName))
				throw new InvalidOperationException(string.Format(nullPropertyExceptionmessageFormat, "ProjectConfigName"));

			if (string.IsNullOrWhiteSpace(RequestedBy))
				throw new InvalidOperationException(string.Format(nullPropertyExceptionmessageFormat, "RequestedBy"));

			if (string.IsNullOrWhiteSpace(TeamProjectName))
				throw new InvalidOperationException(string.Format(nullPropertyExceptionmessageFormat, "TeamProjectName"));
		}

		#endregion
	}
}