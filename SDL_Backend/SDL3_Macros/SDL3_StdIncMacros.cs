using SDL3.Unsafe;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SDL3
{
	/// <summary>
	///	C# equivalents for public SDL_stdinc.h macros that are useful in C#,
	///	but are not imported from SDL3.dll because they are preprocessor values
	///	that cannot be exported like other runtime symbols. Definitions are
	///	purposely kept close to their original C macro spellings so they remain
	///	easy to cross-check.
	/// </summary>
	public static class SDL3_StdIncMacros
	{
		// ---------------------------------------------------------------------
		// size_t limits
		// ---------------------------------------------------------------------

		/// <summary>
		/// C# equivalent of SDL_SIZE_MAX.
		/// </summary>
		public static nuint SDL_SIZE_MAX
		{
			get
			{
				return unchecked((nuint)(-1));
			}
		}

		// ---------------------------------------------------------------------
		// Integer limits
		// ---------------------------------------------------------------------

		public const sbyte SDL_MAX_SINT8 = (sbyte)0x7F;
		public const sbyte SDL_MIN_SINT8 = (sbyte)(~0x7F);

		public const byte SDL_MAX_UINT8 = (byte)0xFFu;
		public const byte SDL_MIN_UINT8 = (byte)0x00u;

		public const short SDL_MAX_SINT16 = (short)0x7FFF;
		public const short SDL_MIN_SINT16 = (short)(~0x7FFF);

		public const ushort SDL_MAX_UINT16 = (ushort)0xFFFFu;
		public const ushort SDL_MIN_UINT16 = (ushort)0x0000u;

		public const int SDL_MAX_SINT32 = 0x7FFFFFFF;
		public const int SDL_MIN_SINT32 = ~0x7FFFFFFF;

		public const uint SDL_MAX_UINT32 = 0xFFFFFFFFu;
		public const uint SDL_MIN_UINT32 = 0x00000000u;

		public const long SDL_MAX_SINT64 = 0x7FFFFFFFFFFFFFFFL;
		public const long SDL_MIN_SINT64 = ~0x7FFFFFFFFFFFFFFFL;

		public const ulong SDL_MAX_UINT64 = 0xFFFFFFFFFFFFFFFFul;
		public const ulong SDL_MIN_UINT64 = 0x0000000000000000ul;

		// ---------------------------------------------------------------------
		// Time and floating-point constants
		// ---------------------------------------------------------------------

		public const long SDL_MAX_TIME = SDL_MAX_SINT64;
		public const long SDL_MIN_TIME = SDL_MIN_SINT64;

		/// <summary>
		///	C# equivalent to SDL_FLT_EPSILON
		/// </summary>
		public const float SDL_FLT_EPSILON = 1.1920928955078125e-07f;

		public const double SDL_PI_D = 3.141592653589793238462643383279502884d;
		public const float SDL_PI_F = 3.141592653589793238462643383279502884f;

		// ---------------------------------------------------------------------
		// printf format fragment macros
		// ---------------------------------------------------------------------

		public const string SDL_PRIs64 = "lld";
		public const string SDL_PRIu64 = "llu";
		public const string SDL_PRIx64 = "llx";
		public const string SDL_PRIX64 = "llX";

		public const string SDL_PRIs32 = "d";
		public const string SDL_PRIu32 = "u";
		public const string SDL_PRIx32 = "x";
		public const string SDL_PRIX32 = "X";

		public const string SDL_PRILL_PREFIX = "ll";
		public const string SDL_PRILLd = SDL_PRILL_PREFIX + "d";
		public const string SDL_PRILLu = SDL_PRILL_PREFIX + "u";
		public const string SDL_PRILLx = SDL_PRILL_PREFIX + "x";
		public const string SDL_PRILLX = SDL_PRILL_PREFIX + "X";

		// ---------------------------------------------------------------------
		// Unicode and iconv sentinel macros
		// ---------------------------------------------------------------------

		public const uint SDL_INVALID_UNICODE_CODEPOINT = 0xFFFDu;

		public static nuint SDL_ICONV_ERROR
		{
			get
			{
				return unchecked((nuint)(-1));
			}
		}

		public static nuint SDL_ICONV_E2BIG
		{
			get
			{
				return unchecked((nuint)(-2));
			}
		}

		public static nuint SDL_ICONV_EILSEQ
		{
			get
			{
				return unchecked((nuint)(-3));
			}
		}

		public static nuint SDL_ICONV_EINVAL
		{
			get
			{
				return unchecked((nuint)(-4));
			}
		}

		// ---------------------------------------------------------------------
		// Public function-like helper macros
		// ---------------------------------------------------------------------

		/// <summary>
		/// C# equivalent of SDL_FOURCC(A, B, C, D).
		/// </summary>
		public static uint SDL_FOURCC(byte a, byte b, byte c, byte d)
		{
			return (uint)a | ((uint)b << 8) | ((uint)c << 16) | ((uint)d << 24);
		}

		/// <summary>
		/// C# helper equivalent to SDL_arraysize(array).
		/// </summary>
		public static int SDL_arraysize<T>(T[] array)
		{
			ArgumentNullException.ThrowIfNull(array);

			return array.Length;
		}

		/// <summary>
		/// C# helper equivalent to SDL_arraysize(array) but specifically for C# span types.
		/// </summary>
		public static int SDL_arraysize<T>(ReadOnlySpan<T> array)
		{
			return array.Length;
		}

		/// <summary>
		/// C# equivalent of SDL_min(x, y).
		/// </summary>
		public static T SDL_min<T>(T x, T y) where T : IComparisonOperators<T, T, bool>
		{
			if (x < y) {
				return x;
			}

			return y;
		}

		/// <summary>
		/// C# equivalent of SDL_max(x, y).
		/// </summary>
		public static T SDL_max<T>(T x, T y) where T : IComparisonOperators<T, T, bool>
		{
			if (x > y) {
				return x;
			}

			return y;
		}

		/// <summary>
		/// C# equivalent of SDL_clamp(x, a, b).
		/// </summary>
		public static T SDL_clamp<T>(T x, T a, T b) where T : IComparisonOperators<T, T, bool>
		{
			if (x < a) {
				return a;
			}
			else {
				if (x > b) {
					return b;
				}
			}

			return x;
		}

		/// <summary>
		/// C# equivalent of SDL_copyp(dst, src).
		/// </summary>
		public static unsafe void SDL_copyp<T>(T* dst, T* src) where T : unmanaged
		{
			if (dst == null) {
				throw new ArgumentNullException(nameof(dst));
			}
			if (src == null) {
				throw new ArgumentNullException(nameof(src));
			}

			*dst = *src;
		}

		/// <summary>
		/// C# equivalent of SDL_zero(x).
		/// </summary>
		public static void SDL_zero<T>(ref T value) where T : unmanaged
		{
			MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref value, 1)).Clear();
		}

		/// <summary>
		/// C# equivalent of SDL_zerop(x).
		/// </summary>
		public static unsafe void SDL_zerop<T>(T* value) where T : unmanaged
		{
			if (value == null) {
				throw new ArgumentNullException(nameof(value));
			}

			RuntimeUnsafe.InitBlockUnaligned(value, 0, (uint)sizeof(T));
		}

		/// <summary>
		/// C# equivalent of SDL_zeroa(x).
		/// </summary>
		public static void SDL_zeroa<T>(Span<T> values) where T : unmanaged
		{
			values.Clear();
		}

		/// <summary>
		/// C# equivalent of SDL_size_mul_check_overflow(a, b, ret).
		/// Returns true when the multiplication fits in size_t.
		/// </summary>
		public static bool SDL_size_mul_check_overflow(SDLSize a, SDLSize b, out SDLSize ret)
		{
			if ((a.Value != 0) && b.Value > (SDL_SIZE_MAX / a.Value)) {
				ret = SDLSize.Zero;

				return false;
			}

			ret = new SDLSize(a.Value * b.Value);

			return true;
		}

		/// <summary>
		/// Unsafe C-style equivalent of SDL_size_mul_check_overflow(a, b, ret).
		/// Returns true when the multiplication fits in size_t.
		/// </summary>
		public static unsafe bool SDL_size_mul_check_overflow(size_t a, size_t b, size_t* ret)
		{
			if (ret == null) {
				throw new ArgumentNullException(nameof(ret));
			}

			if ((a != 0) && b > (SDL_SIZE_MAX / a)) {
				return false;
			}

			*ret = a * b;

			return true;
		}

		/// <summary>
		/// C# equivalent of SDL_size_add_check_overflow(a, b, ret).
		/// Returns true when the addition fits in size_t.
		/// </summary>
		public static bool SDL_size_add_check_overflow(SDLSize a, SDLSize b, out SDLSize ret)
		{
			if (b.Value > (SDL_SIZE_MAX - a.Value)) {
				ret = SDLSize.Zero;

				return false;
			}

			ret = new SDLSize(a.Value + b.Value);

			return true;
		}

		/// <summary>
		/// Unsafe C-style equivalent of SDL_size_add_check_overflow(a, b, ret).
		/// Returns true when the addition fits in size_t.
		/// </summary>
		public static unsafe bool SDL_size_add_check_overflow(size_t a, size_t b, size_t* ret)
		{
			if (ret == null) {
				throw new ArgumentNullException(nameof(ret));
			}

			if (b > (SDL_SIZE_MAX - a)) {
				return false;
			}

			*ret = a + b;

			return true;
		}
	}
}