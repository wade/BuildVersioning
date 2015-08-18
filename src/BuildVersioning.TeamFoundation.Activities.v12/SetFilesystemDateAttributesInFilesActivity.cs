using System;
using System.Activities;
using BuildVersioning.Commands;
using Microsoft.TeamFoundation.Build.Client;

namespace BuildVersioning.TeamFoundation.Activities
{
	[BuildActivity(HostEnvironmentOption.Agent)]
	public sealed class SetFilesystemDateAttributesInFilesActivity : CodeActivity
	{
		/// <summary>
		/// Gets or sets the accessed date-time value that will be set in each matched file's accessed date
		/// filesystem attribute when the <see cref="SetAccessedDate"/> property value is <c>true</c>.
		/// </summary>
		/// <value>
		/// The accessed date-time value that will be set in each matched file's accessed date filesystem
		/// attribute when the <see cref="SetAccessedDate"/> property value is <c>true</c>.
		/// </value>
		public InArgument<DateTime> AccessedDate { get; set; }

		/// <summary>
		/// Gets or sets the created date-time value that will be set in each matched file's accessed date
		/// filesystem attribute when the <see cref="SetCreatedDate"/> property value is <c>true</c>.
		/// </summary>
		/// <value>
		/// The created date-time value that will be set in each matched file's accessed date filesystem
		/// attribute when the <see cref="SetCreatedDate"/> property value is <c>true</c>.
		/// </value>
		public InArgument<DateTime> CreatedDate { get; set; }

		/// <summary>
		/// Gets or sets the modified date-time value that will be set in each matched file's accessed date
		/// filesystem attribute when the <see cref="SetModifiedDate"/> property value is <c>true</c>.
		/// </summary>
		/// <value>
		/// The modified date-time value that will be set in each matched file's accessed date
		/// filesystem attribute when the <see cref="SetModifiedDate"/> property value is <c>true</c>.
		/// </value>
		public InArgument<DateTime> ModifiedDate { get; set; }

		/// <summary>
		/// Gets or sets the directory path to search.
		/// </summary>
		/// <value>
		/// The directory path to search.
		/// </value>
		/// <remarks>
		/// The directory path to search is required and must exist, otherwise the component will fail.
		/// The specified directory path will be searched for files macthing the file spec(s) specified
		/// in the <see cref="FileNamesToSearch"/> property. If the <see cref="Recursive"/> property is
		/// set to <c>true</c>, the direcotry path will be searched recursively for all file specs.
		/// If the <see cref="Recursive"/> property is set to <c>false</c>, only the top-level direcotry
		/// will be searched, but no subdirectories.
		/// </remarks>
		public InArgument<string> DirectoryToSearch { get; set; }

		/// <summary>
		/// Gets or sets the file names/specs to search.
		/// </summary>
		/// <value>
		/// The file names/specs to search.
		/// </value>
		/// <remarks>
		/// The value may contain one or more file names or file specs delimited by a semi-colon character.
		/// If only one value is supplied, the semi-colon is not required.
		/// <para>The default value is "AssemblyInfo.cs".</para>
		/// </remarks>
		public InArgument<string> FileNamesToSearch { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the searched directory path is recursive.
		/// </summary>
		/// <value>
		///   <c>true</c> if the searched directory path recursive; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>
		/// The default value is <c>true</c>.
		/// </remarks>
		public InArgument<bool> Recursive { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether each matched file's accessed date filesystem attribute is set with the <see cref="AccessedDate"/> property value.
		/// </summary>
		/// <value>
		///   <c>true</c> if each matched file's accessed date filesystem attribute is set with the <see cref="AccessedDate"/> property value; otherwise, <c>false</c>.
		/// </value>
		public InArgument<bool> SetAccessedDate { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether each matched file's created date filesystem attribute is set with the <see cref="CreatedDate"/> property value.
		/// </summary>
		/// <value>
		///   <c>true</c> if each matched file's created date filesystem attribute is set with the <see cref="CreatedDate"/> property value; otherwise, <c>false</c>.
		/// </value>
		public InArgument<bool> SetCreatedDate { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether each matched file's modified date filesystem attribute is set with the <see cref="ModifiedDate"/> property value.
		/// </summary>
		/// <value>
		///   <c>true</c> if each matched file's modified date filesystem attribute is set with the <see cref="ModifiedDate"/> property value; otherwise, <c>false</c>.
		/// </value>
		public InArgument<bool> SetModifiedDate { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to write verbose informational log messages.
		/// </summary>
		/// <value>
		/// <c>true</c> if verbose informational log messages should be written; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>
		/// The default value is <c>true</c>. When <c>false</c>, no informational log
		/// messages will be written, but error messages will be written.
		/// </remarks>
		public InArgument<bool> WriteVerboseLogMessages { get; set; }

		/// <summary>
		/// When implemented in a derived class, performs the execution of the activity.
		/// </summary>
		/// <param name="context">The execution context under which the activity executes.</param>
		protected override void Execute(CodeActivityContext context)
		{
			var accessedDate = AccessedDate.Get(context);
			var createdDate = CreatedDate.Get(context);
			var modifiedDate = ModifiedDate.Get(context);
			var directoryToSearch = DirectoryToSearch.Get(context);
			var fileNamesToSearch = FileNamesToSearch.Get(context);
			var commandLog = new CodeActivityContextCommandLog(context);
			var recursive = Recursive.Get(context);
			var setAccessedDate = SetAccessedDate.Get(context);
			var setCreatedDate = SetCreatedDate.Get(context);
			var setModifiedDate = SetModifiedDate.Get(context);
			var writeVerboseLogMessages = WriteVerboseLogMessages.Get(context);

			var command =
				new SetFilesystemDateAttributesInFilesCommand
				{
					AccessedDate = accessedDate,
					CreatedDate = createdDate,
					ModifiedDate = modifiedDate,
					DirectoryToSearch = directoryToSearch,
					FileNamesToSearch = fileNamesToSearch,
					CommandLog = commandLog,
					Recursive = recursive,
					SetAccessedDate = setAccessedDate,
					SetCreatedDate = setCreatedDate,
					SetModifiedDate = setModifiedDate,
					WriteVerboseLogMessages = writeVerboseLogMessages
				};

			var successful = command.Execute();

			if (false == successful)
				commandLog.Error("The SetFilesystemDateAttributesInFiles activity failed.");
		}
	}
}
