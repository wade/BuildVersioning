using System;

namespace BuildVersioning.Commands
{
	/// <summary>
	/// Command that is used to set one assembly attribute
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
	public class SetAssemblyAttributeInFilesCommand : SetAssemblyAttributesInFilesCommandBase
	{
		/// <summary>
		/// Executes the component.
		/// </summary>
		/// <returns>
		///   <c>true</c> if the command executed successfully; otherwise, <c>false</c>
		/// </returns>
		public override bool Execute()
		{
			if (string.IsNullOrWhiteSpace(AttributeName))
				throw new InvalidOperationException("The AttributeName property is null, empty or contains only whitespace, which is not allowed. It must be set to the name of a valid .NET attribute type name without the \"Attribute\" suffix (e.g. AssemblyConfiguration).");

			AssemblyAttributesToValuesDictionary.Add(AttributeName, AttributeValue);

			return base.Execute();
		}

		/// <summary>
		/// Gets or sets the name of the assembly attribute to be set.
		/// </summary>
		/// <value>
		/// The name of the assembly attribute to be set.
		/// </value>
		/// <remarks>
		/// The attribute should be formatted as the assembly attribute's type name
		/// without a namespace declaration and without the "Attribute" suffix.
		/// </remarks>
		public string AttributeName { get; set; }

		/// <summary>
		/// Gets or sets the value that will be set for the attribute with specified <see cref="AttributeName"/>.
		/// </summary>
		/// <value>
		/// The value that will be set for the attribute with specified <see cref="AttributeName"/>.
		/// </value>
		public string AttributeValue { get; set; }
	}
}