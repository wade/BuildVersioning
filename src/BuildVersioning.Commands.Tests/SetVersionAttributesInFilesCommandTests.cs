using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace BuildVersioning.Commands
{
	[TestClass]
	public class SetVersionAttributesInFilesCommandTests
	{
		private const string TestFileNameFormat = "AssemblyInfo{0}.cs";
		private const int NumberOfTestFiles = 4;

		private string _directoryToSearch;
		private List<string> _testFilenames;

		[TestInitialize]
		public void TestInitialize()
		{
			SetupTestFiles();
		}


		[TestMethod]
		public void Can_Set_All_Assembly_Version_Attributes_In_Files()
		{
			// Arrange
			const string productVersion = "1.2.0.0";
			const string version = "1.2.3.4";

			var command =
				new SetVersionAttributesInFilesCommand
				{
					AssemblyFileVersion = version,
					AssemblyInformationalVersion = productVersion,
					AssemblyVersion = version,
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

				text.ShouldContain("[assembly: AssemblyVersion(\"1.2.3.4\")]");
				text.ShouldContain("[assembly: AssemblyFileVersion(\"1.2.3.4\")]");
				text.ShouldContain("[assembly: AssemblyInformationalVersion(\"1.2.0.0\")]");
			}
		}


		private void SetupTestFiles()
		{
			_testFilenames = new List<string>();

			var baseDir = Path.GetDirectoryName(GetType().Assembly.Location);
			Debug.Assert(false == string.IsNullOrWhiteSpace(baseDir));
			var dir = Path.Combine(baseDir, "SetVersionAttributesInFilesCommandTestFiles");

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
					file.WriteLine("[assembly: ComVisible(false)]");
					file.WriteLine();

					if (i > 0)
						file.WriteLine("[assembly: AssemblyVersion(\"0.0.0.0\")]");

					if (i > 1)
						file.WriteLine("[assembly: AssemblyFileVersion(\"3.1\")]");

					if (i > 2)
						file.WriteLine("[assembly: AssemblyInformationalVersion(\"99.88.77.66\")]");

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