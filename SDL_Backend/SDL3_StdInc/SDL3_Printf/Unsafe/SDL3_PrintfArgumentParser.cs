using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SDL3.Interop;

namespace SDL3
{
	namespace Unsafe
	{
		internal enum SDLPrintfStringFormatMode
		{
			UTF8_Mode,
			WideChar_Mode
		}

		internal enum SDLPrintfIntegerSlotSize
		{
			ShortInt = 4,
			LongInt = 8
		}

		internal static class SDLPrintfArgumentParser
		{
			internal static SDLPrintfSlot[] Parse(string fmt, object?[] args, SDLPrintfArgumentLayout layout, SDLPrintfStringFormatMode strFmtMode)
			{
				ArgumentNullException.ThrowIfNull(fmt);
				ArgumentNullException.ThrowIfNull(args);

				List<SDLPrintfSlot> slots = new List<SDLPrintfSlot>();

				int argIndex = 0;
				for (int i = 0; i < fmt.Length; i++) {
					if (fmt[i] != '%') {
						continue;
					}

					i++;
					if (i >= fmt.Length) {
						throw new FormatException("The format string ends with an incomplete '%' conversion.");
					}

					if (fmt[i] == '%') {
						continue;
					}
				}
			}

			private static SDLPrintfSlot ReadIntegerSlot(object?[] args, ref int argIndex, string argDescription)
			{
				object? value = ReadNextArgument(args, ref argIndex, argDescription);

				switch (value) {
					case sbyte typed:
						return SDLPrintfSlot.FromSignedInteger(typed, (int)SDLPrintfIntegerSlotSize.ShortInt);
					case byte typed:
						return SDLPrintfSlot.FromSignedInteger(typed, (int)SDLPrintfIntegerSlotSize.ShortInt);
					case short typed:
						return SDLPrintfSlot.FromSignedInteger(typed, (int)SDLPrintfIntegerSlotSize.ShortInt);
					case ushort typed:
						return SDLPrintfSlot.FromSignedInteger(typed, (int)SDLPrintfIntegerSlotSize.ShortInt);
					case int typed:
						return SDLPrintfSlot.FromSignedInteger(typed, (int)SDLPrintfIntegerSlotSize.ShortInt);
					case uint typed:
						if (typed <= int.MaxValue) {
							return SDLPrintfSlot.FromSignedInteger(typed, (int)SDLPrintfIntegerSlotSize.ShortInt);
						}
						else {
							goto default;
						}
					default:
						throw new ArgumentException($"The {argDescription} argument must fit in a C integer type.", nameof(args));
				}
			}

			private static SDLPrintfSlot ReadSignedIntegerSlot(string slotSizeModifier, object?[] args, ref int argIndex, char conversion, SDLPrintfArgumentLayout layout)
			{
				object? value = ReadNextArgument(args, ref argIndex, $"%{conversion}");
				int nativeSignedIntegerSlotSize = GetNativeIntegerSlotSize(slotSizeModifier, layout);

				switch (value) {
					case sbyte typed:
						return SDLPrintfSlot.FromSignedInteger(typed, nativeSignedIntegerSlotSize);
					case byte typed:
						return SDLPrintfSlot.FromSignedInteger(typed, nativeSignedIntegerSlotSize);
					case short typed:
						return SDLPrintfSlot.FromSignedInteger(typed, nativeSignedIntegerSlotSize);
					case ushort typed:
						return SDLPrintfSlot.FromSignedInteger(typed, nativeSignedIntegerSlotSize);
					case int typed:
						return SDLPrintfSlot.FromSignedInteger(typed, nativeSignedIntegerSlotSize);
					case uint typed:
						return SDLPrintfSlot.FromSignedInteger(typed, nativeSignedIntegerSlotSize);
					case long typed:
						return SDLPrintfSlot.FromSignedInteger(typed, nativeSignedIntegerSlotSize);
					case nint typed:
						return SDLPrintfSlot.FromSignedInteger((long)typed, nativeSignedIntegerSlotSize);
					case CLong typed:
						return SDLPrintfSlot.FromSignedInteger((long)typed.Value, nativeSignedIntegerSlotSize);
					default:
						throw new ArgumentException($"the %{conversion} conversion requires a signed integer scalar.", nameof(args));
				}
			}

