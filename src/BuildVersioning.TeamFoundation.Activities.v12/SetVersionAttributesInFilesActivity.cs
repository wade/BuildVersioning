using System.Activities;
using System.Reflection;
using BuildVersioning.Commands;
using Microsoft.TeamFoundation.Build.Client;

namespace BuildVersioning.TeamFoundation.Activities
{
	[BuildActivity(HostEnvironmentOption.Agent)]
	public sealed class SetVersionAttributesInFilesActivity : CodeActivity
	{
		/// <summary>
		/// Gets or sets the desired value of the assembly file version attribute.
		/// </summary>
		/// <value>
		/// The desired value of the assembly file version attribute.
		/// </value>
		/// <remarks>
		/// When set to a null, empty or whitespace value, the <see cref="AssemblyFileVersionAttribute"/>
		/// will not be searched or modified. When set to a version value, the expected format is a typical
		/// four-part version string, a.b.c.d.
		/// <para>The default value is null.</para>
		/// </remarks>
		public InArgument<string> AssemblyFileVersion { get; set; }

		/// <summary>
		/// Gets or sets the desired value of the assembly informational version attribute.
		/// </summary>
		/// <value>
		/// The desired value of the assembly informational version attribute.
		/// </value>
		/// <remarks>
		/// When set to a null, empty or whitespace value, the <see cref="AssemblyInformationalVersionAttribute"/>
		/// will not be searched or modified. When set to a version value, the expected format is a typical
		/// four-part version string, a.b.c.d.
		/// <para>The default value is null.</para>
		/// </remarks>
		public InArgument<string> AssemblyInformationalVersion { get; set; }

		/// <summary>
		/// Gets or sets the desired value of the assembly version attribute.
		/// </summary>
		/// <value>
		/// The desired value of the assembly version attribute.
		/// </value>
		/// <remarks>
		/// When set to a null, empty or whitespace value, the <see cref="AssemblyVersionAttribute"/>
		/// will not be searched or modified. When set to a version value, the expected format is a typical
		/// four-part version string, a.b.c.d.
		/// <para>The default value is null.</para>
		/// </remarks>
		public InArgument<string> AssemblyVersion { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether attributes with assigned values should be created if they do not exist.
		/// </summary>
		/// <value>
		/// <c>true</c> if attributes with assigned values should be created if they do not exist; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>
		/// The default value is <c>true</c>.
		/// <para>
		/// When set to <c>true</c>, the <see cref="AssemblyVersionAttribute"/>, <see cref="AssemblyFileVersionAttribute"/>
		/// and/or <see cref="AssemblyInformationalVersionAttribute"/> will be created and added to matched files if they
		/// do not already exist and only if they have a non-null, non-empty, non-whitespace value.
		/// </para><para>
		/// If set to <c>false</c>, version attributes will be set when they already exist, but will not be added if they
		/// do not already exist in a matched file.
		/// </para>
		/// </remarks>
		public InArgument<bool> CreateAttributeIfNotExists { get; set; }

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
			var assemblyFileVersion = AssemblyFileVersion.Get(context);
			var assemblyInformationalVersion = AssemblyInformationalVersion.Get(context);
			var assemblyVersion = AssemblyVersion.Get(context);
			var createAttributeIfNotExists = CreateAttributeIfNotExists.Get(context);
			var directoryToSearch = DirectoryToSearch.Get(context);
			var fileNamesToSearch = FileNamesToSearch.Get(context);
			var commandLog = new CodeActivityContextCommandLog(context);
			var recursive = Recursive.Get(context);
			var writeVerboseLogMessages = WriteVerboseLogMessages.Get(context);

			var command =
				new SetVersionAttributesInFilesCommand
				{
					AssemblyFileVersion = assemblyFileVersion,
					AssemblyInformationalVersion = assemblyInformationalVersion,
					AssemblyVersion = assemblyVersion,
					CreateAttributeIfNotExists = createAttributeIfNotExists,
					DirectoryToSearch = directoryToSearch,
					FileNamesToSearch = fileNamesToSearch,
					CommandLog = commandLog,
					Recursive = recursive,
					WriteVerboseLogMessages = writeVerboseLogMessages
				};

			var successful = command.Execute();

			if (false == successful)
				commandLog.Error("The SetVersionAttributesInFiles activity failed.");
		}
	}
}
