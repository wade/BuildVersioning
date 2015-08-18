using System;

namespace BuildVersioningManager.Models.HomeModels
{
	public class RecentActivityModel
	{
		/// <summary>
		/// Gets or sets the project's most recent activity date-time value.
		/// </summary>
		/// <value>
		/// The project's most recent activity date-time value.
		/// </value>
		public DateTime Date { get; set; }

		/// <summary>
		/// Gets or sets the project's most recently active project configuration identifier.
		/// </summary>
		/// <value>
		/// The project's most recently active project configuration identifier.
		/// </value>
		public long ProjectConfigId { get; set; }

		/// <summary>
		/// Gets or sets the name of the project's most recently active project configuration.
		/// </summary>
		/// <value>
		/// The name of the project's most recently active project configuration.
		/// </value>
		public string ProjectConfigName { get; set; }

		/// <summary>
		/// Gets or sets the identifier of the project.
		/// </summary>
		/// <value>
		/// The identifier of the project.
		/// </value>
		public long ProjectId { get; set; }

		/// <summary>
		/// Gets or sets the name of the project.
		/// </summary>
		/// <value>
		/// The name of the project.
		/// </value>
		public string ProjectName { get; set; }

		/// <summary>
		/// Gets or sets the project's most recent version.
		/// </summary>
		/// <value>
		/// The project's most recent version.
		/// </value>
		public string Version { get; set; }
	}
}