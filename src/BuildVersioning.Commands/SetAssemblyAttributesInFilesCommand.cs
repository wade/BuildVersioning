using System.Collections.Generic;

namespace BuildVersioning.Commands
{
	/// <summary>
	/// Command that is used to set one or more assembly attributes
	/// in one or more C# source code files (e.g. AssemblyInfo.cs files).
	/// </summary>
	/// <remarks>
	/// This component will search the directory path specified by the <see cref="SetAssemblyAttributesInFilesCommandBase.DirectoryToSearch"/> property
	/// for one or more files identified by the <see cref="SetAssemblyAttributesInFilesCommandBase.FileNamesToSearch"/> property and for each
	/// file found, it will search the file for one or more of the assembly attributes specified using the
	/// <see cref="SetAssemblyAttributesInFilesCommandBase.AssemblyAttributesToValuesDictionary"/> property.
	/// Each of the attributes found in a searched file will have its value changed if the
	/// corresponding property of this component, is set to a valid value (e.g. not null, empty or whitespace).
	/// <para>
	/// The component can be configured to create any missing attributes in each file by setting the
	/// <see cref="SetAssemblyAttributesInFilesCommandBase.CreateAttributeIfNotExists"/> property value to <c>true</c>.
	/// </para><para>
	/// The directory searched may be searched recursively by setting the component's <see cref="SetAssemblyAttributesInFilesCommandBase.Recursive"/>
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
	public class SetAssemblyAttributesInFilesCommand : SetAssemblyAttributesInFilesCommandBase
	{
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
		public Dictionary<string, string> AssemblyAttributesToValues
		{
			get { return AssemblyAttributesToValuesDictionary; }
			set { AssemblyAttributesToValuesDictionary = value; }
		}
	}
}