			private static SDLPrintfSlot ReadUnsignedIntegerSlot(string slotSizeModifier, object?[] args, ref int argIndex, char conversion, SDLPrintfArgumentLayout layout) 
			{
				object? value = ReadNextArgument(args, ref argIndex, $"%{conversion}");
				int nativeUnsignedIntegerSlotSize = GetNativeIntegerSlotSize(slotSizeModifier, layout);

				switch (value) {
					case sbyte typed:
						return SDLPrintfSlot.FromUnsignedInteger(unchecked((ulong)typed), nativeUnsignedIntegerSlotSize);
					case byte typed:
						return SDLPrintfSlot.FromUnsignedInteger(typed, nativeUnsignedIntegerSlotSize);
					case short typed:
						return SDLPrintfSlot.FromUnsignedInteger(unchecked((ulong)typed), nativeUnsignedIntegerSlotSize);
					case ushort typed:
						return SDLPrintfSlot.FromUnsignedInteger(typed, nativeUnsignedIntegerSlotSize);
					case int typed:
						return SDLPrintfSlot.FromUnsignedInteger(unchecked((ulong)typed), nativeUnsignedIntegerSlotSize);
					case uint typed:
						return SDLPrintfSlot.FromUnsignedInteger(typed, nativeUnsignedIntegerSlotSize);
					case long typed:
						return SDLPrintfSlot.FromUnsignedInteger(unchecked((ulong)typed), nativeUnsignedIntegerSlotSize);
					case ulong typed:
						return SDLPrintfSlot.FromUnsignedInteger(typed, nativeUnsignedIntegerSlotSize);
					case nint typed:
						return SDLPrintfSlot.FromUnsignedInteger(unchecked((ulong)(long)typed), nativeUnsignedIntegerSlotSize);
					case nuint typed:
						return SDLPrintfSlot.FromUnsignedInteger((ulong)typed, nativeUnsignedIntegerSlotSize);
					case CLong typed:
						return SDLPrintfSlot.FromUnsignedInteger(unchecked((ulong)(long)typed.Value), nativeUnsignedIntegerSlotSize);
					case CULong typed:
						return SDLPrintfSlot.FromUnsignedInteger((ulong)typed.Value, nativeUnsignedIntegerSlotSize);
					default:
						throw new ArgumentException($"The %{conversion} conversion requires an integer scalar.", nameof(args));
				}
			}

			private static SDLPrintfSlot ReadCharacterSlot(string slotSizeModifier, object?[] args, ref int argIndex, SDLPrintfStringFormatMode mode)
			{
				if (mode == SDLPrintfStringFormatMode.UTF8_Mode && slotSizeModifier.Length != 0) {
					throw new NotSupportedException("Wide or length-modified %c conversions are not supported by the SDL UTF-8 printf path");
				}
				else if (mode == SDLPrintfStringFormatMode.WideChar_Mode && (slotSizeModifier != "" || slotSizeModifier != "l")) {
					throw new NotSupportedException($"Unsupported SDL wide printf character length modifier '{slotSizeModifier}'.");
				}

				object? value = ReadNextArgument(args, ref argIndex, "%c")

			}

			private static int SkipFlags(string fmt, int index)
			{
				while (index < fmt.Length) {
					switch (fmt[index]) {
						case '-':
						case '+':
						case ' ':
						case '#':
						case '0':
						case '\'':
							index++;
							break;
						default:
							return index;
					}
				}

				return index;
			}

			private static int SkipAsciiDigits(string fmt, int index)
			{
				while ((index < fmt.Length) && (fmt[index] >= '0') && (fmt[index] <= '9')) {
					index++;
				}

				return index;
			}

			private static string ReadLengthModifier(string fmt, ref int index)
			{
				if (index >= fmt.Length) {
					return string.Empty;
				}

				char first = fmt[index];
				if (first == 'h' || first == 'l') {
					if (((index + 1) < fmt.Length) && (fmt[index + 1] == first)) {
						index += 2;

						if (first == 'h') {
							return "hh";
						}
						else {
							return "ll";
						}
					}

					index++;

					return first.ToString();
				}

				if (first == 'j' || first == 'z' || first == 't' || first == 'L') {
					index++;

					return first.ToString();
				}

				return string.Empty;
			}

			private static int GetNativeIntegerSlotSize(string slotSizeModifier, SDLPrintfArgumentLayout layout)
			{
				switch (slotSizeModifier) {
					case "":
					case "h":
					case "hh":
						return (int)SDLPrintfIntegerSlotSize.ShortInt;
					case "l":
						return layout.longByteCount;
					case "ll":
					case "j":
						return (int)SDLPrintfIntegerSlotSize.LongInt;
					case "z":
					case "t":
						return layout.ptrByteCount;
					default:
						throw new NotSupportedException($"Unsupported integer slot size modifier '{slotSizeModifier}'.");
				}
			}

			private static object? ReadNextArgument(object?[] args, ref int argIndex, string argDescription)
			{
				if (argIndex >= args.Length) {
					throw new FormatException($"The format string requires an argument for {argDescription}, but no argument remains.");
				}

				return args[argIndex++];
			}
		}
	}
}