namespace BuildVersioning
{
	/// <summary>
	/// Defines the release type used to properly tag semantic version strings.
	/// </summary>
	public enum ReleaseType
	{
		/// <summary>
		/// Used for most builds during development and testing.
		/// </summary>
		PreRelease = 0,

		/// <summary>
		/// Used during release stabilization when nearing the very end of the development and testing iterations.
		/// </summary>
		ReleaseCandidate,

		/// <summary>
		/// Used during official releases when converting a release candidate into an actual release.
		/// </summary>
		Release
	}
}