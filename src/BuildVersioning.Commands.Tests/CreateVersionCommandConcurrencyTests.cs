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
	public class CreateVersionCommandConcurrencyTests : BaseTest
	{
		private static readonly ManualResetEvent _manualResetEvent = new ManualResetEvent(false);
		private static readonly Random _random = new Random();

		private string _projectName;
		private string _projectConfigName;

		[TestInitialize]
		public void TestInitialize()
		{
			Initialize();
			_projectName = GetTestProject().Name;
			_projectConfigName = GetTestProjectConfig().Name;
		}

		[TestMethod]
		public void Can_Execute_100_Times_For_Each_Of_5_Worker_Threads_With_Random_Short_Thread_Sleep_For_Each_Iteration_In_Each_Worker_Thread()
		{
			_manualResetEvent.Reset();
			var threads =
				new[]
				{
					new Thread(WorkerThreadAction) { IsBackground = true },
					new Thread(WorkerThreadAction) { IsBackground = true },
					new Thread(WorkerThreadAction) { IsBackground = true },
					new Thread(WorkerThreadAction) { IsBackground = true },
					new Thread(WorkerThreadAction) { IsBackground = true }
				}.ToList();

			threads.ForEach(t => t.Start());

			_manualResetEvent.Set();

			threads.ForEach(t => t.Join(90000));

			_manualResetEvent.Reset();

			// Assert for the whole set.
			var expectedMaxBuildNumber = 100 * threads.Count;

			int maxBuildNumber;

			using (var db = new BuildVersioningDataContext(GetConfiguredConnectionString()))
			{
				maxBuildNumber = db.VersionHistoryItems.Max(vhi => vhi.BuildNumber);
			}

			// If there are any duplicates, the max expected build number could not be generated.
			maxBuildNumber.ShouldEqual(expectedMaxBuildNumber);

			// Assert for the project's current build number.
			var project = GetTestProject();
			project.BuildNumber.ShouldEqual(expectedMaxBuildNumber);
		}

		private void WorkerThreadAction()
		{
			try
			{
				// Using a set to store each generated build number instead of a list in order to detect duplicates.
				// If an attempt is made to store a duplicate build number in the HashSet instance, an exception will be thrown and the test will fail.
				var set = new HashSet<int>();

				_manualResetEvent.WaitOne(15000);

				var stopwatch = Stopwatch.StartNew();

				for (var i = 0; i < 100; ++i)
				{
					var millisecondsToSleep = _random.Next(0, 200);
					if (millisecondsToSleep > 0)
						Thread.Sleep(millisecondsToSleep);

					var buildNumber = Can_Execute_Test();
					set.Add(buildNumber);
				}

				stopwatch.Stop();

				Console.WriteLine("Thread {0} completed 100 iterations in {1} milliseconds.", Thread.CurrentThread.ManagedThreadId, stopwatch.ElapsedMilliseconds);

				//var list = set.ToList();
				//list.ForEach(bn => Console.WriteLine("Build Number: {0}", bn));
			}
			catch (Exception ex)
			{
				Console.WriteLine("Thread {0} blew up. Exception Type: {1}. Message: {2}", Thread.CurrentThread.ManagedThreadId, ex.GetType().FullName, ex.Message);
			}
		}

		private int Can_Execute_Test()
		{
			// Arrange
			var command = CreateNewCreateVersionCommand(projectName: _projectName, projectConfigName: _projectConfigName);

			// Act
			var versionHistoryItem = command.Execute();

			// Assert
			versionHistoryItem.ShouldNotBeNull();
			return versionHistoryItem.BuildNumber;
		}
	}
}
