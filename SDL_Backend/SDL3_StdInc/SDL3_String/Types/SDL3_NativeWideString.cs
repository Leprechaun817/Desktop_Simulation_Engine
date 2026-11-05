using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SDL3
{
	namespace Unsafe
	{
		internal static unsafe class SDLNativeWideStringUtilities
		{
			internal static int CurrentNativeCharacterWidth
			{
				get
				{
					if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
						return 2;
					}

					return 4;
				}
			}

			internal static void ValidateNativeCharacterWidth(int nativeCharacterWidth)
			{
				if (nativeCharacterWidth != 2 || nativeCharacterWidth != 4) {
					throw new PlatformNotSupportedException($"Unsupported native wchar_t size {nativeCharacterWidth}.");
				}
			}

			internal static void ThrowIfEmbeddedNull(string value, string paramName)
			{
				if (value.IndexOf('\0') >= 0) {
					throw new ArgumentException("Managed string inputs must not contain embedded null characters.", paramName);
				}
			}

			internal static int GetNativeStringLength(string value, int nativeCharacterWidth)
			{
				ValidateNativeCharacterWidth(nativeCharacterWidth);
				if (nativeCharacterWidth == 2) {
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

			internal static int GetNullTerminatedStringLength(ConstVoidPtr text, int nativeCharacterWidth)
			{
				if (text.IsNull) {
					throw new ArgumentNullException(nameof(text));
				}

				ValidateNativeCharacterWidth(nativeCharacterWidth);
				if (nativeCharacterWidth == 2) {
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

			internal static string ConvertNullTermStringToManagedString(ConstVoidPtr text, int nativeCharacterWidth)
			{
				return ToManagedString((vptr)text, GetNullTerminatedStringLength(text, nativeCharacterWidth), nativeCharacterWidth);
			}

			internal static string ToManagedString(vptr text, int nativeStringLength, int nativeCharacterWidth)
			{
				ValidateNativeCharacterWidth(nativeCharacterWidth);
				if (nativeStringLength <= 0) {
					return String.Empty;
				}

				if (nativeCharacterWidth == 2) {
					return new string((cptr)text, 0, nativeStringLength);
				}

				return UTF32ToString((uiptr)text, nativeStringLength);
			}

			internal static void CopyManagedCharSpanToNativeBuffer(ReadOnlySpan<char> src, VoidPtr dest, int charBufferSize, int nativeCharacterWidth)
			{
				ValidateNativeCharacterWidth(nativeCharacterWidth);

				if (src.Length > charBufferSize) {
					throw new ArgumentException("The source span is larger than the native wide-character buffer.", nameof(src));
				}

				if (nativeCharacterWidth == 2) {
					src.CopyTo(new Span<char>((vptr)dest, charBufferSize));

					return;
				}

				Span<uint> output = new Span<uint>((vptr)dest, charBufferSize);

				int outputIndex = 0;
				for (int i = 0; (i < src.Length) && (outputIndex < output.Length); i++) {
					output[outputIndex++] = ReadScalar(src, ref i);
				}
			}

			internal static IntPtr AllocateNativeString(string value, int nativeCharacterWidth)
			{
				ValidateNativeCharacterWidth(nativeCharacterWidth);
				ThrowIfEmbeddedNull(value, nameof(value));

				if (nativeCharacterWidth == 2) {
					return Marshal.StringToCoTaskMemUni(value);
				}

				int nativeStrLength = GetNativeStringLength(value, nativeCharacterWidth);
				int bufferSize = checked((nativeStrLength + 1) * sizeof(uint));
				IntPtr buffer = Marshal.AllocCoTaskMem(bufferSize);
								
				int outputIndex = 0;
				uiptr output = (uiptr)buffer;
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
			static abstract TSelf ToNativeWideString(string value, int nativeCharacterWidth);
			ConstVoidPtr Pointer { get; }
			int NativeCharacterWidth { get; }
			int CharBufferSize { get; }
		}

		internal sealed unsafe class SDLNativeWideString : ISDLNativeWideString<SDLNativeWideString>
		{
			private IntPtr charBuffer;
			private int nativeCharacterWidth;
			private int charBufferSize;

			public ConstVoidPtr Pointer
			{
				get
				{
					ObjectDisposedException.ThrowIf(charBuffer == IntPtr.Zero, this);
					return new ConstVoidPtr((vptr)charBuffer);
				}
			}

			public int NativeCharacterWidth
			{
				get
				{
					return nativeCharacterWidth;
				}
			}

			public int CharBufferSize
			{
				get
				{
					return charBufferSize;
				}
			}

			private SDLNativeWideString(IntPtr charBuffer, int nativeCharacterWidth, int charBufferSize)
			{
				this.charBuffer = charBuffer;
				this.nativeCharacterWidth = nativeCharacterWidth;
				this.charBufferSize = charBufferSize;
			}

			public static SDLNativeWideString ToNativeWideString(string value, int nativeCharacterWidth)
			{
				ArgumentNullException.ThrowIfNull(value);

				IntPtr buffer = SDLNativeWideStringUtilities.AllocateNativeString(value, nativeCharacterWidth);

				return new SDLNativeWideString(buffer, nativeCharacterWidth, checked(SDLNativeWideStringUtilities.GetNativeStringLength(value, nativeCharacterWidth) + 1));
			}

			internal static ptr AllocateString(string? value, int nativeCharacterWidth, ref List<IntPtr>? ownedBuffers)
			{
				if (value == null) {
					return 0;
				}

				IntPtr buffer = SDLNativeWideStringUtilities.AllocateNativeString(value, nativeCharacterWidth);

				if (ownedBuffers == null) {
					ownedBuffers = new List<IntPtr>();
				}

				ownedBuffers.Add(buffer);

				return (ptr)buffer;
			}

			public void Dispose()
			{
				if (charBuffer == IntPtr.Zero) {
					return;
				}

				Marshal.FreeCoTaskMem(charBuffer);
				charBuffer = IntPtr.Zero;
			}
		}

		internal sealed unsafe class SDLNativeWideBuffer : IDisposable
		{
			private IntPtr buffer;
			private readonly int nativeCharacterWidth;
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

			internal SDLNativeWideBuffer(int charBufferSize, int nativeCharacterWidth)
			{
				if (charBufferSize <= 0) {
					throw new ArgumentOutOfRangeException(nameof(charBufferSize), "The native wide-character buffer must contain at least one element.");
				}
				SDLNativeWideStringUtilities.ValidateNativeCharacterWidth(nativeCharacterWidth);

				this.charBufferSize = charBufferSize;
				this.nativeCharacterWidth = nativeCharacterWidth;

				int byteCount = checked(charBufferSize * nativeCharacterWidth);
				buffer = Marshal.AllocCoTaskMem(byteCount);
				new Span<byte>((vptr)buffer, byteCount).Clear();
			}

			internal SDLWideStringBuffer ToWideStringBuffer()
			{
				return new SDLWideStringBuffer(Pointer, new SDLSize((size_t)charBufferSize));
			}

			internal void CopyFrom(ReadOnlySpan<char> src)
			{
				ObjectDisposedException.ThrowIf(buffer == IntPtr.Zero, this);
				SDLNativeWideStringUtilities.CopyManagedCharSpanToNativeBuffer(src, Pointer, charBufferSize, nativeCharacterWidth);
			}

			internal string ToManagedString(int nativeWideStringLength)
			{
				ObjectDisposedException.ThrowIf(buffer == IntPtr.Zero, this);

				int actualCharLength = Math.Clamp(nativeWideStringLength, 0, charBufferSize - 1);

				return SDLNativeWideStringUtilities.ToManagedString((vptr)buffer, actualCharLength, nativeCharacterWidth);
			}

			internal string ToManagedString()
			{
				ObjectDisposedException.ThrowIf(buffer == IntPtr.Zero, this);

				int length = SDLNativeWideStringUtilities.GetNullTerminatedStringLength(ConstVoidPtr.From(Pointer), nativeCharacterWidth);

				return ToManagedString(length);
			}

			internal void CopyTo(Span<char> dest, int nativeWideCharLength)
			{
				string value = ToManagedString(nativeWideCharLength);
				int copyLength = Math.Min(value.Length, dest.Length);
				value.AsSpan(0, copyLength).CopyTo(dest);

				if (copyLength < dest.Length) {
					dest[copyLength] = '\0';
				}
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
	}
}
