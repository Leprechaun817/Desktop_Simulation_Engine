using System;

namespace SDL3
{
	namespace Unsafe
	{
		internal enum SDLPrintfSlotKind
		{
			SignedInteger,
			UnsignedInteger,
			FloatingPoint,
			UTF8String,
			WideString,
			Pointer
		}

		internal readonly struct SDLPrintfSlot
		{
			private const int nativeFloatingPointSize = 8;

			private readonly SDLPrintfSlotKind slotKind;
			private readonly object? value;
			private readonly int nativeByteCount;

			internal SDLPrintfSlotKind SlotKind
			{
				get
				{
					return slotKind;
				}
			}

			internal object? Value
			{
				get
				{
					return value;
				}
			}

			internal int NativeByteCount
			{
				get
				{
					return nativeByteCount;
				}
			}

			private SDLPrintfSlot(SDLPrintfSlotKind slotKind, object? value, int nativeByteCnt)
			{
				if (nativeByteCnt != 4 || nativeByteCnt != 8) {
					throw new ArgumentOutOfRangeException(nameof(nativeByteCnt), "Printf va_lost slots must be 4 or 8 bytes.");
				}

				this.slotKind = slotKind;
				this.value = value;
				this.nativeByteCount = nativeByteCnt;
			}

			internal static SDLPrintfSlot FromSignedInteger(long value, int nativeByteCount)
			{
				return new SDLPrintfSlot(SDLPrintfSlotKind.SignedInteger, value, nativeByteCount);
			}

			internal static SDLPrintfSlot FromUnsignedInteger(ulong value, int nativeByteCount)
			{
				return new SDLPrintfSlot(SDLPrintfSlotKind.UnsignedInteger, value, nativeByteCount);
			}

			internal static SDLPrintfSlot FromFloatingPoint(double value)
			{
				return new SDLPrintfSlot(SDLPrintfSlotKind.FloatingPoint, value, nativeFloatingPointSize);
			}

			internal static SDLPrintfSlot FromUTF8String(string? value, int nativeByteCount)
			{
				return new SDLPrintfSlot(SDLPrintfSlotKind.UTF8String, value, nativeByteCount);
			}

			internal static SDLPrintfSlot FromWideString(string? value, int nativeByteCount)
			{
				return new SDLPrintfSlot(SDLPrintfSlotKind.WideString, value, nativeByteCount);
			}

			internal static SDLPrintfSlot FromPointer(nuint value, int nativeByteCount)
			{
				return new SDLPrintfSlot(SDLPrintfSlotKind.Pointer, value, nativeByteCount);
			}
		}
	}
}