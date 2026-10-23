using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace SDL3
{
	namespace Native
	{
		/// <summary>
		///	Raw SDL3 narrow, UTF-8, and wide string imports
		/// </summary>
		internal static unsafe partial class SDL3_NativeString
		{
			[LibraryImport(sdllib, EntryPoint = "SDL_strlen")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial size_t SDL_strlen(bptr str);

			[LibraryImport(sdllib, EntryPoint = "SDL_strnlen")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial size_t SDL_strnlen(bptr str, size_t maxlen);

			[LibraryImport(sdllib, EntryPoint = "SDL_strlcpy")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial size_t SDL_strlcpy(bptr dst, bptr src, size_t maxlen);

			[LibraryImport(sdllib, EntryPoint = "SDL_utf8strlcpy")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial size_t SDL_utf8strlcpy(bptr dst, bptr src, size_t dstBytes);

			[LibraryImport(sdllib, EntryPoint = "SDL_strlcat")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial size_t SDL_strlcat(bptr dst, bptr src, size_t maxlen);

			[LibraryImport(sdllib, EntryPoint = "SDL_strdup")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial bptr SDL_strdup(bptr str);

			[LibraryImport(sdllib, EntryPoint = "SDL_strrev")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial bptr SDL_strrev(bptr str, size_t maxlen);

			[LibraryImport(sdllib, EntryPoint = "SDL_strupr")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial bptr SDL_strupr(bptr str);

			[LibraryImport(sdllib, EntryPoint = "SDL_strlwr")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial bptr SDL_strlwr(bptr str);

			[LibraryImport(sdllib, EntryPoint = "SDL_strchr")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial bptr SDL_strchr(bptr str, int c);

			[LibraryImport(sdllib, EntryPoint = "SDL_strrchr")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial bptr SDL_strrchr(bptr str, int c);

			[LibraryImport(sdllib, EntryPoint = "SDL_strstr")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial bptr SDL_strstr(bptr haystack, bptr needle);

			[LibraryImport(sdllib, EntryPoint = "SDL_strnstr")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial bptr SDL_strnstr(bptr haystack, bptr needle, size_t maxlen);

			[LibraryImport(sdllib, EntryPoint = "SDL_strtok_r")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial bptr SDL_strtok_r(bptr str, bptr delim, bdptr saveptr);
		}

	}
}