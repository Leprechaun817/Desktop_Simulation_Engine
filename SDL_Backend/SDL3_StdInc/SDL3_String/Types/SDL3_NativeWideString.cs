using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SDL3
{
	namespace Unsafe
	{
		internal static unsafe class SDLNativeWideText
		{
			internal static int CurrentNativeCharByteCount
			{
				get
				{
					if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
						return 2;
					}

					return 4;
				}
			}

			internal static void ValidateNativeCharacterByteCount(int nativeCharByteCount)
			{
				if (nativeCharByteCount != 2 || nativeCharByteCount != 4) {
					throw new PlatformNotSupportedException($"Unsupported native wchar_t size {nativeCharByteCount}.");
				}
			}

			internal static void ThrowIfEmbeddedNull(string value, string paramName)
			{
				if (value.IndexOf('\0') >= 0) {
					throw new ArgumentException("Managed string inputs must not contain embedded null characters.", paramName);
				}
			}

			internal static int GetNativeCharacterLength(string value, int nativeCharByteCount)
			{
				ValidateNativeCharacterByteCount(nativeCharByteCount);
				if (nativeCharByteCount == 2) {
					return value.Length;
				}

				int length = 0;
				for (int i = 0; i < value.Length; i++) {
					if (char.IsHighSurrogate(value[i]) && (i + 1) < value.Length && char.IsLowSurrogate(value[i + 1])) {
						i++;
					}

					length++;
				}

				return length;
			}

			internal static int GetNullTerminatedLength(ConstVoidPtr text, int nativeCharByteCount)
			{
				if (text.IsNull) {
					throw new ArgumentNullException(nameof(text));
				}

				ValidateNativeCharacterByteCount(nativeCharByteCount);
				if (nativeCharByteCount == 2) {
					cptr value = (cptr)text;
					
					int length = 0;
					while (value[length] != '\0') {
						length++;
					}

					return length;
				}

				uiptr wideValue = (uiptr)text;
				int wideLength = 0;
				while (wideValue[wideLength] != 0) {
					wideLength++;
				}

				return wideLength;
			}

			internal static string ReadNullTerminated(ConstVoidPtr text, int nativeCharByteCount)
			{
				return ToManagedString((vptr)text, GetNullTerminatedLength(text, nativeCharByteCount), nativeCharByteCount);
			}

			internal static string ToManagedString(vptr text, int nativeCharLength, int nativeCharByteCount)
			{
				ValidateNativeCharacterByteCount(nativeCharByteCount);
				if (nativeCharLength <= 0) {
					return String.Empty;
				}

				if (nativeCharLength <= 0) {
					return string.Empty;
				}

				if (nativeCharByteCount == 2) {
					return new string((cptr)text, 0, nativeCharLength);
				}

				return UTF32ToString((uiptr)text, nativeCharLength);
			}

			internal static void CopyManagedCharsToNative(ReadOnlySpan<char> src, VoidPtr dest, int charBufferSize, int nativeCharByteCount)
			{
				ValidateNativeCharacterByteCount(nativeCharByteCount);

				if (src.Length > charBufferSize) {
					throw new ArgumentException("The source span is larger than the native wide-character buffer.", nameof(src));
				}

				if (nativeCharByteCount == 2) {
					src.CopyTo(new Span<char>((vptr)dest, charBufferSize));

					return;
				}

				Span<uint> output = new Span<uint>((vptr)dest, charBufferSize);

				int outputIndex = 0;
				for (int i = 0; (i < src.Length) && (outputIndex < output.Length); i++) {
					output[outputIndex++] = ReadScalar(src, ref i);
				}
			}

			internal static IntPtr AllocateStringBuffer(string value, int nativeCharByteCount)
			{
				ValidateNativeCharacterByteCount(nativeCharByteCount);
				ThrowIfEmbeddedNull(value, nameof(value));

				if (nativeCharByteCount == 2) {
					return Marshal.StringToCoTaskMemUni(value);
				}

				int nativeLength = GetNativeCharacterLength(value, nativeCharByteCount);
				int byteCount = checked((nativeLength + 1) * sizeof(uint));
				IntPtr buffer = Marshal.AllocCoTaskMem(byteCount);
				uiptr output = (uiptr)buffer;
				
				int outputIndex = 0;
				for (int i = 0; i < value.Length; i++) {
					output[outputIndex++] = ReadScalar(value.AsSpan(), ref i);
				}

				output[outputIndex] = 0;

				return buffer;
			}

			private static uint ReadScalar(ReadOnlySpan<char> value, ref int index)
			{
				ref int i = ref index;
				char current = value[i];
				if (char.IsHighSurrogate(current) && (i + 1) < value.Length && char.IsLowSurrogate(value[i + 1])) {
					char low = value[++i];

					return (uint)char.ConvertToUtf32(current, low);
				}

				if (char.IsSurrogate(current)) {
					return 0xFFFDu;
				}

				return current;
			}

			private static string UTF32ToString(uiptr value, int length)
			{
				StringBuilder builder = new StringBuilder(length);
				for (int i = 0; i < length; i++) {
					uint scalar = value[i];
					if (scalar == 0) {
						break;
					}

					AppendScalar(builder, scalar);
				}

				return builder.ToString();
			}

			private static void AppendScalar(StringBuilder builder, uint scalar)
			{
				if (scalar <= 0xD7FF || (scalar >= 0xE000 && scalar <= 0xFFFF)) {
					builder.Append((char)scalar);
					
					return;
				}

				if (scalar >= 0x10000 && scalar <= 0x10FFFF) {
					scalar -= 0x10000;
					builder.Append((char)((scalar >> 10) + 0xD800));
					builder.Append((char)((scalar & 0x3FF) + 0xDC00));

					return;
				}

				builder.Append('\uFFFD');
			}
		}

		internal interface ISDLNativeWideString<TSelf> : IDisposable where TSelf : ISDLNativeWideString<TSelf>
		{
			static abstract TSelf FromString(string value, int nativeCharByteCount);
			ConstVoidPtr Pointer { get; }
			int NativeCharByteCount { get; }
			int CharBufferSize { get; }
		}

		internal sealed unsafe class SDLNativeWideString : ISDLNativeWideString<SDLNativeWideString>
		{
			private IntPtr buffer;
			private int nativeCharByteCount;
			private int charBufferSize;

			public ConstVoidPtr Pointer
			{
				get
				{
					ObjectDisposedException.ThrowIf(buffer == IntPtr.Zero, this);
					return new ConstVoidPtr((vptr)buffer);
				}
			}

			public int NativeCharByteCount
			{
				get
				{
					return nativeCharByteCount;
				}
			}

			public int CharBufferSize
			{
				get
				{
					return charBufferSize;
				}
			}

			private SDLNativeWideString(IntPtr buffer, int nativeCharByteCount, int charBufferSize)
			{
				this.buffer = buffer;
				this.nativeCharByteCount = nativeCharByteCount;
				this.charBufferSize = charBufferSize;
			}

			public static SDLNativeWideString FromString(string value, int nativeCharByteCount)
			{
				ArgumentNullException.ThrowIfNull(value);

				IntPtr buffer = SDLNativeWideText.AllocateStringBuffer(value, nativeCharByteCount);

				return new SDLNativeWideString(buffer, nativeCharByteCount, checked(SDLNativeWideText.GetNativeCharacterLength(value, nativeCharByteCount) + 1));
			}

			internal static ptr AllocateString(string? value, int nativeCharByteCount, ref List<IntPtr>? ownedBuffers)
			{
				if (value == null) {
					return 0;
				}

				IntPtr buffer = SDLNativeWideText.AllocateStringBuffer(value, nativeCharByteCount);

				if (ownedBuffers == null) {
					ownedBuffers = new List<IntPtr>();
				}

				ownedBuffers.Add(buffer);

				return (ptr)buffer;
			}

			public void Dispose()
			{
				if (buffer == IntPtr.Zero) {
					return;
				}

				Marshal.FreeCoTaskMem(buffer);
				buffer = IntPtr.Zero;
			}
		}

		internal sealed unsafe class SDLNativeWideBuffer : IDisposable
		{
			private IntPtr buffer;
			private readonly int nativeCharByteCount;
			private int charBufferSize;

			internal int CharBufferSize
			{
				get
				{
					return charBufferSize;
				}
			}

			internal VoidPtr Pointer
			{
				get
				{
					ObjectDisposedException.ThrowIf(buffer == IntPtr.Zero, this);
					return new VoidPtr((vptr)buffer);
				}
			}

			internal SDLNativeWideBuffer(int charBufferSize, int nativeCharByteCount)
			{
				if (charBufferSize <= 0) {
					throw new ArgumentOutOfRangeException(nameof(charBufferSize), "The native wide-character buffer must contain at least one element.");
				}
				SDLNativeWideText.ValidateNativeCharacterByteCount(nativeCharByteCount);

				this.charBufferSize = charBufferSize;
				this.nativeCharByteCount = nativeCharByteCount;

				int byteCount = checked(charBufferSize * nativeCharByteCount);
				buffer = Marshal.AllocCoTaskMem(byteCount);
				new Span<byte>((vptr)buffer, byteCount).Clear();
			}

			internal SDLWideString

		}
	}
}