using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace BuildVersioning.Commands
{
	[TestClass]
	public class SetFilesystemDateAttributesInFilesCommandTests
	{
		private const string TestFileNameFormat = "TestFile{0}.txt";
		private const int NumberOfTestFiles = 5;

		private string _directoryToSearch;
		private List<string> _testFilenames;

		[TestInitialize]
		public void TestInitialize()
		{
			SetupTestFiles();
		}


		[TestMethod]
		public void Can_Set_All_Date_Filesystem_Attributes_In_Files()
		{
			// Arrange
			var dateTime = new DateTime(1973, 09, 04, 08, 14, 37);

			var command =
				new SetFilesystemDateAttributesInFilesCommand
				{
					AccessedDate = dateTime,
					CommandLog = new ConsoleCommandLog(),
					CreatedDate = dateTime,
					DirectoryToSearch = _directoryToSearch,
					FileNamesToSearch = "TestFile*.txt",
					ModifiedDate = dateTime,
					Recursive = true,
					SetAccessedDate = true,
					SetCreatedDate = true,
					SetModifiedDate = true,
					WriteVerboseLogMessages = true
				};

			// Act
			var succeeded = command.Execute();

			// Assert
			succeeded.ShouldBeTrue();

			foreach (var filename in _testFilenames)
			{
				File.Exists(filename).ShouldBeTrue();

				var fileInfo = new FileInfo(filename);

				fileInfo.LastAccessTime.ShouldEqual(dateTime);
				fileInfo.CreationTime.ShouldEqual(dateTime);
				fileInfo.LastWriteTime.ShouldEqual(dateTime);
			}
		}


		private void SetupTestFiles()
		{
			_testFilenames = new List<string>();

			var baseDir = Path.GetDirectoryName(GetType().Assembly.Location);
			Debug.Assert(false == string.IsNullOrWhiteSpace(baseDir));
			var dir = Path.Combine(baseDir, "SetFilesystemDateAttributesInFilesCommandTestFiles");

			if (Directory.Exists(dir))
				Directory.Delete(dir, true);

			Directory.CreateDirectory(dir);

			for (var i = 0; i < NumberOfTestFiles; ++i)
			{
				var filename = Path.Combine(dir, string.Format(TestFileNameFormat, i));
				using (var file = File.CreateText(filename))
				{
					file.WriteLine("This is test file number {0}.", i);
					file.Flush();
					file.Close();
				}
				_testFilenames.Add(filename);
			}

			_directoryToSearch = dir;
		}
	}
}