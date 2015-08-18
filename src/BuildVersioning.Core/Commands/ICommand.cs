namespace BuildVersioning.Commands
{
	/// <summary>
	/// Implemented by classes that implement the Build Versioning command pattern.
	/// </summary>
	/// <typeparam name="TReturn">The type of the return instance or value.</typeparam>
	public interface ICommand<out TReturn>
	{
		/// <summary>
		/// Executes the command instance.
		/// </summary>
		/// <returns>
		/// A <typeparamref name="TReturn"/> instance or value.
		/// </returns>
		TReturn Execute();

		/// <summary>
		/// Gets or sets the command log.
		/// </summary>
		/// <value>
		/// The command log.
		/// </value>
		/// <remarks>
		/// The concrete command log allows specific log entries to be tracked depending upon
		/// the environment in which the command is executed. For example, when excuting within
		/// a Team Build custom action class, an adapter implementation may be used so that log
		/// entries are logged in the TFS build log.
		/// </remarks>
		ICommandLog CommandLog { get; set; }
	}
}