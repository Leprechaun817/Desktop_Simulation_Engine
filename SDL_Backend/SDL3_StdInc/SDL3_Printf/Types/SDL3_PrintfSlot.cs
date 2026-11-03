using System;

namespace SDL3
{
	namespace Unsafe
	{
		internal enum SDLPrintfSlotType
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

			private readonly SDLPrintfSlotType slotType;
			private readonly object? value;
			private readonly int nativeByteCount;

			internal SDLPrintfSlotType SlotType
			{
				get
				{
					return slotType;
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

			private SDLPrintfSlot(SDLPrintfSlotType slotKind, object? value, int nativeByteCnt)
			{
				if (nativeByteCnt != 4 || nativeByteCnt != 8) {
					throw new ArgumentOutOfRangeException(nameof(nativeByteCnt), "Printf va_lost slots must be 4 or 8 bytes.");
				}

				this.slotType = slotKind;
				this.value = value;
				this.nativeByteCount = nativeByteCnt;
			}

			internal static SDLPrintfSlot FromSignedInteger(long value, int nativeByteCount)
			{
				return new SDLPrintfSlot(SDLPrintfSlotType.SignedInteger, value, nativeByteCount);
			}

			internal static SDLPrintfSlot FromUnsignedInteger(ulong value, int nativeByteCount)
			{
				return new SDLPrintfSlot(SDLPrintfSlotType.UnsignedInteger, value, nativeByteCount);
			}

			internal static SDLPrintfSlot FromFloatingPoint(double value)
			{
				return new SDLPrintfSlot(SDLPrintfSlotType.FloatingPoint, value, nativeFloatingPointSize);
			}

			internal static SDLPrintfSlot FromUTF8String(string? value, int nativeByteCount)
			{
				return new SDLPrintfSlot(SDLPrintfSlotType.UTF8String, value, nativeByteCount);
			}

			internal static SDLPrintfSlot FromWideString(string? value, int nativeByteCount)
			{
				return new SDLPrintfSlot(SDLPrintfSlotType.WideString, value, nativeByteCount);
			}

			internal static SDLPrintfSlot FromPointer(nuint value, int nativeByteCount)
			{
				return new SDLPrintfSlot(SDLPrintfSlotType.Pointer, value, nativeByteCount);
			}
		}
	}
}