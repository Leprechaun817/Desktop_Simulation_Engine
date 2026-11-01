using System;
using Backend.Interop;
using SDL3.Unsafe;

namespace SDL3
{
	namespace Interop
	{
		[Flags]
		public enum SDLInteropFeatureFlags
		{
			None = 0,
			Vsnprintf = 1,
			ManufacturedValistBlocks = 2,
			NativeEllipsisHelpers = 4,
			SScanf = 8,
			VSScanf = 16,
			ASPrintf = 32,
			WCharPrintf = 64,
			NativePrintfCallBuilder = 128
		}

		public enum SDLValistABI
		{
			Unsupported,
			WindowsMSVC_x64_86
			WindowsMSVC_x86,
			LinuxSystemV_x64_86,
			Linux_i386Stack,
			BSDSystemV_x64_86,
			BSD_i386Stack
		}

		[Flags]
		public enum SDLSystemFeatureFlags
		{
			None = 0,
			WindowsMessageHook = 1,
			Win32Direct3D = 2,
			X11EventHook = 4,
			LinuxThreadPriority = 8,
			iOSAnimation = 16,
			AndroidSystem = 32,
			GDKSystem = 64
		}

		public readonly record struct SDLPrintfArgumentLayout(int ptrByteCount, int longByteCount);

		public readonly record struct SDLInteropProfile(SDLValistABI valistABI, string? sdllibPath, Version? sdlVersion, SDLInteropFeatureFlags supportedFeatures,
														SDLSystemFeatureFlags systemFeatures);

		public static class SDLInteropProfiles
		{
			private static readonly object syncRoot = new object();
			private static SDLInteropProfile? current;
			private static SDLV
		}
	}
}