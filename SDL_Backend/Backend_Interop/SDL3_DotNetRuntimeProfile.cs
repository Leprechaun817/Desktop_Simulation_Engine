using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Backend
{
	namespace Interop
	{
		public readonly record struct DotNetRuntimeProfile(string frameworkDescription, string runtimeIdentifier, Version runtimeVersion, bool isDynamicCodeSupported,
														   bool isDynamicCodeCompiled);

		public static class DotNetRuntimeProfiles
		{
			private static readonly object syncRoot = new object();
			private static DotNetRuntimeProfile? current;

			public static bool IsInitialized
			{
				get
				{
					lock (syncRoot)
					{
						return current.HasValue;
					}
				}
			}

			public static DotNetRuntimeProfile Initialize()
			{
				lock (syncRoot) 
				{
					if (current == null) {
						current = Detect();
					}

					return current.Value;
				}
			}

			public static DotNetRuntimeProfile Detect()
			{
				return new DotNetRuntimeProfile(RuntimeInformation.FrameworkDescription, RuntimeInformation.RuntimeIdentifier, Environment.Version, RuntimeFeature.IsDynamicCodeSupported,
												RuntimeFeature.IsDynamicCodeCompiled);
			}

			public static string FormatDiagnostic(DotNetRuntimeProfile profile)
			{
				return string.Join("; ",
								   $"dotnet.framework = {profile.frameworkDescription}",
								   $"dotnet.rid = {profile.runtimeIdentifier}",
								   $"dotnet.version = {profile.runtimeVersion}",
								   $"dotnet.dynamic_code_supported = {profile.isDynamicCodeSupported}",
								   $"dotnet.dynamic_code_compiled = {profile.isDynamicCodeCompiled}");
			}

			public static string FormatCurrentDiagnostic()
			{
				return FormatDiagnostic(current!.Value);
			}
		}
	}
}