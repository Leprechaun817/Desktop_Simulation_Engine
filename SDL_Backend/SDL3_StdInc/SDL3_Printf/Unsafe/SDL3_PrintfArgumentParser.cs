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

		internal enum SDLPrintfIntegerWidth 
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
						return SDLPrintfSlot.FromSignedInteger(typed, (int)SDLPrintfIntegerWidth.ShortInt);
					case byte typed:
						return SDLPrintfSlot.FromSignedInteger(typed, (int)SDLPrintfIntegerWidth.ShortInt);
					case short typed:
						return SDLPrintfSlot.FromSignedInteger(typed, (int)SDLPrintfIntegerWidth.ShortInt);
					case ushort typed:
						return SDLPrintfSlot.FromSignedInteger(typed, (int)SDLPrintfIntegerWidth.ShortInt);
					case int typed:
						return SDLPrintfSlot.FromSignedInteger(typed, (int)SDLPrintfIntegerWidth.ShortInt);
					case uint typed:
						if (typed <= int.MaxValue) {
							return SDLPrintfSlot.FromSignedInteger(typed, (int)SDLPrintfIntegerWidth.ShortInt);
						}
						else {
							goto default;
						}
					default:
						throw new ArgumentException($"The {argDescription} argument must fit in a C integer type.", nameof(args));
				}
			}

			private static SDLPrintfSlot ReadSignedIntegerSlot(string widthModifier, object?[] args, ref int argIndex, char conversion, SDLPrintfArgumentLayout layout)
			{
				object? value = ReadNextArgument(args, ref argIndex, $"%{conversion}");

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
				while ((index < fmt.Length) && (fmt[index] >= '0') && (fmt[index] <= '9'){
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

			private static int GetNativeIntegerBitWidth(string widthModifier, SDLPrintfArgumentLayout layout)
			{
				switch (widthModifier) {
					case "":
					case "h":
					case "hh":
						return (int)SDLPrintfIntegerWidth.ShortInt;
					case "l":
						return layout.longByteCount;
					case "ll":
					case "j":
						return (int)SDLPrintfIntegerWidth.LongInt;
					case "z":
					case "t":
						return layout.ptrByteCount;
					default:
						throw new NotSupportedException($"Unsupported integer length modifier '{widthModifier}'.");
				}
			}

			private static object? ReadNextArgument(object?[] args, ref int argIndex, string role)
			{
				if (argIndex >= args.Length) {
					throw new FormatException($"The format string requires an argument for {role}, but no argument remains.");
				}

				return args[argIndex++];
			}
		}
	}
}