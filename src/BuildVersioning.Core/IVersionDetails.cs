using System;

namespace BuildVersioning
{
	/// <summary>
	/// Implemented by classes that provide details about a generated version.
	/// </summary>
	public interface IVersionDetails
	{
		/// <summary>
		/// Gets or sets the build number part of the version.
		/// </summary>
		/// <value>
		/// The build number part of the version.
		/// </value>
		int BuildNumber { get; }

		/// <summary>
		/// Gets or sets the date and time that the version was generated.
		/// </summary>
		/// <value>
		/// The date and time that the version was generated.
		/// </value>
		DateTime Date { get; }

		/// <summary>
		/// Gets or sets part 1 of 4 of the generated version.
		/// </summary>
		/// <value>
		/// Part 1 of 4 of the generated version.
		/// </value>
		int GeneratedVersionPart1 { get; }

		/// <summary>
		/// Gets or sets part 2 of 4 of the generated version.
		/// </summary>
		/// <value>
		/// Part 2 of 4 of the generated version.
		/// </value>
		int GeneratedVersionPart2 { get; }

		/// <summary>
		/// Gets or sets part 3 of 4 of the generated version.
		/// </summary>
		/// <value>
		/// Part 3 of 4 of the generated version.
		/// </value>
		int GeneratedVersionPart3 { get; }

		/// <summary>
		/// Gets or sets part 4 of 4 of the generated version.
		/// </summary>
		/// <value>
		/// Part 4 of 4 of the generated version.
		/// </value>
		int GeneratedVersionPart4 { get; }

		/// <summary>
		/// Gets or sets the product version as a string that is associated with the generated version.
		/// </summary>
		/// <value>
		/// The product version as a string that is associated with the generated version.
		/// </value>
		string ProductVersion { get; }

		/// <summary>
		/// Gets or sets part 1 of 4 of the product version.
		/// </summary>
		/// <value>
		/// Part 1 of 4 of the product version.
		/// </value>
		int ProductVersionPart1 { get; }

		/// <summary>
		/// Gets or sets part 2 of 4 of the product version.
		/// </summary>
		/// <value>
		/// Part 2 of 4 of the product version.
		/// </value>
		int ProductVersionPart2 { get; }

		/// <summary>
		/// Gets or sets part 3 of 4 of the product version.
		/// </summary>
		/// <value>
		/// Part 3 of 4 of the product version.
		/// </value>
		int ProductVersionPart3 { get; }

		/// <summary>
		/// Gets or sets part 4 of 4 of the product version.
		/// </summary>
		/// <value>
		/// Part 4 of 4 of the product version.
		/// </value>
		int ProductVersionPart4 { get; }

		/// <summary>
		/// Gets the type of the release.
		/// </summary>
		/// <value>
		/// The type of the release.
		/// </value>
		ReleaseType ReleaseType { get; }

		/// <summary>
		/// Gets the semantic version string.
		/// </summary>
		/// <value>
		/// The semantic version string.
		/// </value>
		string SemanticVersion { get; }

		/// <summary>
		/// Gets the semantic version suffix.
		/// </summary>
		/// <value>
		/// The semantic version suffix.
		/// </value>
		/// <example>-pre, -rc</example>
		string SemanticVersionSuffix { get; }

		/// <summary>
		/// Gets or sets the generated version as a string.
		/// </summary>
		/// <value>
		/// The generated version as a string.
		/// </value>
		string Version { get; }
	}
}