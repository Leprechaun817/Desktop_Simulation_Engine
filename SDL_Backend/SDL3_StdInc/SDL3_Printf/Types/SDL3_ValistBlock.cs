using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SDL3.Interop;
using SDL3.Native;

namespace SDL3
{
	namespace Unsafe
	{
		internal delegate SDLValistBlock SDLValistCreateBlock(ReadOnlySpan<SDLPrintfSlot> slots);

		/// <summary>
		/// Object that holds the va_list storage for a SDL printf call
		/// </summary>
		internal sealed unsafe class SDLValistBlock : IDisposable
		{
			private enum ValistAllocationKind 
			{
				None,
				Windows,
				Posix
			}

			private enum PosixMmapOffsetKind
			{
				Native,
				Int64
			}

			private const size_t MSVCx64SlotSize = 8;
			private const int SystemVx64ValistSize = 24;
			private const int SystemVx64GPRegisterCount = 6;
			private const int SystemVx64FPRegisterCount = 16;
			private const int SystemVx64GPSaveAreaBytes = 48;
			private const int SystemVx64FPSaveAreaBytes = 256;
			private const int SystemVx64RegisterSaveAreaBytes = SystemVx64GPSaveAreaBytes + SystemVx64FPSaveAreaBytes;
			private const int SystemVx64OverflowSlotSize = 8;
			private const int i386StackAlignment = 4;

			private vptr allocationBase;
			private size_t allocByteCount;
			private ValistAllocationKind allocKind;
			private vptr valistPtr;
			private List<mptr>? ownedUTF8Strings;
			private List<mptr>? ownedWideStrings;
			private bool disposed;

			private SDLValistBlock() {}

			private static void WriteUInt32(bptr target, uint value)
			{
				RuntimeUnsafe.WriteUnaligned(target, value);
			}

			private static void WritePointer(bptr target, vptr value)
			{
				RuntimeUnsafe.WriteUnaligned(target, (ulong)(size_t)value);
			}

			private static void ClearPayload(bptr payload, size_t byteCount)
			{
				bptr current = payload;
				for (size_t i = 0; i < byteCount; i++) {
					*current = 0;
					current++;
				}
			}
		}
	}
}
