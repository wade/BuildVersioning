using BuildVersioning;
using BuildVersioning.Entities;

namespace BuildVersioningManager.Models.ProjectConfigModels
{
	/// <summary>
	/// Used to add a new project config.
	/// </summary>
	public class AddProjectConfigModel
	{
		/// <summary>
		/// Gets or sets the build number position.
		/// </summary>
		/// <value>
		/// The build number position.
		/// </value>
		public BuildNumberPositions BuildNumberPosition { get; set; }

		/// <summary>
		/// Gets or sets the optional project config description.
		/// </summary>
		/// <value>
		/// The optional project config description.
		/// </value>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the generated version part 1, usually the major version number.
		/// </summary>
		/// <value>
		/// The generated version part 1, usually the major version number.
		/// </value>
		public int GeneratedVersionPart1 { get; set; }

		/// <summary>
		/// Gets or sets the generated version part 2, usually the minor version number.
		/// </summary>
		/// <value>
		/// The generated version part 2, usually the minor version number.
		/// </value>
		public int GeneratedVersionPart2 { get; set; }

		/// <summary>
		/// Gets or sets the generated version part 3, usually the build number or revision number part of the version number.
		/// </summary>
		/// <value>
		/// The generated version part 3, usually the build number or revision number part of the version number.
		/// </value>
		public int GeneratedVersionPart3 { get; set; }

		/// <summary>
		/// Gets or sets the generated version part 4, usually the build number or revision number part of the version number.
		/// </summary>
		/// <value>
		/// The generated version part 4, usually the build number or revision number part of the version number.
		/// </value>
		public int GeneratedVersionPart4 { get; set; }

		/// <summary>
		/// Gets or sets the project config name.
		/// </summary>
		/// <value>
		/// The project config name.
		/// </value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the parent project identifier.
		/// </summary>
		/// <value>
		/// The parent project identifier.
		/// </value>
		public long ParentProjectId { get; set; }

		/// <summary>
		/// Gets or sets the name of the parent project.
		/// </summary>
		/// <value>
		/// The name of the parent project.
		/// </value>
		public string ParentProjectName { get; set; }

		/// <summary>
		/// Gets or sets the generated version part 1, usually the major version number.
		/// </summary>
		/// <value>
		/// The generated version part 1, usually the major version number.
		/// </value>
		public int ProductVersionPart1 { get; set; }

		/// <summary>
		/// Gets or sets the generated version part 2, usually the minor version number.
		/// </summary>
		/// <value>
		/// The generated version part 2, usually the minor version number.
		/// </value>
		public int ProductVersionPart2 { get; set; }

		/// <summary>
		/// Gets or sets the generated version part 3, usually the build number or revision number part of the version number.
		/// </summary>
		/// <value>
		/// The generated version part 3, usually the build number or revision number part of the version number.
		/// </value>
		public int ProductVersionPart3 { get; set; }

		/// <summary>
		/// Gets or sets the generated version part 4, usually the build number or revision number part of the version number.
		/// </summary>
		/// <value>
		/// The generated version part 4, usually the build number or revision number part of the version number.
		/// </value>
		public int ProductVersionPart4 { get; set; }

		/// <summary>
		/// Gets or sets the type of the release.
		/// </summary>
		/// <value>
		/// The type of the release.
		/// </value>
		public ReleaseType ReleaseType { get; set; }
	}
}