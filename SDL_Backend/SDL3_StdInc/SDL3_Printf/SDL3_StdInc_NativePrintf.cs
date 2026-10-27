using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SDL3
{
	namespace Native
	{
		/// <summary>
		/// Native ABI kept intentionally visible. va_list pointer is ABI-specific
		/// and is produced by a platform-specific unsafe wrapper
		/// </summary>
		internal static unsafe partial class SDL3_NativePrintf
		{
			[LibraryImport(sdllib, EntryPoint = "SDL_vsnprintf", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_vsnprintf(bptr text, size_t maxlen, string fmt, vptr ap);

			[LibraryImport(sdllib, EntryPoint = "SDL_vsscanf", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_vsscanf(string text, string fmt, vptr ap);

			[LibraryImport(sdllib, EntryPoint = "SDL_vswprinf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_vswprintf(vptr text, size_t maxlen, vptr fmt, vptr ap);

			[LibraryImport(sdllib, EntryPoint = "SDL_vasprintf", StringMarshalling = StringMarshalling.Utf8)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_vasprintf(bdptr text, string fmt, vptr ap);
		}
	}
}