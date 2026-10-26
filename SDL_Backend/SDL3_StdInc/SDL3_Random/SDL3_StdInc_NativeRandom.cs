using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SDL3
{
	namespace Native
	{
		/// <summary>
		/// Random function imports from SDL_stdinc.h
		/// </summary>
		internal static unsafe partial class SDL3_NativeRandom
		{
			[LibraryImport(sdllib, EntryPoint = "SDL_srand")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial void SDL_srand(ulong seed);

			[LibraryImport(sdllib, EntryPoint = "SDL_rand")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_rand(int n);

			[LibraryImport(sdllib, EntryPoint = "SDL_randf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_randf();

			[LibraryImport(sdllib, EntryPoint = "SDL_rand_bits")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial uint SDL_rand_bits();

			[LibraryImport(sdllib, EntryPoint = "SDL_rand_r")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_rand_r(ulptr state, int n);

			[LibraryImport(sdllib, EntryPoint = "SDL_randf_r")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_randf_r(ulptr state);

			[LibraryImport(sdllib, EntryPoint = "SDL_rand_bits_r")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial uint SDL_rand_bits_r(ulptr state);
		}
	}
}