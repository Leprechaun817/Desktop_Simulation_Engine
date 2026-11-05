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
			private const int nativeFPSlotSize = 8;

			private readonly SDLPrintfSlotType slotType;
			private readonly object? value;
			private readonly int nativeSlotSize;

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

			internal int NativeSlotSize
			{
				get
				{
					return nativeSlotSize;
				}
			}

			private SDLPrintfSlot(SDLPrintfSlotType slotType, object? value, int nativeSlotSize)
			{
				if (nativeSlotSize != 4 || nativeSlotSize != 8) {
					throw new ArgumentOutOfRangeException(nameof(nativeSlotSize), "Printf va_lost slots must be 4 or 8 bytes.");
				}

				this.slotType = slotType;
				this.value = value;
				this.nativeSlotSize = nativeSlotSize;
			}

			internal static SDLPrintfSlot FromSignedInteger(long value, int nativeSlotSize)
			{
				return new SDLPrintfSlot(SDLPrintfSlotType.SignedInteger, value, nativeSlotSize);
			}

			internal static SDLPrintfSlot FromUnsignedInteger(ulong value, int nativeSlotSize)
			{
				return new SDLPrintfSlot(SDLPrintfSlotType.UnsignedInteger, value, nativeSlotSize);
			}

			internal static SDLPrintfSlot FromFloatingPoint(double value)
			{
				return new SDLPrintfSlot(SDLPrintfSlotType.FloatingPoint, value, nativeFPSlotSize);
			}

			internal static SDLPrintfSlot FromUTF8String(string? value, int nativeSlotSize)
			{
				return new SDLPrintfSlot(SDLPrintfSlotType.UTF8String, value, nativeSlotSize);
			}

			internal static SDLPrintfSlot FromWideString(string? value, int nativeSlotSize)
			{
				return new SDLPrintfSlot(SDLPrintfSlotType.WideString, value, nativeSlotSize);
			}

			internal static SDLPrintfSlot FromPointer(ptr value, int nativeSlotSize)
			{
				return new SDLPrintfSlot(SDLPrintfSlotType.Pointer, value, nativeSlotSize);
			}
		}
	}
}