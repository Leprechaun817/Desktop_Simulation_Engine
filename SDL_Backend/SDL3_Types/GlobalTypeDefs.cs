global using RuntimeUnsafe = System.Runtime.CompilerServices.Unsafe;
global using static SDL3.Native.GlobalConstants;

#pragma warning disable CS8981

global using int8 = sbyte;
global using uint8 = byte;
global using int16 = short;
global using uint16 = ushort;
global using int32 = int;
global using uint32 = uint;
global using int64 = long;
global using uint64 = ulong;

global using mptr = nint;

//ABI alias for C/SDL size_t type used in raw native signatures in interop declarations
//Use sdlSize in wrapper/public APIs when the value should carry C/SDL size semantics
global using size_t = nuint;
global using ptr = nuint;

global using unsafe vptr = void*;
global using unsafe bptr = byte*;
global using unsafe sbptr = sbyte*;
global using unsafe shptr = short*;
global using unsafe ushptr = ushort*;
global using unsafe iptr = int*;
global using unsafe uiptr = uint*;
global using unsafe lptr = long*;
global using unsafe ulptr = ulong*;

global using unsafe SDLMallocFunctionPtr = delegate* unmanaged[Cdecl]<nuint, void*>;
global using unsafe SDLCallocFunctionPtr = delegate* unmanaged[Cdecl]<nuint, nuint, void*>;
global using unsafe SDLReallocFunctionPtr = delegate* unmanaged[Cdecl]<void*, nuint, void*>;
global using unsafe SDLFreeFunctionPtr = delegate* unmanaged[Cdecl]<void*, void>;

#pragma warning restore CS8981

namespace SDL3
{
	namespace Native
	{
		public static class GlobalConstants
		{
			public const string sdllib = "SDL3.dll";
		}
	}
}