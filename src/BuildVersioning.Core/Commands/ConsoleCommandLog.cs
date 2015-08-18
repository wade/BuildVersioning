using System;
using System.IO;

namespace BuildVersioning.Commands
{
	/// <summary>
	/// A implementation of the <see cref="ICommandLog"/> interface that writes log entries to the console.
	/// </summary>
	public class ConsoleCommandLog : ICommandLog
	{
		public void Error(string message)
		{
			using (var writer = new StreamWriter(Console.OpenStandardError()))
			{
				writer.WriteLine("Error: " + message);
			}
		}

		public void Error(string message, params object[] args)
		{
			var formattedMessage = string.Format(message, args);
			Error(formattedMessage);
		}

		public void Message(string message)
		{
			Console.WriteLine("Info: " + message);
		}

		public void Message(string message, params object[] args)
		{
			var formattedMessage = string.Format(message, args);
			Message(formattedMessage);
		}

		public void Warning(string message)
		{
			Console.WriteLine("Warning: " + message);
		}

		public void Warning(string message, params object[] args)
		{
			var formattedMessage = string.Format(message, args);
			Warning(formattedMessage);
		}
	}
}