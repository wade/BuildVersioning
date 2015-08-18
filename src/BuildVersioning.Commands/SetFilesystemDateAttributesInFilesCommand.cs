using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BuildVersioning.Commands
{
	/// <summary>
	/// Command that is used to set one or more filesystem date attributes in one or more files.
	/// </summary>
	/// <remarks>
	/// This command is useful for setting the filesystem date attributes of all build output files
	/// to the same build date-time value.
	/// <para>
	/// This command will search the directory path specified by the <see cref="DirectoryToSearch"/> property
	/// for one or more files identified by the <see cref="FileNamesToSearch"/> property and for each
	/// file found, it will set one or more filesystem date attributes including created date, last modified date
	/// and last accessed date.
	/// </para><para>
	/// The directory searched may be searched recursively by setting the command's <see cref="Recursive"/>
	/// property to a value of <c>true</c>.
	/// </para>
	/// </remarks>
	public class SetFilesystemDateAttributesInFilesCommand : ICommand<bool>
	{
		private const bool DefaultRecursivePropertyValue = true;
		private const bool DefaultWriteVerboseLogMessagesPropertyValue = true;

		private List<string> _fileNames;

		/// <summary>
		/// Initializes a new instance of the <see cref="SetFilesystemDateAttributesInFilesCommand"/> class.
		/// </summary>
		public SetFilesystemDateAttributesInFilesCommand()
		{
			// Set default property values.
			Recursive = DefaultRecursivePropertyValue;
			WriteVerboseLogMessages = DefaultWriteVerboseLogMessagesPropertyValue;
		}

		/// <summary>
		/// Executes the component.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the command executed successfully; otherwise, <c>false</c>
		/// </returns>
		public virtual bool Execute()
		{
			LogPropertyValues();

			if (string.IsNullOrWhiteSpace(DirectoryToSearch))
			{
				CommandLog.Error(
					"The DirectoryToSearch property value is null or empty. " +
					"The DirectoryToSearch property is required and must be set to the absolute path of a directory. " +
					"The files may be contained in the directory or in any subdirectory if the Recursive property has a 'true' value.");
				return false;
			}

			if (false == Directory.Exists(DirectoryToSearch))
			{
				CommandLog.Error(string.Format("The directory specified by the DirectoryToSearch property, '{0}', does not exist.", DirectoryToSearch));
				return false;
			}

			if (string.IsNullOrWhiteSpace(FileNamesToSearch))
			{
				CommandLog.Error(
					"The FileNamesToSearch property value is null or empty. " +
					"The FileNamesToSearch property is required and must be set to a semicolon-delimited " +
					"list of file names (without the path) which may contain wildcard characters (such as *).");
				return false;
			}

			// Initialize list of file names and/or patterns to search.
			_fileNames = InitializeFileNames();

			LogFileNamesToSearch();

			var files = FindFiles(DirectoryToSearch);

			LogFoundFiles(files);

			files.ForEach(SetAssemblyAttributesInFile);

			return true;
		}

		/// <summary>
		/// Gets or sets the accessed date-time value that will be set in each matched file's accessed date
		/// filesystem attribute when the <see cref="SetAccessedDate"/> property value is <c>true</c>.
		/// </summary>
		/// <value>
		/// The accessed date-time value that will be set in each matched file's accessed date filesystem
		/// attribute when the <see cref="SetAccessedDate"/> property value is <c>true</c>.
		/// </value>
		public DateTime AccessedDate { get; set; }

		/// <summary>
		/// Gets or sets the created date-time value that will be set in each matched file's accessed date
		/// filesystem attribute when the <see cref="SetCreatedDate"/> property value is <c>true</c>.
		/// </summary>
		/// <value>
		/// The created date-time value that will be set in each matched file's accessed date filesystem
		/// attribute when the <see cref="SetCreatedDate"/> property value is <c>true</c>.
		/// </value>
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// Gets or sets the modified date-time value that will be set in each matched file's accessed date
		/// filesystem attribute when the <see cref="SetModifiedDate"/> property value is <c>true</c>.
		/// </summary>
		/// <value>
		/// The modified date-time value that will be set in each matched file's accessed date
		/// filesystem attribute when the <see cref="SetModifiedDate"/> property value is <c>true</c>.
		/// </value>
		public DateTime ModifiedDate { get; set; }

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
		/// set to <c>true</c>, the directory path will be searched recursively for all file specs.
		/// If the <see cref="Recursive"/> property is set to <c>false</c>, only the top-level directory
		/// will be searched, but no subdirectories.
		/// </remarks>
		public string DirectoryToSearch { get; set; }

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
		public string FileNamesToSearch { get; set; }

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
		public ICommandLog CommandLog { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the searched directory path is recursive.
		/// </summary>
		/// <value>
		///   <c>true</c> if the searched directory path recursive; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>
		/// The default value is <c>true</c>.
		/// </remarks>
		/// <seealso cref="DirectoryToSearch"/>
		public bool Recursive { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether each matched file's accessed date filesystem attribute is set with the <see cref="AccessedDate"/> property value.
		/// </summary>
		/// <value>
		///   <c>true</c> if each matched file's accessed date filesystem attribute is set with the <see cref="AccessedDate"/> property value; otherwise, <c>false</c>.
		/// </value>
		public bool SetAccessedDate { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether each matched file's created date filesystem attribute is set with the <see cref="CreatedDate"/> property value.
		/// </summary>
		/// <value>
		///   <c>true</c> if each matched file's created date filesystem attribute is set with the <see cref="CreatedDate"/> property value; otherwise, <c>false</c>.
		/// </value>
		public bool SetCreatedDate { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether each matched file's modified date filesystem attribute is set with the <see cref="ModifiedDate"/> property value.
		/// </summary>
		/// <value>
		///   <c>true</c> if each matched file's modified date filesystem attribute is set with the <see cref="ModifiedDate"/> property value; otherwise, <c>false</c>.
		/// </value>
		public bool SetModifiedDate { get; set; }

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
		public bool WriteVerboseLogMessages { get; set; }

		private List<string> InitializeFileNames()
		{
			var items = FileNamesToSearch.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
			return new List<string>(items);
		}

		private List<string> FindFiles(string directory)
		{
			var searchOption = Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			var list = new List<string>();
			if (Directory.Exists(directory))
			{
				foreach (var fileName in _fileNames)
				{
					var files = Directory.GetFiles(directory, fileName, searchOption);
					list.AddRange(files);
				}
			}
			return list;
		}

		private void SetAssemblyAttributesInFile(string filename)
		{
			// NOTE: This method assumes C# assembly attribute syntax is used in the file.

			var fileInfo = new FileInfo(filename);
			ClearReadOnlyFileSystemAttribute(fileInfo);
			SetAccessedDateFileSystemAttribute(fileInfo);
			SetCreatedDateFileSystemAttribute(fileInfo);
			SetModifiedDateFileSystemAttribute(fileInfo);
		}

		private void ClearReadOnlyFileSystemAttribute(FileInfo info)
		{
			if (info.IsReadOnly)
			{
				info.IsReadOnly = false;
				if (WriteVerboseLogMessages)
				{
					CommandLog.Message("Removing read-only file attribute on file: {0}", info.FullName);
				}
			}
		}

		private void SetAccessedDateFileSystemAttribute(FileSystemInfo info)
		{
			if (SetAccessedDate)
			{
				info.LastAccessTime = AccessedDate;
				if (WriteVerboseLogMessages)
				{
					CommandLog.Message("Setting the last accessed date-time file attribute on file: {0}", info.FullName);
				}
			}
		}

		private void SetCreatedDateFileSystemAttribute(FileSystemInfo info)
		{
			if (SetCreatedDate)
			{
				info.CreationTime = CreatedDate;
				if (WriteVerboseLogMessages)
				{
					CommandLog.Message("Setting the created date-time file attribute on file: {0}", info.FullName);
				}
			}
		}

		private void SetModifiedDateFileSystemAttribute(FileSystemInfo info)
		{
			if (SetModifiedDate)
			{
				info.LastWriteTime = ModifiedDate;
				if (WriteVerboseLogMessages)
				{
					CommandLog.Message("Setting the last modified date-time file attribute on file: {0}", info.FullName);
				}
			}
		}

		private void LogPropertyValues()
		{
			if (false == WriteVerboseLogMessages) return;
			var sb = new StringBuilder(2048);
			sb.AppendLine("SetFilesystemDateAttributesInFiles property values:");
			sb.AppendFormat("    AccessedDate ..............: {0}\n", AccessedDate);
			sb.AppendFormat("    CreatedDate ...............: {0}\n", CreatedDate);
			sb.AppendFormat("    ModifiedDate ..............: {0}\n", ModifiedDate);
			sb.AppendFormat("    DirectoryToSearch .........: {0}\n", DirectoryToSearch);
			sb.AppendFormat("    FileNamesToSearch .........: {0}\n", FileNamesToSearch);
			sb.AppendFormat("    Recursive .................: {0}\n", Recursive);
			sb.AppendFormat("    SetAccessedDate ...........: {0}\n", SetAccessedDate);
			sb.AppendFormat("    SetCreatedDate ............: {0}\n", SetCreatedDate);
			sb.AppendFormat("    SetModifiedDate ...........: {0}\n", SetModifiedDate);
			sb.AppendFormat("    WriteVerboseLogMessages ...: {0}\n", WriteVerboseLogMessages);
			CommandLog.Message(sb.ToString());
		}

		private void LogFileNamesToSearch()
		{
			if (false == WriteVerboseLogMessages) return;
			const string header = "File names/specs to search:";
			var sb = new StringBuilder(header.Length + FileNamesToSearch.Length + (_fileNames.Count * 5));
			sb.AppendLine(header);
			foreach (var fileName in _fileNames)
			{
				sb.AppendFormat("    {0}\n", fileName);
			}
			CommandLog.Message(sb.ToString());
		}

		private void LogFoundFiles(IReadOnlyCollection<string> files)
		{
			if (false == WriteVerboseLogMessages) return;
			const string header = "The following files were found and will be their date filesystem attributes set:";
			var sb = new StringBuilder(header.Length + (files.Count * 400));
			sb.AppendLine(header);
			foreach (var file in files)
			{
				sb.AppendFormat("    {0}\n", file);
			}
			CommandLog.Message(sb.ToString());
		}
	}
}