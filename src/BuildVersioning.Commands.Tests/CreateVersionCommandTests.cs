using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using BuildVersioning.Data.Providers.EFProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace BuildVersioning.Commands
{
	[TestClass]
	public class CreateVersionCommandTests : BaseTest
	{
		[TestInitialize]
		public void TestInitialize()
		{
			Initialize();
		}

		[TestMethod]
		public void No_Op_Test()
		{
			// Do nothing.
			// This is used to run a one-off test to create and/or migrate the database using EF.
		}

		[TestMethod]
		public void Can_Execute()
		{
			var stopwatch = Stopwatch.StartNew();

			var buildNumber = Can_Execute_Test();

			stopwatch.Stop();

			Console.WriteLine("Completed 1 call in {0} milliseconds.", stopwatch.ElapsedMilliseconds);
			Console.WriteLine("Current Build Number: {0}", buildNumber);
		}

		[TestMethod]
		public void Can_Execute_100_Times()
		{
			Can_Execute_Multiple_Times(100);
		}

		[TestMethod]
		public void Can_Execute_1000_Times()
		{
			Can_Execute_Multiple_Times(1000);
		}

		//[TestMethod]
		//public void Can_Execute_10000_Times()
		//{
		//	Can_Execute_Multiple_Times(10000);
		//}

		[TestMethod]
		public void Can_Execute_Multiple_Times(int iterations)
		{
			var expectedMaxBuildNumber = iterations;

			// Using a set to store each generated build number instead of a list in order to detect duplicates.
			// If an attempt is made to store a duplicate build number in the HashSet instance, an exception will be thrown and the test will fail.
			var set = new HashSet<int>();

			var stopwatch = Stopwatch.StartNew();

			for (var i = 0; i < iterations; ++i)
			{
				var buildNumber = Can_Execute_Test();
				set.Add(buildNumber);
				Thread.Sleep(1); // <-- Used to smooth out race conditions regarding the Date property whereby fast machines can get the date out of order.
			}

			stopwatch.Stop();

			Console.WriteLine("Completed 100 iterations in {0} milliseconds.", stopwatch.ElapsedMilliseconds);

			var list = set.ToList();
			list.ForEach(bn => Console.WriteLine("Build Number: {0}", bn));

			// Assert for the whole set.
			int maxBuildNumber;

			using (var db = new BuildVersioningDataContext(GetConfiguredConnectionString()))
			{
				maxBuildNumber = db.VersionHistoryItems.Max(vhi => vhi.BuildNumber);
			}

			// If there are any duplicates, the max expected build number could not be generated.
			maxBuildNumber.ShouldEqual(expectedMaxBuildNumber);
		}

		private int Can_Execute_Test()
		{
			// Arrange
			var project = GetTestProject();
			var projectConfig = GetTestProjectConfig();

			var lastBuildNumberDate = project.DateBuildNumberUpdated;
			var startingBuildNumber = project.BuildNumber;
			var expectedBuildNumber = startingBuildNumber + 1;
			var expectedVersion = string.Format("1.0.{0}.0", expectedBuildNumber);
			var expectedSemanticVersion = string.Format("1.0.{0}-pre", expectedBuildNumber);
			const string expectedProductVersion = "1.0.0.0";
			const string connectionString = "Server=(local);Database=BuildVersions;Trusted_Connection=True;";

			var command = CreateNewCreateVersionCommand(connectionString: connectionString, projectName: project.Name, projectConfigName: projectConfig.Name);

			// Act
			var versionHistoryItem = command.Execute();

			// Assert
			versionHistoryItem.ShouldNotBeNull();
			versionHistoryItem.BuildNumber.ShouldEqual(expectedBuildNumber);
			versionHistoryItem.Date.ShouldBeGreaterThan(lastBuildNumberDate);
			versionHistoryItem.GeneratedBuildNumberPosition.ShouldEqual(projectConfig.GeneratedBuildNumberPosition);
			versionHistoryItem.ProductVersion.ShouldEqual(expectedProductVersion);
			versionHistoryItem.Version.ShouldEqual(expectedVersion);
			versionHistoryItem.SemanticVersion.ShouldEqual(expectedSemanticVersion);

			return versionHistoryItem.BuildNumber;
		}
	}
}
