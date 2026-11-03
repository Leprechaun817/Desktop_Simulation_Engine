using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace SDL3
{
	namespace Native
	{
		internal enum SDLNativePrintfArgumentType
		{
			SignedInteger = 1,
			UnsignedInteger = 2,
			FloatingPoint = 3,
			UTF8String = 4,
			WideString = 5,
			Pointer = 6
		}

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal struct SDLNativePrintfArgument
		{
			internal int type;
			internal int nativeByteCount;
			internal long signedValue;
			internal ulong unsignedValue;
			internal double floatingValue;
			internal size_t pointerValue;
		}
	
		/// <summary>
		/// Raw function interop imports for the SDL native ellipsis sidecar
		/// 
		/// These functions are fixed-arity wrappers around native SDL_sscanf calls. The
		/// public validates the format string and output pointer list before the interop
		/// layer here is reached.
		/// </summary>
		internal static unsafe partial class SDL3_NativeEllipsisHelper
		{
			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_set_sdl_path", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SetSDLPath(string path);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_get_last_error")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int GetLastError(bptr buffer, size_t bufferSize);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_snprintf", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int Snprintf(bptr text, size_t maxlen, string fmt, SDLNativePrintfArgument* args, size_t argCount);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_asprintf", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int Asprintf(bdptr text, string fmt, SDLNativePrintfArgument* args, size_t argCount);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_swprintf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int Swprintf(vptr text, size_t maxlen, vptr fmt, SDLNativePrintfArgument* arg, size_t argCount, int nativeWideCharByteCount);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_set_error", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SetError(string fmt, SDLNativePrintfArgument* args, size_t argCount);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_log_message", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int LogMessage(int category, int priority, string fmt, SDLNativePrintfArgument* args, size_t argCount);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_ioprintf", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial size_t IOprintf(vptr context, string fmt, SDLNativePrintfArgument* args, size_t argCount);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_render_debug_text_format", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int RenderDebugTextFormat(SDLRenderer* renderer, float x, float y, string fmt, SDLNativePrintfArgument* args, size_t argCount);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_sscanf_0", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int Sscanf0(string text, string format);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_sscanf_1", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int Sscanf1(string text, string format, vptr a0);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_sscanf_2", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int Sscanf2(string text, string format, vptr a0, vptr a1);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_sscanf_3", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int Sscanf3(string text, string format, vptr a0, vptr a1, vptr a2);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_sscanf_4", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int Sscanf4(string text, string format, vptr a0, vptr a1, vptr a2, vptr a3);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_sscanf_5", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int Sscanf5(string text, string format, vptr a0, vptr a1, vptr a2, vptr a3, vptr a4);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_sscanf_6", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int Sscanf6(string text, string format, vptr a0, vptr a1, vptr a2, vptr a3, vptr a4, vptr a5);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_sscanf_7", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int Sscanf7(string text, string format, vptr a0, vptr a1, vptr a2, vptr a3, vptr a4, vptr a5, vptr a6);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_sscanf_8", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int Sscanf8(string text, string format, vptr a0, vptr a1, vptr a2, vptr a3, vptr a4, vptr a5, vptr a6, vptr a7);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_sscanf_9", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int Sscanf9(string text, string format, vptr a0, vptr a1, vptr a2, vptr a3, vptr a4, vptr a5, vptr a6, vptr a7, vptr a8);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_sscanf_10", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int Sscanf10(string text, string format, vptr a0, vptr a1, vptr a2, vptr a3, vptr a4, vptr a5, vptr a6, vptr a7, vptr a8, vptr a9);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_sscanf_11", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int Sscanf11(string text, string format, vptr a0, vptr a1, vptr a2, vptr a3, vptr a4, vptr a5, vptr a6, vptr a7, vptr a8, vptr a9, vptr a10);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_sscanf_12", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int Sscanf12(string text, string format, vptr a0, vptr a1, vptr a2, vptr a3, vptr a4, vptr a5, vptr a6, vptr a7, vptr a8, vptr a9, vptr a10, vptr a11);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_sscanf_13", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int Sscanf13(string text, string format, vptr a0, vptr a1, vptr a2, vptr a3, vptr a4, vptr a5, vptr a6, vptr a7, vptr a8, vptr a9, vptr a10, vptr a11,
												 vptr a12);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_sscanf_14", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int Sscanf14(string text, string format, vptr a0, vptr a1, vptr a2, vptr a3, vptr a4, vptr a5, vptr a6, vptr a7, vptr a8, vptr a9, vptr a10, vptr a11,
												 vptr a12, vptr a13);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_sscanf_15", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int Sscanf15(string text, string format, vptr a0, vptr a1, vptr a2, vptr a3, vptr a4, vptr a5, vptr a6, vptr a7, vptr a8, vptr a9, vptr a10, vptr a11,
												 vptr a12, vptr a13, vptr a14);

			[LibraryImport(helperlib, EntryPoint = "sdl_native_ellipsis_sscanf_16", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int Sscanf16(string text, string format, vptr a0, vptr a1, vptr a2, vptr a3, vptr a4, vptr a5, vptr a6, vptr a7, vptr a8, vptr a9, vptr a10, vptr a11,
												 vptr a12, vptr a13, vptr a14, vptr a15);
		}
	}
}
