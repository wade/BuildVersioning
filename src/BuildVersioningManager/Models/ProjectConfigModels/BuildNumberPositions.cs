namespace BuildVersioningManager.Models.ProjectConfigModels
{
	/// <summary>
	/// Defines the allowed build number position values.
	/// </summary>
	public enum BuildNumberPositions
	{
		/// <summary>
		/// The build number will be the third position in a four-part version.
		/// </summary>
		Three = 3,

		/// <summary>
		/// The build number will be the fourth position in a four-part version.
		/// </summary>
		Four = 4
	}
}