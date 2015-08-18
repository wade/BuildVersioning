using System;
using System.Activities;
using BuildVersioning.Commands;
using Microsoft.TeamFoundation.Build.Workflow.Activities;

namespace BuildVersioning.TeamFoundation.Activities
{
	/// <summary>
	/// Implements the <see cref="ICommandLog"/> interface by using the logging/tracking features of the TeamBuild <see cref="CodeActivityContext"/> class.
	/// </summary>
	public class CodeActivityContextCommandLog : ICommandLog
	{
		private CodeActivityContext _context;

		/// <summary>
		/// Initializes a new instance of the <see cref="CodeActivityContextCommandLog"/> class.
		/// </summary>
		public CodeActivityContextCommandLog()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CodeActivityContextCommandLog"/> class.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <exception cref="System.ArgumentNullException">context</exception>
		public CodeActivityContextCommandLog(CodeActivityContext context)
		{
			if (null == context)
			{
				throw new ArgumentNullException("context");
			}
			_context = context;
		}

		/// <summary>
		/// Sets the context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <exception cref="System.ArgumentNullException">context</exception>
		internal void SetContext(CodeActivityContext context)
		{
			if (null == context)
			{
				throw new ArgumentNullException("context");
			}
			if (null == _context)
			{
				_context = context;
			}
		}

		/// <summary>
		/// Logs the the specified error message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Error(string message)
		{
			_context.TrackBuildError(message);
		}

		/// <summary>
		/// Logs the the specified error message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="args">The message arguments.</param>
		public void Error(string message, params object[] args)
		{
			Error(string.Format(message, args));
		}

		/// <summary>
		/// Logs the specified informational message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Message(string message)
		{
			_context.TrackBuildMessage(message);
		}

		/// <summary>
		/// Messages the specified informational message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="args">The message arguments.</param>
		public void Message(string message, params object[] args)
		{
			Message(string.Format(message, args));
		}

		/// <summary>
		/// Logs the specified warning message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Warning(string message)
		{
			_context.TrackBuildWarning(message);
		}

		/// <summary>
		/// Logs the specified warning message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="args">The message arguments.</param>
		public void Warning(string message, params object[] args)
		{
			Warning(string.Format(message, args));
		}
	}
}