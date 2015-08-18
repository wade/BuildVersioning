using System;

namespace BuildVersioning.Entities
{
	public class VersionHistoryItem : BaseEntity, IVersionDetails
	{
		/// <summary>
		/// Gets or sets the name of the build definition that generated the version.
		/// </summary>
		/// <value>
		/// The name of the build definition that generated the version.
		/// </value>
		public string BuildDefinitionName { get; set; }

		/// <summary>
		/// Gets or sets the build number part of the version.
		/// </summary>
		/// <value>
		/// The build number part of the version.
		/// </value>
		public int BuildNumber { get; set; }

		/// <summary>
		/// Gets or sets the date and time that the version was generated.
		/// </summary>
		/// <value>
		/// The date and time that the version was generated.
		/// </value>
		public DateTime Date { get; set; }

		/// <summary>
		/// Gets or sets the generated build number position.
		/// </summary>
		/// <value>
		/// The generated build number position.
		/// </value>
		/// <remarks>
		/// The values allowed are 1, 2, 3 or 4 but 3 or 4 are the usual values.
		/// <para>
		/// A version has up to 4 parts. The build number is usually assigned to either part 3 or 4.
		/// </para>
		/// </remarks>
		public int GeneratedBuildNumberPosition { get; set; }

		/// <summary>
		/// Gets or sets part 1 of 4 of the generated version.
		/// </summary>
		/// <value>
		/// Part 1 of 4 of the generated version.
		/// </value>
		public int GeneratedVersionPart1 { get; set; }

		/// <summary>
		/// Gets or sets part 2 of 4 of the generated version.
		/// </summary>
		/// <value>
		/// Part 2 of 4 of the generated version.
		/// </value>
		public int GeneratedVersionPart2 { get; set; }

		/// <summary>
		/// Gets or sets part 3 of 4 of the generated version.
		/// </summary>
		/// <value>
		/// Part 3 of 4 of the generated version.
		/// </value>
		public int GeneratedVersionPart3 { get; set; }

		/// <summary>
		/// Gets or sets part 4 of 4 of the generated version.
		/// </summary>
		/// <value>
		/// Part 4 of 4 of the generated version.
		/// </value>
		public int GeneratedVersionPart4 { get; set; }

		/// <summary>
		/// Gets or sets the identifier of the project for which the version was generated.
		/// </summary>
		/// <value>
		/// The identifier of the project for which the version was generated.
		/// </value>
		public long ProjectId { get; set; }

		/// <summary>
		/// Gets or sets the name of the project for which the version was generated.
		/// </summary>
		/// <value>
		/// The name of the project for which the version was generated.
		/// </value>
		public string ProjectName { get; set; }

		/// <summary>
		/// Gets or sets the identifier of the project configuration for which the version was generated.
		/// </summary>
		/// <value>
		/// The identifier of the project configuration for which the version was generated.
		/// </value>
		public long ProjectConfigId { get; set; }

		/// <summary>
		/// Gets or sets the name of the project configuration for which the version was generated.
		/// </summary>
		/// <value>
		/// The name of the project configuration for which the version was generated.
		/// </value>
		public string ProjectConfigName { get; set; }

		/// <summary>
		/// Gets or sets the product version as a string that is associated with the generated version.
		/// </summary>
		/// <value>
		/// The product version as a string that is associated with the generated version.
		/// </value>
		public string ProductVersion { get; set; }

		/// <summary>
		/// Gets or sets part 1 of 4 of the product version.
		/// </summary>
		/// <value>
		/// Part 1 of 4 of the product version.
		/// </value>
		public int ProductVersionPart1 { get; set; }

		/// <summary>
		/// Gets or sets part 2 of 4 of the product version.
		/// </summary>
		/// <value>
		/// Part 2 of 4 of the product version.
		/// </value>
		public int ProductVersionPart2 { get; set; }

		/// <summary>
		/// Gets or sets part 3 of 4 of the product version.
		/// </summary>
		/// <value>
		/// Part 3 of 4 of the product version.
		/// </value>
		public int ProductVersionPart3 { get; set; }

		/// <summary>
		/// Gets or sets part 4 of 4 of the product version.
		/// </summary>
		/// <value>
		/// Part 4 of 4 of the product version.
		/// </value>
		public int ProductVersionPart4 { get; set; }

		/// <summary>
		/// Gets or sets the type of the release.
		/// </summary>
		/// <value>
		/// The type of the release.
		/// </value>
		public ReleaseType ReleaseType { get; set; }

		/// <summary>
		/// Gets or sets the release type as a string.
		/// </summary>
		/// <value>
		/// The release type as a string.
		/// </value>
		public string ReleaseTypeString
		{
			get { return ReleaseType.ToString(); }
			set
			{
				ReleaseType releaseType;
				if (false == Enum.TryParse(value, true, out releaseType))
					throw new ArgumentException("The value is not a valid ReleaseType string.");
				ReleaseType = releaseType;
			}
		}

		/// <summary>
		/// Gets the semantic version string.
		/// </summary>
		/// <value>
		/// The semantic version string.
		/// </value>
		public string SemanticVersion { get; set; }

		/// <summary>
		/// Gets the semantic version suffix.
		/// </summary>
		/// <value>
		/// The semantic version suffix.
		/// </value>
		/// <example>-pre, -rc</example>
		public string SemanticVersionSuffix { get; set; }

		/// <summary>
		/// Gets or sets the name of the user that requested the build that generated the version.
		/// </summary>
		/// <value>
		/// The name of the user that requested the build that generated the version.
		/// </value>
		public string RequestedBy { get; set; }

		/// <summary>
		/// Gets or sets the name of the Team Project that owns the build definition that generated the version.
		/// </summary>
		/// <value>
		/// The name of the Team Project that owns the build definition that generated the version.
		/// </value>
		public string TeamProjectName { get; set; }

		/// <summary>
		/// Gets or sets the generated version as a string.
		/// </summary>
		/// <value>
		/// The generated version as a string.
		/// </value>
		public string Version { get; set; }
	}
}