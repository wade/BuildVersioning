using System.Reflection;

namespace BuildVersioning.Commands
{
	/// <summary>
	/// Command that is used to set one or more version attributes
	/// in one or more C# source code files (e.g. AssemblyInfo.cs files).
	/// </summary>
	/// <remarks>
	/// This component will search the directory path specified by the <see cref="SetAssemblyAttributesInFilesCommandBase.DirectoryToSearch"/> property
	/// for one or more files identified by the <see cref="SetAssemblyAttributesInFilesCommandBase.FileNamesToSearch"/> property and for each
	/// file found, it will search the file for one or more of the following assembly version attributes:
	/// <list type="bullet">
	///		<item>
	///			<term>AssemblyVersion</term>
	///			<description>The <see cref="AssemblyVersionAttribute"/>.</description>
	///		</item>
	///		<item>
	///			<term>AssemblyFileVersion</term>
	///			<description>The <see cref="AssemblyFileVersionAttribute"/>.</description>
	///		</item>
	///		<item>
	///			<term>AssemblyInformationalVersion</term>
	///			<description>The <see cref="AssemblyInformationalVersionAttribute"/>.</description>
	///		</item>
	/// </list>
	/// Each of the attributes found in a searched file will have its version value changed if the
	/// corresponding property of this component, is set to a valid value (e.g. not null, empty or whitespace).
	/// <para>
	/// The component can be configured to create any missing attributes in each file setting the
	/// <see cref="SetAssemblyAttributesInFilesCommandBase.CreateAttributeIfNotExists"/> property value to <c>true</c>.
	/// </para><para>
	/// The directory searched may be searched recursively by setting the component's <see cref="SetAssemblyAttributesInFilesCommandBase.Recursive"/>
	/// property to a value of <c>true</c>.
	/// </para><para>
	/// The component's <see cref="AssemblyVersion"/> property sets the value of <see cref="AssemblyVersionAttribute"/>
	/// instances found in files or created, if applicable. If the <see cref="AssemblyVersion"/> property has a
	/// null, empty or whitespace value, the assembly version attribute instances will be left unchanged and will
	/// not be created.
	/// </para><para>
	/// The component's <see cref="AssemblyFileVersion"/> property sets the value of <see cref="AssemblyFileVersionAttribute"/>
	/// instances found in files or created, if applicable. If the <see cref="AssemblyFileVersion"/> property has a
	/// null, empty or whitespace value, the assembly version attribute instances will be left unchanged and will
	/// not be created.
	/// </para><para>
	/// The component's <see cref="AssemblyInformationalVersion"/> property sets the value of <see cref="AssemblyInformationalVersionAttribute"/>
	/// instances found in files or created, if applicable. If the <see cref="AssemblyInformationalVersion"/> property has a
	/// null, empty or whitespace value, the assembly version attribute instances will be left unchanged and will
	/// not be created.
	/// </para><para>
	/// It is possible to set each version attribute to a different value. Typically, <see cref="AssemblyVersion"/> and
	/// <see cref="AssemblyFileVersion"/> will be set to the same value while the <see cref="AssemblyInformationalVersion"/>,
	/// which represents the "Product Version" in Windows, is set to a more controlled, less-granular version.
	/// Each of the three version properties must be set independently even if they should use the same value.
	/// </para><para>
	/// The component only works for assembly version attributes defined in the C# language syntax.
	/// Additionally, the component only works for assembly attribute declarations that follow the most common
	/// declaration convention as follows:
	/// <example>
	///		[assembly: AssemblyVersion("1.0.0.0")]
	///		[assembly: AssemblyFileVersion("1.0.0.0")]
	///		[assembly: AssemblyInformationalVersion("1.0.0.0")]
	/// </example>
	/// It will not work if the version attributes are qualified with full or partial namespace of if the
	/// optional "Attribute" suffix is specified. It will work regardless of whitespace as long as the whitespace
	/// is allowed by the C# assembly attribute syntax.
	/// </para>
	/// </remarks>
	public class SetVersionAttributesInFilesCommand : SetAssemblyAttributesInFilesCommandBase
	{
		/// <summary>
		/// Executes the component.
		/// </summary>
		/// <returns>
		///   <c>true</c> if the command executed successfully; otherwise, <c>false</c>
		/// </returns>
		public override bool Execute()
		{
			AssemblyAttributesToValuesDictionary.Add("AssemblyFileVersion", AssemblyFileVersion);
			AssemblyAttributesToValuesDictionary.Add("AssemblyInformationalVersion", AssemblyInformationalVersion);
			AssemblyAttributesToValuesDictionary.Add("AssemblyVersion", AssemblyVersion);

			return base.Execute();
		}

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
		/// <seealso cref="AssemblyFileVersionAttribute"/>
		public string AssemblyFileVersion { get; set; }

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
		/// <seealso cref="AssemblyInformationalVersionAttribute"/>
		public string AssemblyInformationalVersion { get; set; }

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
		/// <seealso cref="AssemblyVersionAttribute"/>
		public string AssemblyVersion { get; set; }
	}
}