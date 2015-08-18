using System.Collections.Generic;
using BuildVersioning.Entities;

namespace BuildVersioningManager.Models.ProjectConfigModels
{
	/// <summary>
	/// Used to display a list of project configs for the specified project.
	/// </summary>
	public class ProjectConfigsModel
	{
		/// <summary>
		/// Gets or sets the project.
		/// </summary>
		/// <value>
		/// The project.
		/// </value>
		public Project Project { get; set; }

		/// <summary>
		/// Gets or sets the project configs.
		/// </summary>
		/// <value>
		/// The project configs.
		/// </value>
		public List<ProjectConfig> ProjectConfigs { get; set; }
	}
}