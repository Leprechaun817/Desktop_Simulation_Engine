using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SDL3
{
	namespace Native
	{
		internal static unsafe partial class SDL3_NativeIOStream
		{
			[LibraryImport(sdllib, EntryPoint = "SDL_IOFromFile")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr SDL_IOFromFile(bptr file, bptr mode);

			[LibraryImport(sdllib, EntryPoint = "SDL_IOFromMem")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr SDL_IOFromMem(bptr mem, size_t size);

			[LibraryImport(sdllib, EntryPoint = "SDL_IOFromConstMem")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr SDL_IOFromConstMem(vptr mem, size_t size);

			[LibraryImport(sdllib, EntryPoint = "SDL_IOFromDynamicMem")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr SDL_IOFromDynamicMem();


		}
	}
}