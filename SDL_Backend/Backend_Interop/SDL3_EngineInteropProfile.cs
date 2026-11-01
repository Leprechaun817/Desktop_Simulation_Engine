namespace Backend
{
	namespace Interop
	{
		public readonly record struct EngineInteropProfile(HostInteropProfile host, DotNetRuntimeProfile dotnet);

		public static class EngineInteropProfiles
		{
			private static readonly object syncRoot = new object();
			private static EngineInteropProfile? current;

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

			public static string FormatDiagnostic(EngineInteropProfile profile)
			{
				return string.Join("; ",
								   HostInteropProfiles.FormatDiagnostic(profile.host),
								   DotNetRuntimeProfiles.FormatDiagnostic(profile.dotnet));
			}

			public static string FormatCurrentDiagnostic()
			{
				return FormatDiagnostic(current!.Value);
			}

		}
	}
}