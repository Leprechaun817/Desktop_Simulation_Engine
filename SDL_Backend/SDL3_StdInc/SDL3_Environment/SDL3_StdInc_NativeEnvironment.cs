using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SDL3
{
	namespace Native
	{
		/// <summary>
		/// SDL3 environment function imports from the SDL_stdinc header file
		/// </summary>
		internal static unsafe partial class SDL3_NativeEnvironment
		{
			[LibraryImport(sdllib, EntryPoint = "SDL_GetEnviornment")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr SDL_GetEnvironment();

			[LibraryImport(sdllib, EntryPoint = "SDL_CreateEnvironment")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr SDL_CreateEnvironment([MarshalAs(UnmanagedType.I1)] bool populated);

			[LibraryImport(sdllib, EntryPoint = "SDL_GetEnvironmentVariable")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial bptr SDL_GetEnvironmentVariable(vptr env, bptr name);

			[LibraryImport(sdllib, EntryPoint = "SDL_GetEnvironmentVariables")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial bptr* SDL_GetEnvironmentVariables(vptr env);

			[LibraryImport(sdllib, EntryPoint = "SDL_SetEnvironmentVariable")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			[return: MarshalAs(UnmanagedType.I1)]
			internal static partial bool SDL_SetEnvironmentVariable(vptr env, bptr name, bptr value, [MarshalAs(UnmanagedType.I1)] bool overwrite);

			[LibraryImport(sdllib, EntryPoint = "SDL_UnsetEnvironmentVariable")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			[return: MarshalAs(UnmanagedType.I1)]
			internal static partial bool SDL_UnsetEnvironmentVariable(vptr env, bptr name);

			[LibraryImport(sdllib, EntryPoint = "SDL_DestroyEnvironment")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial void SDL_DestroyEnvironment(vptr env);

			[LibraryImport(sdllib, EntryPoint = "SDL_getenv")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial bptr SDL_getenv(bptr name);

			[LibraryImport(sdllib, EntryPoint = "SDL_getenv_unsafe")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial bptr SDL_getenv_unsafe(bptr name);

			[LibraryImport(sdllib, EntryPoint = "SDL_setenv_unsafe")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_setenv_unsafe(bptr name, bptr value, int overwrite);

			[LibraryImport(sdllib, EntryPoint = "SDL_unsetenv_unsafe")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_unsetenv_unsafe(bptr name);
		}
	}	
}