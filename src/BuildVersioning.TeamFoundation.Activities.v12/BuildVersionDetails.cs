namespace BuildVersioning.TeamFoundation.Activities
{
	/// <summary>
	/// Information about a generated version.
	/// </summary>
	public class BuildVersionDetails : VersionDetails
	{
		/// <summary>
		/// Creates a new instance with values mapped from an <see cref="IVersionDetails"/> instance.
		/// </summary>
		/// <param name="versionDetails">The <see cref="IVersionDetails"/> instance to be mapped into a new <see cref="BuildVersionDetails"/> instance.</param>
		/// <returns>
		/// A new instance of <see cref="BuildVersionDetails"/> with values mapped from the specified instance of <see cref="IVersionDetails"/>.
		/// </returns>
		public new static BuildVersionDetails FromIVersionDetails(IVersionDetails versionDetails)
		{
			return
				new BuildVersionDetails
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