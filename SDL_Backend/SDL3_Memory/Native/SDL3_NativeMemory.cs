using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SDL3
{
	namespace Native
	{
		/// <summary>
		///	Raw SDL3 memory imports.
		///	This layer intentionally mirrors the native SDL3 C API as closely
		///	as C# will allow. Wrapper layers are preferred for normal engine code.
		/// </summary>
		internal static unsafe partial class SDL3_NativeMemory
		{
			[LibraryImport(sdllib, EntryPoint = "SDL_malloc")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr SDL_malloc(size_t size);

			[LibraryImport(sdllib, EntryPoint = "SDL_calloc")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr SDL_calloc(size_t nmemb, size_t size);

			[LibraryImport(sdllib, EntryPoint = "SDL_realloc")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr SDL_realloc(vptr mem, size_t size);

			[LibraryImport(sdllib, EntryPoint = "SDL_free")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial void SDL_free(vptr mem);

			[LibraryImport(sdllib, EntryPoint = "SDL_aligned_alloc")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr SDL_aligned_alloc(size_t alignment, size_t size);

			[LibraryImport(sdllib, EntryPoint = "SDL_aligned_free")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial void SDL_aligned_free(vptr mem);

			[LibraryImport(sdllib, EntryPoint = "SDL_GetMemoryFunctions")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial void SDL_GetMemoryFunctions(out SDLMallocFunctionPtr mallocFunc, out SDLCallocFunctionPtr callocFunc, out SDLReallocFunctionPtr reallocFunc,
																out SDLFreeFunctionPtr freeFunc);

			[LibraryImport(sdllib, EntryPoint = "SDL_GetOriginalMemoryFunctions")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial void SDL_GetOriginalMemoryFunctions(out SDLMallocFunctionPtr mallocFunc, out SDLCallocFunctionPtr callocFunc, out SDLReallocFunctionPtr reallocFunc,
																		out SDLFreeFunctionPtr freeFunc);

			/// <summary>
			/// Raw SDL_SetMemoryFunctions import
			/// Since a C boolean type is used as the SDL3 bool, a byte type is the return
			/// value here to keep the native boundary explicit. This also avoids the WIN32
			/// BOOL-style marshalling assumptions.
			/// </summary>
			[LibraryImport(sdllib, EntryPoint = "SDL_SetMemoryFunctions")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial byte SDL_SetMemoryFunctions(SDLMallocFunctionPtr mallocFunc, SDLCallocFunctionPtr callocFunc, SDLReallocFunctionPtr reallocFunc,
																SDLFreeFunctionPtr freeFunc);

			[LibraryImport(sdllib, EntryPoint = "SDL_GetNumAllocations")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_GetNumAllocations();

			[LibraryImport(sdllib, EntryPoint = "SDL_memcpy")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr SDL_memcpy(vptr dst, vptr src, size_t len);

			[LibraryImport(sdllib, EntryPoint = "SDL_memmove")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr SDL_memmove(vptr dst, vptr src, size_t len);

			[LibraryImport(sdllib, EntryPoint = "SDL_memset")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr SDL_memset(vptr dst, int c, size_t len);

			[LibraryImport(sdllib, EntryPoint = "SDL_memset4")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr SDL_memset4(vptr dst, uint value, size_t dwords);

			[LibraryImport(sdllib, EntryPoint = "SDL_memcmp")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_memcmp(vptr s1, vptr s2, size_t len);
		}
	}
}
