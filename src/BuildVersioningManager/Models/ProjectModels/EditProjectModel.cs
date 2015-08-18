namespace BuildVersioningManager.Models.ProjectModels
{
	/// <summary>
	/// Used to edit a project.
	/// </summary>
	public class EditProjectModel
	{
		/// <summary>
		/// Gets or sets the current build number.
		/// </summary>
		/// <value>
		/// The current build number.
		/// </value>
		public int BuildNumber { get; set; }

		/// <summary>
		/// Gets or sets the optional project description.
		/// </summary>
		/// <value>
		/// The optional project description.
		/// </value>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the project identifier.
		/// </summary>
		/// <value>
		/// The project identifier.
		/// </value>
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the project name.
		/// </summary>
		/// <value>
		/// The project name.
		/// </value>
		public string Name { get; set; }
	}
}