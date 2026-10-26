using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SDL3
{
	namespace Native
	{
		/// <summary>
		///	Raw SDL3 narrow, UTF-8, and wide string imports
		/// </summary>
		internal static unsafe partial class SDL3_NativeString
		{
			//String functions
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

			//string-to-integer functions
			[LibraryImport(sdllib, EntryPoint = "SDL_itoa")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial bptr SDL_itoa(int value, bptr str, int radix);

			[LibraryImport(sdllib, EntryPoint = "SDL_uitoa")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial bptr SDL_uitoa(uint value, bptr str, int radix);

			[LibraryImport(sdllib, EntryPoint = "SDL_ltoa")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial bptr SDL_ltoa(CLong value, bptr str, int radix);

			[LibraryImport(sdllib, EntryPoint = "SDL_ultoa")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial bptr SDL_ultoa(CULong value, bptr str, int radix);

			[LibraryImport(sdllib, EntryPoint = "SDL_lltoa")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial bptr SDL_lltoa(long value, bptr str, int radix);

			[LibraryImport(sdllib, EntryPoint = "SDL_ulltoa")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial bptr SDL_ulltoa(ulong value, bptr str, int radix);

			[LibraryImport(sdllib, EntryPoint = "SDL_atoi")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_atoi(bptr str);

			[LibraryImport(sdllib, EntryPoint = "SDL_atof")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_atof(bptr str);

			[LibraryImport(sdllib, EntryPoint = "SDL_strtol")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial CLong SDL_strtol(bptr str, bdptr endp, int baseRadix);

			[LibraryImport(sdllib, EntryPoint = "SDL_stroul")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial CULong SDL_stroul(bptr str, bdptr endp, int baseRadix);

			[LibraryImport(sdllib, EntryPoint = "SDL_strtoll")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial long SDL_strtoll(bptr str, bdptr endp, int baseRadix);

			[LibraryImport(sdllib, EntryPoint = "SDL_strtoull")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial ulong SDL_strtoull(bptr str, bdptr endp, int baseRadix);

			[LibraryImport(sdllib, EntryPoint = "SDL_strtod")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_strtod(bptr str, bdptr endp);

			[LibraryImport(sdllib, EntryPoint = "SDL_utf8strlen")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial size_t SDL_utf8strlen(bptr str);

			[LibraryImport(sdllib, EntryPoint = "SDL_utf8strnlen")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial size_t SDL_utf8nlen(bptr str, size_t bytes);

			[LibraryImport(sdllib, EntryPoint = "SDL_StepUTF8")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial size_t SDL_StepUTF8(bdptr pstr, size_t* pslen);

			[LibraryImport(sdllib, EntryPoint = "SDL_StepBackUTF8")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial size_t SDL_StepBackUTF8(bptr start, bdptr pstr);

			[LibraryImport(sdllib, EntryPoint = "SDL_UCS4ToUTF8")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial bptr SDL_UCS4ToUTF8(uint codepoint, bptr dst);

			[LibraryImport(sdllib, EntryPoint = "SDL_strcmp")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_strcmp(bptr str1, bptr str2);

			[LibraryImport(sdllib, EntryPoint = "SDL_strcmp")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_strncmp(bptr str1, bptr str2, size_t maxlen);

			[LibraryImport(sdllib, EntryPoint = "SDL_strcmp")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_strcasecmp(bptr str1, bptr str2);

			[LibraryImport(sdllib, EntryPoint = "SDL_strcmp")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_strncasecmp(bptr str1, bptr str2, size_t maxlen);

			[LibraryImport(sdllib, EntryPoint = "SDL_strcmp")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial bptr SDL_strpbrk(bptr str, bptr breakset);

			[LibraryImport(sdllib, EntryPoint = "SDL_wcslen")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial size_t SDL_wcslen(vptr wstr);

			[LibraryImport(sdllib, EntryPoint = "SDL_wcsnlen")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial size_t SDL_wcsnlen(vptr wstr, size_t maxlen);

			[LibraryImport(sdllib, EntryPoint = "SDL_wcslcpy")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial size_t SDL_wcslcpy(vptr dst, vptr src, size_t maxlen);

			[LibraryImport(sdllib, EntryPoint = "SDL_wcslcat")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial size_t SDL_wcslcat(vptr dst, vptr src, size_t maxlen);

			[LibraryImport(sdllib, EntryPoint = "SDL_wcsdup")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr SDL_wcsdup(vptr wstr);

			[LibraryImport(sdllib, EntryPoint = "SDL_wcsstr")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr SDL_wcsstr(vptr haystack, vptr needle);

			[LibraryImport(sdllib, EntryPoint = "SDL_wcsnstr")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr SDL_wcsnstr(vptr haystack, vptr needle, size_t maxlen);

			[LibraryImport(sdllib, EntryPoint = "SDL_wcscmp")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_wcscmp(vptr str1, vptr str2);

			[LibraryImport(sdllib, EntryPoint = "SDL_wcsncmp")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_wcsncmp(vptr str1, vptr str2, size_t maxlen);

			[LibraryImport(sdllib, EntryPoint = "SDL_wcscasecmp")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_wcscasecmp(vptr str1, vptr str2);

			[LibraryImport(sdllib, EntryPoint = "SDL_wcsncasecmp")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_wcsncasecmp(vptr str1, vptr str2, size_t maxlen);

			[LibraryImport(sdllib, EntryPoint = "SDL_wcstol")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial CLong SDL_wcstol(vptr str, vdptr endp, int baseRadix);

			[LibraryImport(sdllib, EntryPoint = "SDL_wcstoul")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial CULong SDL_wcstoul(vptr str, vdptr endp, int baseRadix);

			[LibraryImport(sdllib, EntryPoint = "SDL_wcstoull")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial long SDL_wcstoll(vptr str, vdptr endp, int baseRadix);

			[LibraryImport(sdllib, EntryPoint = "SDL_wcs")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial ulong SDL_wcstoull(vptr str, vdptr endp, int baseRadix);
		}
	}
}
