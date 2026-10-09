using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SDL3
{
	namespace Native
	{
		/// <summary>
		///	SDL3 sort/search function imports from SDL_stdinc header file
		/// </summary>

		internal static unsafe partial class SDL3_NativeSearchSort
		{
			[LibraryImport(sdllib, EntryPoint = "SDL_qsort")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial void SDL_qsort(vptr baseAddr, size_t nmemb, size_t size, SDLCompareFunctionPtr compare);

			[LibraryImport(sdllib, EntryPoint = "SDL_qsort_r")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial void SDL_qsort_r(vptr baseAddr, size_t nmemb, size_t size, SDLCompareFunctionPtr compare, vptr userdata);

			[LibraryImport(sdllib, EntryPoint = "SDL_bsearch")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr SDL_bsearch(vptr key, vptr baseAddr, size_t nmemb, size_t size, SDLCompareFunctionPtr compare);

			[LibraryImport(sdllib, EntryPoint = "SDL_bsearch_r")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr SDL_bsearch_r(vptr key, vptr baseAddr, size_t nmemb, size_t size, SDLCompareFunctionPtr compare, vptr userdata);
		}
	}
}
