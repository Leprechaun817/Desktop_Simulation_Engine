using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace SDL3_Backend_Tests
{
	namespace Memory
	{
		internal static class SDLAllocatorTestProcess
		{
			private const string ChildVariable = "DSE_SDL_ALLOCATOR_TEST_CHILD";
			public static bool IsChild
			{
				get
				{
					return Environment.GetEnvironmentVariable(ChildVariable) == "1";
				}
			}

			public static void Run(Type testClass, string methodName)
			{
				string resultsDir = Path.Combine(Path.GetTempPath(), "DSE_SDLAllocatorTests", Guid.NewGuid().ToString("N"));
				Directory.CreateDirectory(resultsDir);

				try {
					ProcessStartInfo startInfo = new ProcessStartInfo() {
						FileName = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet",
						WorkingDirectory = AppContext.BaseDirectory,
						UseShellExecute = false,
						CreateNoWindow = true,
						RedirectStandardOutput = true,
						RedirectStandardError = true
					};

					startInfo.ArgumentList.Add("vstest");
					startInfo.ArgumentList.Add(testClass.Assembly.Location);
					startInfo.ArgumentList.Add("/Platform:x64");
					startInfo.ArgumentList.Add("/TestCaseFilter:FullyQualifiedName=" + testClass.FullName + "." + methodName);
					startInfo.ArgumentList.Add("/ResultsDirectory:" + resultsDir);
					startInfo.ArgumentList.Add("/Logger:trx;LogFileName=allocator.trx");
					startInfo.Environment[ChildVariable] = "1";

					Task<string>? outputTask = default;
					Task<string>? errorTask = default;
					string? output = default;
					string? error = default;
					string? diagnostics = default;
					using (Process process = Process.Start(startInfo) ?? throw new InvalidOperationException("Unable to start the isolated SDL allocator test host")) 
					{
						outputTask = process.StandardOutput.ReadToEndAsync();
						errorTask = process.StandardError.ReadToEndAsync();

						if (!process.WaitForExit(30000)) {
							process.Kill(true);
							process.WaitForExit(5000);

							throw new TimeoutException("The isolated SDL allocator test exceeded its 30-second time limit.");
						}

						output = outputTask.GetAwaiter().GetResult();
						error = errorTask.GetAwaiter().GetResult();
						diagnostics = $"Exit code: {process.ExitCode}{Environment.NewLine}STDOUT:{Environment.NewLine}{output}{Environment.NewLine}STDERR:{Environment.NewLine}{error}";
						Assert.True(process.ExitCode == 0, diagnostics);
					}

					string resultsPath = Path.Combine(resultsDir, "allocator.trx");
					Assert.True(File.Exists(resultsPath), "The child host did not produce a test result." + Environment.NewLine + diagnostics);

					XElement result = Assert.Single(XDocument.Load(resultsPath).Descendants().Where(element =>
						element.Name.LocalName == "UnitTestResult"
					));
					Assert.Equal("Passed", (string?)result.Attribute("outcome"));
					Assert.Equal(testClass.FullName + "." + methodName, (string?)result.Attribute("testName"));
				}
				finally {
					//This directory is created solely for this child test run and as such once the test is completed
					//it is no longer needed. So we delete it here.
					Directory.Delete(resultsDir, true);
				}
			}
		}
	}
}