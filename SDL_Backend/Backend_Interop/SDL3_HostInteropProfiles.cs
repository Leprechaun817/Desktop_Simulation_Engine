using System;
using System.Runtime.InteropServices;

namespace Backend
{
	namespace Interop
	{
		public enum HostOperatingSystem
		{
			Windows,
			Linux,
			MacOS,
			FreeBSD,
			NetBSD,
			Browser,
			Unknown
		}

		public enum HostArchitecture
		{
			x86,
			x64_86,
			ARM,
			ARM64,
			ARMv6,
			WASM,
			S390x,
			PPC64LE,
			LoongArch64,
			Unknown
		}

		public enum HostEndianness
		{
			LE,
			BE
		}

		public readonly record struct HostInteropProfile(HostOperatingSystem operatingSystem, HostArchitecture processArchitecture, HostArchitecture operatingSystemArchitecture,
														 int pointerSize, HostEndianness endianness, bool is64BitProcess, bool is64BitOperatingSystem);

		public static class HostInteropProfiles
		{
			private static readonly object syncRoot = new object();
			private static HostInteropProfile? current;

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

			public static HostInteropProfile Initialize()
			{
				lock (syncRoot) 
				{
					if (current == null) {
						current = Detect();
					}

					return current.Value;
				}
			}

			public static HostInteropProfile Detect()
			{
				return new HostInteropProfile(DetectOperatingSystem(), MapArchitecture(RuntimeInformation.ProcessArchitecture), MapArchitecture(RuntimeInformation.OSArchitecture),
											  IntPtr.Size, BitConverter.IsLittleEndian ? HostEndianness.LE : HostEndianness.BE, Environment.Is64BitProcess,
											  Environment.Is64BitOperatingSystem);
			}

			public static string FormatDiagnostic(HostInteropProfile profile)
			{
				return string.Join("; ",
								   $"host.os = {profile.operatingSystem}",
								   $"host.process_arch = {profile.processArchitecture}",
								   $"host.os_arch = {profile.operatingSystemArchitecture}",
								   $"host.pointer_size = {profile.pointerSize}",
								   $"host.endianness = {profile.endianness}",
								   $"host.is_64_bit_process = {profile.is64BitProcess}",
								   $"host.is_64_bit_os = {profile.is64BitOperatingSystem}");
			}

			public static string FormatCurrentDiagnostic()
			{
				return FormatDiagnostic(current!.Value);
			}

			public static HostOperatingSystem DetectOperatingSystem()
			{
				if (OperatingSystem.IsWindows()) {
					return HostOperatingSystem.Windows;
				}
				else if (OperatingSystem.IsLinux()) {
					return HostOperatingSystem.Linux;
				}
				else if (OperatingSystem.IsMacOS()) {
					return HostOperatingSystem.MacOS;
				}
				else if (OperatingSystem.IsFreeBSD()) {
					return HostOperatingSystem.FreeBSD;
				}
				else if (RuntimeInformation.OSDescription.Contains("NetBSD", StringComparison.OrdinalIgnoreCase)) {
					return HostOperatingSystem.NetBSD;
				}
				else if (OperatingSystem.IsBrowser()) {
					return HostOperatingSystem.Browser;
				}
				else {
					return HostOperatingSystem.Unknown;
				}
			}

			public static HostArchitecture MapArchitecture(Architecture architecture)
			{
				switch (architecture.ToString()) {
					case "x86":
						return HostArchitecture.x86;
					case "x86_64":
						return HostArchitecture.x64_86;
					case "ARM":
						return HostArchitecture.ARM;
					case "ARM64":
						return HostArchitecture.ARM64;
					case "ARMv6":
						return HostArchitecture.ARMv6;
					case "WASM":
						return HostArchitecture.WASM;
					case "S390x":
						return HostArchitecture.S390x;
					case "PPC64LE":
						return HostArchitecture.PPC64LE;
					case "LoongArch64":
						return HostArchitecture.LoongArch64;
					default:
						return HostArchitecture.Unknown;
				}
			}
		}
	}
}
