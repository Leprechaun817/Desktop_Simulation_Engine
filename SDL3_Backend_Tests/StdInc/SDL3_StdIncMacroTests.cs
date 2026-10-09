using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using SDL3;
using SDL3.Unsafe;
using Xunit;
using static SDL3.SDL3_StdIncMacros;

namespace SDL3_Backend_Tests
{
	namespace StdIncMacros
	{
		public sealed unsafe class SDLStdIncMacroTests
		{
			[Fact]
			public void IntegerLimits_ShouldMatchTheirCSharpWidths()
			{
				Assert.Equal(sbyte.MinValue, SDL_MIN_SINT8);
				Assert.Equal(sbyte.MaxValue, SDL_MAX_SINT8);
				Assert.Equal(byte.MinValue, SDL_MIN_UINT8);
				Assert.Equal(byte.MaxValue, SDL_MAX_UINT8);
				Assert.Equal(short.MinValue, SDL_MIN_SINT16);
				Assert.Equal(short.MaxValue, SDL_MAX_SINT16);
				Assert.Equal(ushort.MinValue, SDL_MIN_UINT16);
				Assert.Equal(ushort.MaxValue, SDL_MAX_UINT16);
				Assert.Equal(int.MinValue, SDL_MIN_SINT32);
				Assert.Equal(int.MaxValue, SDL_MAX_SINT32);
				Assert.Equal(uint.MinValue, SDL_MIN_UINT32);
				Assert.Equal(uint.MaxValue, SDL_MAX_UINT32);
				Assert.Equal(long.MinValue, SDL_MIN_SINT64);
				Assert.Equal(long.MaxValue, SDL_MAX_SINT64);
				Assert.Equal(ulong.MinValue, SDL_MIN_UINT64);
				Assert.Equal(ulong.MaxValue, SDL_MAX_UINT64);
				Assert.Equal(nuint.MaxValue, SDL_SIZE_MAX);
				Assert.Equal(long.MinValue, SDL_MIN_TIME);
				Assert.Equal(long.MaxValue, SDL_MAX_TIME);
			}

			[Fact]
			public void FloatingPointConstants_ShouldMatchPiAndTheSpacingAboveOne()
			{
				Assert.Equal(Math.PI, SDL_PI_D);
				Assert.Equal(MathF.PI, SDL_PI_F);
				Assert.Equal(MathF.BitIncrement(1.0f) - 1.0f, SDL_FLT_EPSILON);
			}

			[Fact]
			public void SentinelConstants_ShouldRetainNativeUnsignedBitPatterns()
			{
				Assert.Equal(0xFFFDu, SDL_INVALID_UNICODE_CODEPOINT);
				Assert.Equal(nuint.MaxValue, SDL_ICONV_ERROR);
				Assert.Equal(nuint.MaxValue - 1, SDL_ICONV_E2BIG);
				Assert.Equal(nuint.MaxValue - 2, SDL_ICONV_EILSEQ);
				Assert.Equal(nuint.MaxValue - 3, SDL_ICONV_EINVAL);
			}

			[Fact]
			public void PrintfFragments_ShouldMatchSDLIntegerFormats()
			{
				Assert.Equal(new string[] { "lld", "llu", "llx", "llX" }, new string[] { SDL_PRIs64, SDL_PRIu64, SDL_PRIx64, SDL_PRIX64 });
				Assert.Equal(new string[] { "d", "u", "x", "X" }, new string[] { SDL_PRIs32, SDL_PRIu32, SDL_PRIx32, SDL_PRIX32 });
				Assert.Equal("ll", SDL_PRILL_PREFIX);
				Assert.Equal(new string[] { "lld", "llu", "llx", "llX" }, new string[] { SDL_PRILLd, SDL_PRILLu, SDL_PRILLx, SDL_PRILLX });
			}

			[Theory]
			[InlineData(0, 0, 0, 0, 0u)]
			[InlineData(65, 66, 67, 68, 0x44434241u)]
			[InlineData(255, 128, 1, 254, 0xFE0180FFu)]
			public void FourCC_ShouldPackBytesFromLeastToMostSignificant(int a, int b, int c, int d, uint expected)
			{
				Assert.Equal(expected, SDL_FOURCC((byte)a, (byte)b, (byte)c, (byte)d));
			}

			[Fact]
			public void ArraySize_ShouldCountElementsAndRejectNullArrays()
			{
				long[] values = new long[3];
				Assert.Equal(3, SDL_arraysize(values));
				Assert.Equal(2, SDL_arraysize<long>(values.AsSpan(1)));
				Assert.Equal(0, SDL_arraysize(Array.Empty<int>()));
				Assert.Equal(0, SDL_arraysize(ReadOnlySpan<int>.Empty));
				Assert.Throws<ArgumentNullException>(() =>
					SDL_arraysize<int>((int[])null!)
				);
			}

			[Theory]
			[InlineData(-5, 0, 10, 0)]
			[InlineData(0, 0, 10, 0)]
			[InlineData(6, 0, 10, 6)]
			[InlineData(10, 0, 10, 10)]
			[InlineData(15, 0, 10, 10)]
			[InlineData(5, 3, 3, 3)]
			public void Clamp_ShouldRespectBothInclusiveBounds(int value, int minimum, int maximum, int expected)
			{
				Assert.Equal(expected, SDL_clamp(value, minimum, maximum));
				Assert.Equal(Math.Min(value, minimum), SDL_min(value, minimum));
				Assert.Equal(Math.Max(value, maximum), SDL_max(value, maximum));
			}

