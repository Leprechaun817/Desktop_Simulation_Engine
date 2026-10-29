using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SDL3
{
	namespace Native
	{
		internal static unsafe partial class SDL3_NativeCharacter
		{
			[LibraryImport(sdllib, EntryPoint = "SDL_isalpha")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_isalpha(int x);

			[LibraryImport(sdllib, EntryPoint = "SD_isalnum")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_isalnum(int x);

			[LibraryImport(sdllib, EntryPoint = "SDL_isblank")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_isblank(int x);

			[LibraryImport(sdllib, EntryPoint = "SDL_iscntrl")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_iscntrl(int x);

			[LibraryImport(sdllib, EntryPoint = "SDL_isdigit")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_isdigit(int x);

			[LibraryImport(sdllib, EntryPoint = "SDL_isxdigit")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_

			[LibraryImport(sdllib, EntryPoint = "SDL_ispunct")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_

			[LibraryImport(sdllib, EntryPoint = "SDL_isspace")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_

			[LibraryImport(sdllib, EntryPoint = "SDL_isupper")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_

			[LibraryImport(sdllib, EntryPoint = "SDL_islower")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_

			[LibraryImport(sdllib, EntryPoint = "SDL_isgraph")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_

			[LibraryImport(sdllib, EntryPoint = "SDL_toupper")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_

			[LibraryImport(sdllib, EntryPoint = "SDL_tolower")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_

		}
	}
}