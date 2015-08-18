namespace BuildVersioning.Entities
{
	/// <summary>
	/// Inherited by all entity classes.
	/// </summary>
	public abstract class BaseEntity
	{
		/// <summary>
		/// Gets or sets the identifier of the entity.
		/// </summary>
		/// <value>
		/// The identifier of the entity.
		/// </value>
		public long Id { get; set; }
	}
}