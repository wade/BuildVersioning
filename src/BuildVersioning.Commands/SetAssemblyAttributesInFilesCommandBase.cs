using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BuildVersioning.Commands
{
	/// <summary>
	/// Command that is used to set one or more assembly attributes
	/// in one or more C# source code files (e.g. AssemblyInfo.cs files).
	/// </summary>
	/// <remarks>
	/// This component will search the directory path specified by the <see cref="DirectoryToSearch"/> property
	/// for one or more files identified by the <see cref="FileNamesToSearch"/> property and for each
	/// file found, it will search the file for one or more of the assembly attributes specified using the
	/// <see cref="AssemblyAttributesToValuesDictionary"/> property.
	/// Each of the attributes found in a searched file will have its value changed if the
	/// corresponding property of this component, is set to a valid value (e.g. not null, empty or whitespace).
	/// <para>
	/// The component can be configured to create any missing attributes in each file by setting the
	/// <see cref="CreateAttributeIfNotExists"/> property value to <c>true</c>.
	/// </para><para>
	/// The directory searched may be searched recursively by setting the component's <see cref="Recursive"/>
	/// property to a value of <c>true</c>.
	/// </para><para>
	/// The component only works for assembly attributes defined in the C# language syntax.
	/// Additionally, the component only works for assembly attribute declarations that follow the most common
	/// declaration convention as follows:
	/// <example>
	///		[assembly: AssemblyVersion("1.0.0.0")]
	///		[assembly: AssemblyFileVersion("1.0.0.0")]
	///		[assembly: AssemblyInformationalVersion("1.0.0.0")]
	/// </example>
	/// It will not work if the assembly attributes are qualified with full or partial namespace of if the
	/// optional "Attribute" suffix is specified. It will work regardless of whitespace as long as the whitespace
	/// is allowed by the C# assembly attribute syntax.
	/// </para>
	/// </remarks>
	public abstract class SetAssemblyAttributesInFilesCommandBase : ICommand<bool>
	{
		#region " Attribute Regular Expression Format "

		// The regular expression below replaces the string value between the double quotes in
		// an assembly attribute, such as: [assembly: AssemblyVersion("1.0.0.0")]
		// In the example above, the 1.0.0.0 would be matched/replaced.
		//
		// The regular expression contains a single .NET string format specifier, {0}, which
		// is replaced by the name of the specific assembly attribute type name without the "Attribute" suffix,
		// such as AssemblyVersion, AssemblyFileVersion or AssemblyInformationalVersion.
		//
		// Formatted regular expressions created from the AssemblyAttributeRegexFormat constant:
		//		(?<=^\s*\[\s*assembly:\s*AssemblyVersion\s*\(\s*").*(?="\s*\)\s*])
		//		(?<=^\s*\[\s*assembly:\s*AssemblyFileVersion\s*\(\s*").*(?="\s*\)\s*])
		//		(?<=^\s*\[\s*assembly:\s*AssemblyInformationalVersion\s*\(\s*").*(?="\s*\)\s*])
		//
		// The regular expression handles white-space in between C# syntax elements.
		//
		private const string AssemblyAttributeRegexFormat = @"(?<=^\s*\[\s*assembly:\s*{0}\s*\(\s*"").*(?=""\s*\)\s*])";

		#endregion

		private const bool DefaultCreateAttributeIfNotExistsPropertyValue = true;
		private const string DefaultFilenamesToSearchPropertyValue = "AssemblyInfo.cs";
		private const bool DefaultRecursivePropertyValue = true;
		private const bool DefaultWriteVerboseLogMessagesPropertyValue = true;
		private const RegexOptions AssemblyAttributeRegexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline;

		private static readonly ConcurrentDictionary<string, Regex> AttributeRegexDictionary = new ConcurrentDictionary<string, Regex>();

		private readonly StringBuilder _errorStringBuilder;
		private List<string> _fileNames;

		/// <summary>
		/// Initializes a new instance of the <see cref="SetAssemblyAttributesInFilesCommandBase"/> class.
		/// </summary>
		protected SetAssemblyAttributesInFilesCommandBase()
		{
			// Set default property values
			AssemblyAttributesToValuesDictionary = new Dictionary<string, string>();
			CreateAttributeIfNotExists = DefaultCreateAttributeIfNotExistsPropertyValue;
			FileNamesToSearch = DefaultFilenamesToSearchPropertyValue;
			Recursive = DefaultRecursivePropertyValue;
			WriteVerboseLogMessages = DefaultWriteVerboseLogMessagesPropertyValue;

			// Initialize fields
			_errorStringBuilder = new StringBuilder(2048);
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

			if (null == AssemblyAttributesToValuesDictionary ||
				AssemblyAttributesToValuesDictionary.Count < 1)
			{
				CommandLog.Error(
					"The AssemblyAttributesToValuesDictionary property is null or empty. " +
					"The AssemblyAttributesToValuesDictionary property is required and must contain at least one item." +
					"Each item in the dictionary has a key that is the attribute name and the value is the attribute value to be set in matched files.");
				return false;
			}

			if (string.IsNullOrWhiteSpace(DirectoryToSearch))
			{
				CommandLog.Error(
					"The DirectoryToSearch property value is null or empty. " +
					"The DirectoryToSearch property is required and must be set to the absolute path of a " +
					"directory containing files with assembly attributes defined (e.g. AssemblyInfo.cs). " +
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
					"list of file names (without the path) to search for assembly attributes." +
					"If the FileNamesToSearch property is not set, the default value used is 'AssemblyInfo.cs'.");
				return false;
			}

			_fileNames = InitializeFileNames();

			LogFileNamesToSearch();

			var files = FindFiles(DirectoryToSearch);

			LogFoundFiles(files);

			files.ForEach(SetAssemblyAttributesInFile);

			if (_errorStringBuilder.Length > 0)
			{
				CommandLog.Error(string.Format("The following error(s) occurred:\n{0}", _errorStringBuilder));
				return false;
			}
			return true;
		}

		/// <summary>
		/// Gets or sets the assembly attributes to values dictionary.
		/// </summary>
		/// <value>
		/// The assembly attributes to values dictionary.
		/// </value>
		/// <remarks>
		/// For each item in the dictionary, the key is the assembly attribute name without the "Attribute" suffix
		/// and the value is the string value of the attribute that will be set.
		/// </remarks>
		protected Dictionary<string, string> AssemblyAttributesToValuesDictionary { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether attributes with assigned values should be created if they do not exist.
		/// </summary>
		/// <value>
		/// <c>true</c> if attributes with assigned values should be created if they do not exist; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>
		/// The default value is <c>true</c>.
		/// <para>
		/// When set to <c>true</c>, the specified assembly attributes will be created and added to matched files if they
		/// do not already exist and only if they have a non-null, non-empty, non-whitespace value.
		/// </para><para>
		/// If set to <c>false</c>, assembly attributes will be set when they already exist, but will not be added if they
		/// do not already exist in a matched file.
		/// </para>
		/// </remarks>
		public bool CreateAttributeIfNotExists { get; set; }

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

		private string SetAssemblyAttributeInText(string originalText, ref string text, string attributeName, string attributeValue, Regex regex)
		{
			// NOTE: This method assumes C# assembly attribute syntax is used in the file.

			bool appendedAttribute;

			if (CreateAttributeIfNotExists)
			{
				appendedAttribute = (false == HasAssemblyAttribute(regex, text));
				if (appendedAttribute)
				{
					text = AppendAssemblyAttributeToText(attributeName, attributeValue, text);
				}
			}
			else
			{
				appendedAttribute = false;
			}

			bool replacedAttribute;

			text = ReplaceAssemblyAttribute(regex, text, attributeValue, out replacedAttribute);

			replacedAttribute = (replacedAttribute && false == appendedAttribute);

			if (text != originalText && WriteVerboseLogMessages)
			{
				// Append the modification that were made to the file.
				var sb = new StringBuilder(200);
				if (appendedAttribute) sb.AppendFormat("    The {0} attribute was created with value '{1}'.\n", attributeName, attributeValue);
				if (replacedAttribute) sb.AppendFormat("    The existing {0} attribute was value replaced with value '{1}'.\n", attributeName, attributeValue);
				return sb.ToString();
			}

			// No modifications were made to the file, therefore return null.
			return null;
		}

		private void SetAssemblyAttributesInFile(string filename)
		{
			// NOTE: This method assumes C# assembly attribute syntax is used in the file.

			ClearReadOnlyFileSystemAttribute(filename);

			var text = ReadFile(filename);
			var originalText = string.Copy(text);
			var sb = new StringBuilder(1000 + filename.Length + (AssemblyAttributesToValuesDictionary.Count * 200));

			if (WriteVerboseLogMessages)
				sb.AppendFormat("The following modifications were made to file: {0}\n", filename);

			foreach (var item in AssemblyAttributesToValuesDictionary)
			{
				var attributeName = item.Key;
				var attributeValue = item.Value;
				var regex = GetAttributeRegex(attributeName);
				var modificationsText = SetAssemblyAttributeInText(originalText, ref text, attributeName, attributeValue, regex);

				if (WriteVerboseLogMessages)
					sb.Append(modificationsText);
			}

			if (text == originalText)
				return; // There were no modifications therefore there is no more work to do.

			// If this point is reached, the text was modified.
			// Overwrite the file with the updated text.
			WriteFile(filename, text);

			// Write the log messages, if enabled.
			if (WriteVerboseLogMessages)
				CommandLog.Message(sb.ToString());
		}

		private void ClearReadOnlyFileSystemAttribute(string filename)
		{
			var info = new FileInfo(filename);
			if (info.IsReadOnly)
			{
				info.IsReadOnly = false;
				if (WriteVerboseLogMessages)
				{
					CommandLog.Message("Removing read-only file attribute on file '{0}'.", filename);
				}
			}
		}

		private static string ReadFile(string filename)
		{
			using (var reader = new StreamReader(filename))
			{
				return reader.ReadToEnd();
			}
		}

		private static void WriteFile(string filename, string content)
		{
			using (var writer = new StreamWriter(filename))
			{
				writer.Write(content);
			}
		}

		private static bool HasAssemblyAttribute(Regex regex, string input)
		{
			return Matches(regex, input);
		}

		private static string ReplaceAssemblyAttribute(Regex regex, string input, string replacementValue, out bool isModified)
		{
			return Replace(regex, input, replacementValue, out isModified);
		}

		private static bool Matches(Regex regex, string input)
		{
			return regex.IsMatch(input);
		}

		private static string Replace(Regex regex, string input, string replace, out bool isModified)
		{
			var text =
				string.IsNullOrWhiteSpace(replace)
					? input
					: regex.Replace(input, replace);

			isModified = (text != input);
			return text;
		}

		private static string AppendAssemblyAttributeToText(string attributeName, string attributeValue, string text)
		{
			var sb = new StringBuilder(text, text.Length + 100);
			using (var writer = new StringWriter(sb))
			{
				writer.WriteLine("\n[assembly: {0}(\"{1}\")]", attributeName, attributeValue);
			}
			return sb.ToString();
		}

		private void LogPropertyValues()
		{
			if (false == WriteVerboseLogMessages) return;
			var sb = new StringBuilder(2048);
			sb.AppendLine("SetAssemblyAttributesInFiles property values:");
			sb.Append("    AssemblyAttributesToValuesDictionary ...:\n");

			foreach (var item in AssemblyAttributesToValuesDictionary)
			{
				var attributeName = item.Key;
				var attributeValue = item.Value;
				sb.AppendFormat("        {0}={1}\n", attributeName, attributeValue ?? "{null}");
			}

			sb.AppendFormat("    CreateAttributeIfNotExists .............: {0}\n", CreateAttributeIfNotExists);
			sb.AppendFormat("    DirectoryToSearch ......................: {0}\n", DirectoryToSearch);
			sb.AppendFormat("    FileNamesToSearch ......................: {0}\n", FileNamesToSearch);
			sb.AppendFormat("    Recursive ..............................: {0}\n", Recursive);
			sb.AppendFormat("    WriteVerboseLogMessages ................: {0}\n", WriteVerboseLogMessages);
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

		private void LogFoundFiles(List<string> files)
		{
			if (false == WriteVerboseLogMessages) return;
			const string header = "The following files were found and will be checked for the specified assembly attributes:";
			var sb = new StringBuilder(header.Length + (files.Count * 400));
			sb.AppendLine(header);
			foreach (var file in files)
			{
				sb.AppendFormat("    {0}\n", file);
			}
			CommandLog.Message(sb.ToString());
		}

		private static Regex GetAttributeRegex(string attributeName)
		{
			return
				AttributeRegexDictionary.GetOrAdd(attributeName, name =>
				{
					var pattern = string.Format(AssemblyAttributeRegexFormat, name);
					return new Regex(pattern, AssemblyAttributeRegexOptions);
				});
		}
	}
}