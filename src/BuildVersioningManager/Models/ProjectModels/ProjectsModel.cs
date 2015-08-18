using System.Collections.Generic;

namespace BuildVersioningManager.Models.ProjectModels
{
	/// <summary>
	/// Used to display the list of all projects.
	/// </summary>
	public class ProjectsModel
	{
		/// <summary>
		/// Gets or sets the projects.
		/// </summary>
		/// <value>
		/// The projects.
		/// </value>
		public List<BuildVersioning.Entities.Project> Projects { get; set; }
	}
}