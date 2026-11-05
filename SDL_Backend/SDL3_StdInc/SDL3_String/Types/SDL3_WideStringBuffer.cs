using System;
using SDL3.Unsafe;

namespace SDL3
{
	/// <summary>
	///	Caller-owned, native wchar_t container for SDL wide string calls.
	///	Stores the native pointer to the wide-string buffer and serves as
	///	the interface between the native and managed layers of this library
	/// </summary>
	public readonly unsafe struct SDLWideStringBuffer
	{
		private readonly VoidPtr wstringBufferPtr;
		private readonly SDLSize wstringBufferSize;

		public VoidPtr Pointer
		{
			get
			{
				return wstringBufferPtr;
			}
		}

		public SDLSize BufferSize
		{
			get
			{
				return wstringBufferSize;
			}
		}

		public int NativeCharacterWidth
		{
			get
			{
				return SDLNativeWideStringUtilities.CurrentNativeCharacterWidth;
			}
		}

		public SDLWideStringBuffer(VoidPtr wideStrPtr, SDLSize wideStrBufferSize)
		{
			if (wideStrPtr.IsNull) {
				throw new ArgumentException("The native wide-character buffer pointer must not be null.", nameof(wideStrPtr));
			}

			if (wideStrBufferSize.IsZero) {
				throw new ArgumentOutOfRangeException(nameof(wideStrBufferSize), "The native wide-string character buffer must contain at least one element");
			}

			wstringBufferPtr = wideStrPtr;
			wstringBufferSize = wideStrBufferSize;
		}
	}
}
