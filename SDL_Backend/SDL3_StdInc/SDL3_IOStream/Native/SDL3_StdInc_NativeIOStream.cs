using SDL3.Unsafe;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;

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

			[LibraryImport(sdllib, EntryPoint = "SDL_OpenIO")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr SDL_OpenIO(SDLIOStreamInterface* IOInterface, vptr userdata);

			[LibraryImport(sdllib, EntryPoint = "SDL_CloseIO")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial bool SDL_CloseIO(vptr context);

			[LibraryImport(sdllib, EntryPoint = "SDL_GetIOProperties")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial uint SDL_GetIOProperties(vptr context);

			[LibraryImport(sdllib, EntryPoint = "SDL_GetIOStatus")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial SDLIOStatus SDL_GetIOStatus(vptr context);

			[LibraryImport(sdllib, EntryPoint = "SDL_GetIOSize")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial long SDL_GetIOSize(vptr context);

			[LibraryImport(sdllib, EntryPoint = "SDL_SeekIO")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial long SDL_SeekIO(vptr context, long offset, SDLIOSeekStartPos seekPos);

			[LibraryImport(sdllib, EntryPoint = "SDL_TellIO")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial long SDL_TellIO(vptr context);

			[LibraryImport(sdllib, EntryPoint = "SDL_ReadIO")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial size_t SDL_ReadIO(vptr context, vptr pointer, size_t size);

			[LibraryImport(sdllib, EntryPoint = "SDL_WriteIO")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial size_t SDL_WriteIO(vptr context, vptr pointer, size_t size);

			[LibraryImport(sdllib, EntryPoint = "IOvprintf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial size_t SDL_IOvprintf(vptr context, bptr fmt, vptr ap);

			[LibraryImport(sdllib, EntryPoint = "FlushIO")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			[return: MarshalAs(UnmanagedType.I1)]
			internal static partial bool SDL_FlushIO(vptr context);

			[LibraryImport(sdllib, EntryPoint = "SDL_LoadFile_IO")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr SDL_LoadFile_IO(vptr src, size_t* datasize, [MarshalAs(UnmanagedType.I1)] bool closeio);

			[LibraryImport(sdllib, EntryPoint = "SDL_LoadFile")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr SDL_LoadFile(bptr file, size_t* datasize);

			[LibraryImport(sdllib, EntryPoint = "SDL_SaveFile_IO")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			[return: MarshalAs(UnmanagedType.I1)]
			internal static partial bool SDL_SaveFile_IO(vptr src, vptr data, size_t datasize, [MarshalAs(UnmanagedType.I1)] bool closeio);

			[LibraryImport(sdllib, EntryPoint = "SDL_SaveFile")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			[return: MarshalAs(UnmanagedType.I1)]
			internal static partial bool SDL_SaveFile(bptr file, vptr data, size_t datasize);

			[LibraryImport(sdllib, EntryPoint = "SDL_ReadU8")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			[return: MarshalAs(UnmanagedType.I1)]
			internal static partial bool SDL_ReadU8(vptr src, bptr value);

			[LibraryImport(sdllib, EntryPoint = "SDL_ReadS8")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			[return: MarshalAs(UnmanagedType.I1)]

			[LibraryImport(sdllib, EntryPoint = "SDL_ReadU16LE")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			[return: MarshalAs(UnmanagedType.I1)]

			[LibraryImport(sdllib, EntryPoint = "SDL_ReadS16LE")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			[return: MarshalAs(UnmanagedType.I1)]

			[LibraryImport(sdllib, EntryPoint = "SDL_ReadU16BE")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			[return: MarshalAs(UnmanagedType.I1)]

			[LibraryImport(sdllib, EntryPoint = "SDL_ReadS16BE")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			[return: MarshalAs(UnmanagedType.I1)]

			[LibraryImport(sdllib, EntryPoint = "SDL_ReadU32LE")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			[return: MarshalAs(UnmanagedType.I1)]

			[LibraryImport(sdllib, EntryPoint = "SDL_ReadS32LE")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			[return: MarshalAs(UnmanagedType.I1)]

			[LibraryImport(sdllib, EntryPoint = "SDL_ReadU32BE")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			[return: MarshalAs(UnmanagedType.I1)]

			[LibraryImport(sdllib, EntryPoint = "SDL_ReadS32BE")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			[return: MarshalAs(UnmanagedType.I1)]

			[LibraryImport(sdllib, EntryPoint = "SDL_ReadU64LE")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			[return: MarshalAs(UnmanagedType.I1)]

			[LibraryImport(sdllib, EntryPoint = "SDL_ReadS64LE")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			[return: MarshalAs(UnmanagedType.I1)]

			[LibraryImport(sdllib, EntryPoint = "SDL_ReadU64BE")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			[return: MarshalAs(UnmanagedType.I1)]

			[LibraryImport(sdllib, EntryPoint = "SDL_ReadS64BE")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			[return: MarshalAs(UnmanagedType.I1)]

			[LibraryImport(sdllib, EntryPoint = "SDL_WriteU8")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			[return: MarshalAs(UnmanagedType.I1)]

			[LibraryImport(sdllib, EntryPoint = "SDL_WriteS8")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			[return: MarshalAs(UnmanagedType.I1)]
		}
	}
}