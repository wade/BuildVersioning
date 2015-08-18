using System;
using System.Collections.Generic;

namespace BuildVersioning.Entities
{
	/// <summary>
	/// A configuration of a project.
	/// </summary>
	public class ProjectConfig : BaseEntity, IEquatable<ProjectConfig>
	{
		/// <summary>
		/// Gets or sets the project configuration description.
		/// </summary>
		/// <value>
		/// The project configuration description.
		/// </value>
		/// <remarks>
		/// The description is optional.
		/// </remarks>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the project configuration name.
		/// </summary>
		/// <value>
		/// The project configuration name.
		/// </value>
		/// <remarks>
		/// The name is required.
		/// </remarks>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the project that owns this project configuration.
		/// </summary>
		/// <value>
		/// The project that owns this project configuration.
		/// </value>
		public virtual Project Project { get; set; }

		/// <summary>
		/// Gets or sets the project identifier of the project that owns this project configuration.
		/// </summary>
		/// <value>
		/// The project identifier of the project that owns this project configuration.
		/// </value>
		public long ProjectId { get; set; }

		/// <summary>
		/// Gets or sets the generated build number position.
		/// </summary>
		/// <value>
		/// The generated build number position.
		/// </value>
		/// <remarks>
		/// The values allowed are 1, 2, 3 or 4 but 3 or 4 are the usual values.
		/// <para>
		/// A version has up to 4 parts. The build number is usually assigned to either part 3 or 4.
		/// </para>
		/// </remarks>
		public int GeneratedBuildNumberPosition { get; set; }

		/// <summary>
		/// Gets or sets part 1 of 4 of the generated version.
		/// </summary>
		/// <value>
		/// Part 1 of 4 of the generated version.
		/// </value>
		public int GeneratedVersionPart1 { get; set; }

		/// <summary>
		/// Gets or sets part 2 of 4 of the generated version.
		/// </summary>
		/// <value>
		/// Part 2 of 4 of the generated version.
		/// </value>
		public int GeneratedVersionPart2 { get; set; }

		/// <summary>
		/// Gets or sets part 3 of 4 of the generated version.
		/// </summary>
		/// <value>
		/// Part 3 of 4 of the generated version.
		/// </value>
		public int GeneratedVersionPart3 { get; set; }

		/// <summary>
		/// Gets or sets part 4 of 4 of the generated version.
		/// </summary>
		/// <value>
		/// Part 4 of 4 of the generated version.
		/// </value>
		public int GeneratedVersionPart4 { get; set; }

		/// <summary>
		/// Gets or sets part 1 of 4 of the product version.
		/// </summary>
		/// <value>
		/// Part 1 of 4 of the product version.
		/// </value>
		public int ProductVersionPart1 { get; set; }

		/// <summary>
		/// Gets or sets part 2 of 4 of the product version.
		/// </summary>
		/// <value>
		/// Part 2 of 4 of the product version.
		/// </value>
		public int ProductVersionPart2 { get; set; }

		/// <summary>
		/// Gets or sets part 3 of 4 of the product version.
		/// </summary>
		/// <value>
		/// Part 3 of 4 of the product version.
		/// </value>
		public int ProductVersionPart3 { get; set; }

		/// <summary>
		/// Gets or sets part 4 of 4 of the product version.
		/// </summary>
		/// <value>
		/// Part 4 of 4 of the product version.
		/// </value>
		public int ProductVersionPart4 { get; set; }

		/// <summary>
		/// Gets or sets the type of the release.
		/// </summary>
		/// <value>
		/// The type of the release.
		/// </value>
		/// <remarks>
		/// The configured <see cref="ReleaseType"/> is applied to all versions generated with the <see cref="ProjectConfig"/>.
		/// </remarks>
		public ReleaseType ReleaseType { get; set; }

		/// <summary>
		/// Gets or sets the release type as a string.
		/// </summary>
		/// <value>
		/// The release type as a string.
		/// </value>
		/// <remarks>
		/// The configured <see cref="ReleaseType"/> is applied to all versions generated with the <see cref="ProjectConfig"/>.
		/// </remarks>
		public string ReleaseTypeString
		{
			get { return ReleaseType.ToString(); }
			set
			{
				ReleaseType releaseType;
				if (false == Enum.TryParse(value, true, out releaseType))
					throw new ArgumentException("The value is not a valid ReleaseType string.");
				ReleaseType = releaseType;
			}
		}

		#region " Equality Members "

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
		/// </returns>
		public bool Equals(ProjectConfig other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Name, other.Name) && ProjectId == other.ProjectId;
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((ProjectConfig)obj);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
		/// </returns>
		public override int GetHashCode()
		{
			unchecked
			{
				return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ ProjectId.GetHashCode();
			}
		}

		/// <summary>
		/// Implements the operator ==.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator ==(ProjectConfig left, ProjectConfig right)
		{
			return Equals(left, right);
		}

		/// <summary>
		/// Implements the operator !=.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator !=(ProjectConfig left, ProjectConfig right)
		{
			return !Equals(left, right);
		}

		/// <summary>
		/// <see cref="ProjectConfig"/> equality comparer class.
		/// </summary>
		private sealed class NameProjectIdEqualityComparer : IEqualityComparer<ProjectConfig>
		{
			/// <summary>
			/// Determines whether the specified objects are equal.
			/// </summary>
			/// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
			/// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
			/// <returns>
			/// true if the specified objects are equal; otherwise, false.
			/// </returns>
			public bool Equals(ProjectConfig x, ProjectConfig y)
			{
				if (ReferenceEquals(x, y)) return true;
				if (ReferenceEquals(x, null)) return false;
				if (ReferenceEquals(y, null)) return false;
				if (x.GetType() != y.GetType()) return false;
				return string.Equals(x.Name, y.Name) && x.ProjectId == y.ProjectId;
			}

			/// <summary>
			/// Returns a hash code for this instance.
			/// </summary>
			/// <param name="obj">The object.</param>
			/// <returns>
			/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
			/// </returns>
			public int GetHashCode(ProjectConfig obj)
			{
				unchecked
				{
					return ((obj.Name != null ? obj.Name.GetHashCode() : 0) * 397) ^ obj.ProjectId.GetHashCode();
				}
			}
		}

		private static readonly IEqualityComparer<ProjectConfig> _nameProjectIdComparerInstance = new NameProjectIdEqualityComparer();

		/// <summary>
		/// Gets the name project identifier comparer.
		/// </summary>
		/// <value>
		/// The name project identifier comparer.
		/// </value>
		public static IEqualityComparer<ProjectConfig> NameProjectIdComparer
		{
			get { return _nameProjectIdComparerInstance; }
		}

		#endregion
	}
}