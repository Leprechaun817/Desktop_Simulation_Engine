using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SDL3.Native;

namespace SDL3
{
	namespace Unsafe
	{
		internal sealed unsafe class SDLNativePrintfCall : IDisposable
		{
			internal const int MaxArgumentCount = 16;

			private readonly SDLNativePrintfArgument[] args;
			private readonly List<IntPtr>? ownedBuffers;
			private bool disposed;

			internal SDLNativePrintfArgument[] Arguments
			{
				get
				{
					ObjectDisposedException.ThrowIf(disposed, this);

					return args;
				}
			}

			internal int ArgumentCount
			{
				get
				{
					return args.Length;
				}
			}

			private SDLNativePrintfCall(SDLNativePrintfArgument[] args, List<IntPtr>? ownedBuffers)
			{
				this.args = args;
				this.ownedBuffers = ownedBuffers;
			}

			internal static SDLNativePrintfCall Create(ReadOnlySpan<SDLPrintfSlot> slots, int nativeWideCharByteCount)
			{
				if (slots.Length > MaxArgumentCount) {
					throw new FormatException($"SDL printf helpers support at most {MaxArgumentCount} variadic arguments.");
				}

				SDLNativePrintfArgument[] arguments = new SDLNativePrintfArgument[slots.Length];
				List<IntPtr>? ownedNativeBuffers = null;

				for (int i = 0; i < slots.Length; i++) {
					SDLPrintfSlot slot = slots[i];
					arguments[i].nativeByteCount = slot.NativeByteCount;

					switch (slot.SlotType) {
						case SDLPrintfSlotType.SignedInteger:
							arguments[i].type = (int)SDLNativePrintfArgumentType.SignedInteger;
							arguments[i].signedValue = (long)slot.Value!;
							break;
						case SDLPrintfSlotType.UnsignedInteger:
							arguments[i].type = (int)SDLNativePrintfArgumentType.UnsignedInteger;
							arguments[i].unsignedValue = (ulong)slot.Value!;
							break;
						case SDLPrintfSlotType.FloatingPoint:
							arguments[i].type = (int)SDLNativePrintfArgumentType.FloatingPoint;
							arguments[i].floatingValue = (double)slot.Value!;
							break;
						case SDLPrintfSlotType.UTF8String:
							arguments[i].type = (int)SDLNativePrintfArgumentType.UTF8String;
							arguments[i].pointerValue = AllocateUTF8String((string?)slot.Value, ref ownedNativeBuffers);
							break;
						case SDLPrintfSlotType.WideString:
							arguments[i].type = (int)SDLNativePrintfArgumentType.WideString;
							arguments[i].pointerValue = SDLNativeWideString.AllocateString((string?)slot.Value, nativeWideCharByteCount, ref ownedNativeBuffers);
							break;
						case SDLPrintfSlotType.Pointer:
							arguments[i].type = (int)SDLNativePrintfArgumentType.Pointer;
							arguments[i].pointerValue = (ptr)slot.Value!;
							break;
						default:
							throw new ArgumentOutOfRangeException(nameof(slots), $"Unknown printf slot type {slot.SlotType}.");
					}
				}

				return new SDLNativePrintfCall(arguments, ownedNativeBuffers);
			}

			public void Dispose()
			{
				if (disposed) {
					return;
				}

				if (ownedBuffers != null) {
					foreach (IntPtr buffer in ownedBuffers) {
						Marshal.FreeCoTaskMem(buffer);
					}
				}

				disposed = true;
			}

			private static ptr AllocateUTF8String(string? value, ref List<IntPtr>? ownedBuffers)
			{
				if (value == null) {
					return 0;
				}

				IntPtr buffer = Marshal.StringToCoTaskMemUTF8(value);

				if (ownedBuffers == null) {
					ownedBuffers = new List<IntPtr>();
				}

				ownedBuffers.Add(buffer);

				return (ptr)buffer;
			}
		}
	}
}