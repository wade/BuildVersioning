using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace BuildVersioning.Commands
{
	[TestClass]
	public class SetAssemblyAttributeInFilesCommandForAssemblyConfigurationAttributeTests
	{
		private const string TestFileNameFormat = "AssemblyInfo{0}.cs";
		private const int NumberOfTestFiles = 3;

		private string _directoryToSearch;
		private List<string> _testFilenames;

		[TestInitialize]
		public void TestInitialize()
		{
			SetupTestFiles();
		}


		[TestMethod]
		public void Can_Set_AssemblyConfiguration_Attribute_In_Files()
		{
			// Arrange
			const string attributeName = "AssemblyConfiguration";
			const string attributeValue = "Debug";

			var command =
				new SetAssemblyAttributeInFilesCommand
				{
					AttributeName = attributeName,
					AttributeValue = attributeValue,
					CommandLog = new ConsoleCommandLog(),
					CreateAttributeIfNotExists = true,
					DirectoryToSearch = _directoryToSearch,
					FileNamesToSearch = "AssemblyInfo*.cs",
					Recursive = true,
					WriteVerboseLogMessages = true
				};

			// Act
			var succeeded = command.Execute();

			// Assert
			succeeded.ShouldBeTrue();

			foreach (var filename in _testFilenames)
			{
				File.Exists(filename).ShouldBeTrue();

				var text = File.ReadAllText(filename);

				text.ShouldContain("[assembly: AssemblyConfiguration(\"Debug\")]");
			}
		}


		private void SetupTestFiles()
		{
			_testFilenames = new List<string>();

			var baseDir = Path.GetDirectoryName(GetType().Assembly.Location);
			Debug.Assert(false == string.IsNullOrWhiteSpace(baseDir));
			var dir = Path.Combine(baseDir, "SetAssemblyAttributeInFilesCommand for AssemblyConfiguration attribute");

			if (Directory.Exists(dir))
				Directory.Delete(dir, true);

			Directory.CreateDirectory(dir);

			for (var i = 0; i < NumberOfTestFiles; ++i)
			{
				var filename = Path.Combine(dir, string.Format(TestFileNameFormat, i));
				using (var file = File.CreateText(filename))
				{
					file.WriteLine("using System.Reflection;");
					file.WriteLine("using System.Runtime.InteropServices;");
					file.WriteLine();
					file.WriteLine("[assembly: AssemblyTitle(\"TestFile{0}\")]", i);
					file.WriteLine("[assembly: AssemblyDescription(\"This is for a test.\")]");

					if (i == 1)
						file.WriteLine("[assembly: AssemblyConfiguration(\"\")]");

					if (i == 2)
						file.WriteLine("[assembly: AssemblyConfiguration(\"asdf-qwerty-fake-value-for-testing\")]");

					file.WriteLine("[assembly: ComVisible(false)]");
					file.WriteLine();
					file.Flush();
					file.Close();
				}

				if (i == 1)
				{
					// ReSharper disable once UseObjectOrCollectionInitializer
					var fileInfo = new FileInfo(filename);
					fileInfo.IsReadOnly = true;
				}

				_testFilenames.Add(filename);
			}

			_directoryToSearch = dir;
		}
	}
}