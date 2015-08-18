using System;
using System.Web.Mvc;

namespace BuildVersioningManager
{
	public class ErrorInfo : HandleErrorInfo
	{
		public ErrorInfo(string controllerName, string actionName, string message)
			: base(new Exception(), controllerName, actionName)
		{
			ErrorMessage = message;
		}

		public ErrorInfo(string controllerName, string actionName, string message, params object[] args)
			: this(controllerName, actionName, FormatMessage(message, args))
		{
		}

		public ErrorInfo(Exception exception, string controllerName, string actionName)
			: base(exception, controllerName, actionName)
		{
			ErrorMessage = exception.Message;
		}

		public string ErrorMessage { get; set; }

		private static string FormatMessage(string message, object[] args)
		{
			return (null == args || args.Length < 1) ? message : string.Format(message, args);

		}
	}
}