			[Fact]
			public void GenericMacros_ShouldSupportSDLSizeAndFloatingPointValues()
			{
				Assert.Equal(new SDLSize(2), SDL_min(new SDLSize(2), new SDLSize(9)));
				Assert.Equal(new SDLSize(9), SDL_max(new SDLSize(2), new SDLSize(9)));
				Assert.Equal(new SDLSize(5), SDL_clamp(new SDLSize(9), SDLSize.One, new SDLSize(5)));
				Assert.Equal(-1.5, SDL_min(-1.5, 2.5));
				Assert.Equal(2.5, SDL_max(-1.5, 2.5));
				Assert.True(double.IsNaN(SDL_clamp(double.NaN, 0.0, 1.0)));
			}

			[Fact]
			public void CopyPointer_ShouldCopyOneUnmanagedValue()
			{
				MacroValue source = new MacroValue() { Tag = 0xA5, Number = 123456, Flags = 0x1234 };
				MacroValue destination = default;
				SDL_copyp(&destination, &source);
				Assert.Equal(source.Tag, destination.Tag);
				Assert.Equal(source.Number, destination.Number);
				Assert.Equal(source.Flags, destination.Flags);
				destination.Number = 1;
				Assert.Equal(123456, source.Number);
			}

			[Fact]
			public void PointerMacros_ShouldRejectNullPointers()
			{
				Assert.Throws<ArgumentNullException>(() =>
					SDL_copyp<int>(null, (int*)1)
				);
				Assert.Throws<ArgumentNullException>(() =>
					SDL_copyp<int>((int*)1, null)
				);
				Assert.Throws<ArgumentNullException>(() =>
					SDL_zerop<int>(null)
				);
			}

			[Fact]
			public void ZeroedMacros_ShouldClearEveryByteIncludingStructPadding()
			{
				MacroValue value = default;
				Span<byte> bytes = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref value, 1));
				bytes.Fill(0xFF);
				SDL_zero(ref value);
				Assert.All(bytes.ToArray(), item => Assert.Equal((byte)0, item));

				MacroValue* mvPointer = stackalloc MacroValue[1];
				Span<byte> mvPointerBytes = new Span<byte>(mvPointer, sizeof(MacroValue));
				mvPointerBytes.Fill(0xFF);
				SDL_zerop(mvPointer);
				Assert.All(mvPointerBytes.ToArray(), item => Assert.Equal((byte)0, item));

				int[] values = { 1, 2, 3, 4 };
				SDL_zeroa(values.AsSpan(1, 2));
				Assert.Equal(new[] { 1, 0, 0, 4 }, values);
				SDL_zeroa(Span<int>.Empty);
			}

			[Fact]
			public void SizeOverflowManagedHelpers_ShouldReturnresultsOrClearThemOnOverflow()
			{
				Assert.True(SDL_size_mul_check_overflow(new SDLSize(3), new SDLSize(7), out SDLSize product));
				Assert.Equal(new SDLSize(21), product);
				Assert.True(SDL_size_mul_check_overflow(SDLSize.Zero, SDLSize.MaxValue, out product));
				Assert.Equal(SDLSize.Zero, product);
				Assert.True(SDL_size_mul_check_overflow(SDLSize.MaxValue, SDLSize.One, out product));
				Assert.Equal(SDLSize.MaxValue, product);
				Assert.False(SDL_size_mul_check_overflow(SDLSize.MaxValue, new SDLSize(2), out product));
				Assert.Equal(SDLSize.Zero, product);
				Assert.True(SDL_size_add_check_overflow(SDLSize.MaxValue - SDLSize.One, SDLSize.One, out SDLSize sum));
				Assert.Equal(SDLSize.MaxValue, sum);
				Assert.False(SDL_size_add_check_overflow(SDLSize.MaxValue, SDLSize.One, out sum));
				Assert.Equal(SDLSize.Zero, sum);
			}

			[Fact]
			public void SizeOverflowPointerHelpers_ShouldLeaveOutputUntouchedOnOverflow()
			{
				nuint product = 999;
				Assert.False(SDL_size_mul_check_overflow(nuint.MaxValue, 2, &product));
				Assert.Equal((nuint)999, product);
				Assert.True(SDL_size_mul_check_overflow(4, 5, &product));
				Assert.Equal((nuint)20, product);
				Assert.True(SDL_size_mul_check_overflow(0, nuint.MaxValue, &product));
				Assert.Equal((nuint)0, product);

				nuint sum = 123;
				Assert.False(SDL_size_add_check_overflow(nuint.MaxValue, 1, &sum));
				Assert.Equal((nuint)123, sum);
				Assert.True(SDL_size_add_check_overflow(nuint.MaxValue - 1, 1, &sum));
				Assert.Equal(nuint.MaxValue, sum);
				Assert.Throws<ArgumentNullException>(() =>
					SDL_size_mul_check_overflow(1, 2, (nuint*)null)
				);
				Assert.Throws<ArgumentNullException>(() =>
					SDL_size_add_check_overflow(1, 2, (nuint*)null)
				);
			}

			[StructLayout(LayoutKind.Sequential)]
			private struct MacroValue
			{
				public byte Tag;
				public int Number;
				public ushort Flags;
			}
		}
	}
}