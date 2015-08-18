namespace BuildVersioning.Commands
{
	/// <summary>
	/// A null implementation of the <see cref="ICommandLog"/> interface.
	/// </summary>
	/// <remarks>
	/// This implementation does not actually perform any logging.
	/// It is used when no other implementation of <see cref="ICommandLog"/>
	/// is passed as a dependency to one of the core build components.
	/// </remarks>
	public class NullCommandLog : ICommandLog
	{
		public void Error(string message)
		{
			// Do nothing here.
		}

		public void Error(string message, params object[] args)
		{
			// Do nothing here.
		}

		public void Message(string message)
		{
			// Do nothing here.
		}

		public void Message(string message, params object[] args)
		{
			// Do nothing here.
		}

		public void Warning(string message)
		{
			// Do nothing here.
		}

		public void Warning(string message, params object[] args)
		{
			// Do nothing here.
		}
	}
}