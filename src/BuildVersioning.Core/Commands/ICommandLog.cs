namespace BuildVersioning.Commands
{
	/// <summary>
	/// Implemented by classes that provide build component logging.
	/// </summary>
	public interface ICommandLog
	{
		/// <summary>
		/// Logs the the specified error message.
		/// </summary>
		/// <param name="message">The message.</param>
		void Error(string message);

		/// <summary>
		/// Logs the the specified error message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="args">The message arguments.</param>
		void Error(string message, params object[] args);

		/// <summary>
		/// Logs the specified informational message.
		/// </summary>
		/// <param name="message">The message.</param>
		void Message(string message);

		/// <summary>
		/// Messages the specified informational message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="args">The message arguments.</param>
		void Message(string message, params object[] args);

		/// <summary>
		/// Logs the specified warning message.
		/// </summary>
		/// <param name="message">The message.</param>
		void Warning(string message);

		/// <summary>
		/// Logs the specified warning message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="args">The message arguments.</param>
		void Warning(string message, params object[] args);
	}
}