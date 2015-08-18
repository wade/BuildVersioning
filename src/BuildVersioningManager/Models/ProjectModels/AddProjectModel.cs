namespace BuildVersioningManager.Models.ProjectModels
{
	/// <summary>
	/// Used to add a new project.
	/// </summary>
	public class AddProjectModel
	{
		/// <summary>
		/// Gets or sets the optional project description.
		/// </summary>
		/// <value>
		/// The optional project description.
		/// </value>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the project name.
		/// </summary>
		/// <value>
		/// The project name.
		/// </value>
		public string Name { get; set; }
	}
}