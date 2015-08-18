using System;

namespace BuildVersioning
{
	/// <summary>
	/// Information about a generated version.
	/// </summary>
	public class VersionDetails : IVersionDetails
	{
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
		/// Gets the type of the release.
		/// </summary>
		/// <value>
		/// The type of the release.
		/// </value>
		public ReleaseType ReleaseType { get; set; }

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
		/// Gets or sets the generated version as a string.
		/// </summary>
		/// <value>
		/// The generated version as a string.
		/// </value>
		public string Version { get; set; }

		/// <summary>
		/// Creates a new instance with values mapped from an <see cref="IVersionDetails"/> instance.
		/// </summary>
		/// <param name="versionDetails">The <see cref="IVersionDetails"/> instance to be mapped into a new <see cref="VersionDetails"/> instance.</param>
		/// <returns>
		/// A new instance of <see cref="VersionDetails"/> with values mapped from the specified instance of <see cref="IVersionDetails"/>.
		/// </returns>
		public static VersionDetails FromIVersionDetails(IVersionDetails versionDetails)
		{
			return
				new VersionDetails
				{
					BuildNumber = versionDetails.BuildNumber,
					Date = versionDetails.Date,
					GeneratedVersionPart1 = versionDetails.GeneratedVersionPart1,
					GeneratedVersionPart2 = versionDetails.GeneratedVersionPart2,
					GeneratedVersionPart3 = versionDetails.GeneratedVersionPart3,
					GeneratedVersionPart4 = versionDetails.GeneratedVersionPart4,
					ProductVersion = versionDetails.ProductVersion,
					ProductVersionPart1 = versionDetails.ProductVersionPart1,
					ProductVersionPart2 = versionDetails.ProductVersionPart2,
					ProductVersionPart3 = versionDetails.ProductVersionPart3,
					ProductVersionPart4 = versionDetails.ProductVersionPart4,
					ReleaseType = versionDetails.ReleaseType,
					SemanticVersion = versionDetails.SemanticVersion,
					SemanticVersionSuffix = versionDetails.SemanticVersionSuffix,
					Version = versionDetails.Version
				};
		}
	}
}