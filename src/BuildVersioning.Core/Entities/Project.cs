using System;
using System.Collections.Generic;

namespace BuildVersioning.Entities
{
	/// <summary>
	/// A software project to be versioned when built.
	/// </summary>
	public class Project : BaseEntity, IEquatable<Project>
	{
		/// <summary>
		/// Gets or sets the last generated build number.
		/// </summary>
		/// <value>
		/// The last generated build number.
		/// </value>
		public int BuildNumber { get; set; }

		/// <summary>
		/// Gets or sets the date and time that the build number was last updated.
		/// </summary>
		/// <value>
		/// The date and time that the build number was last updated.
		/// </value>
		public DateTime DateBuildNumberUpdated { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		/// <remarks>
		/// The description is optional.
		/// </remarks>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the project name.
		/// </summary>
		/// <value>
		/// The project name.
		/// </value>
		/// <remarks>
		/// The name is required.
		/// </remarks>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the project configurations for this project.
		/// </summary>
		/// <value>
		/// The project configurations for this project.
		/// </value>
		public virtual ICollection<ProjectConfig> ProjectConfigs { get; set; }

		#region " Equality Members "

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
		/// </returns>
		public bool Equals(Project other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Name, other.Name);
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
			return Equals((Project)obj);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
		/// </returns>
		public override int GetHashCode()
		{
			return (Name != null ? Name.GetHashCode() : 0);
		}

		/// <summary>
		/// Implements the operator ==.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>
		/// The result of the operator.
		/// </returns>
		public static bool operator ==(Project left, Project right)
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
		public static bool operator !=(Project left, Project right)
		{
			return !Equals(left, right);
		}

		/// <summary>
		/// <see cref="Project"/> equality comparer class.
		/// </summary>
		private sealed class NameEqualityComparer : IEqualityComparer<Project>
		{
			/// <summary>
			/// Determines whether the specified objects are equal.
			/// </summary>
			/// <param name="x">The first object of type <see cref="Project"/> to compare.</param>
			/// <param name="y">The second object of type <see cref="Project"/> to compare.</param>
			/// <returns>
			/// true if the specified objects are equal; otherwise, false.
			/// </returns>
			public bool Equals(Project x, Project y)
			{
				if (ReferenceEquals(x, y)) return true;
				if (ReferenceEquals(x, null)) return false;
				if (ReferenceEquals(y, null)) return false;
				if (x.GetType() != y.GetType()) return false;
				return string.Equals(x.Name, y.Name);
			}

			/// <summary>
			/// Returns a hash code for this instance.
			/// </summary>
			/// <param name="obj">The object.</param>
			/// <returns>
			/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
			/// </returns>
			public int GetHashCode(Project obj)
			{
				return (obj.Name != null ? obj.Name.GetHashCode() : 0);
			}
		}

		private static readonly IEqualityComparer<Project> _nameComparerInstance = new NameEqualityComparer();

		/// <summary>
		/// Gets the name comparer.
		/// </summary>
		/// <value>
		/// The name comparer.
		/// </value>
		public static IEqualityComparer<Project> NameComparer
		{
			get { return _nameComparerInstance; }
		}

		#endregion
	}
}