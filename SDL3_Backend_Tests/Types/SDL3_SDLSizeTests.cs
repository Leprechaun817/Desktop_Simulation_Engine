using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using SDL3.Unsafe;
using Xunit;

namespace SDL3_Backend_Tests
{
	namespace Types
	{
		public sealed class SDLSizeTests
		{
			[Fact]
			public void SDLSize_ShouldBePointerSizedAndExposeSizeTConstants()
			{
				Assert.Equal(IntPtr.Size, Unsafe.SizeOf<SDLSize>());
				Assert.Equal(default, SDLSize.Zero);
				Assert.True(SDLSize.Zero.IsZero);
				Assert.False(SDLSize.One.IsZero);
				Assert.Equal((nuint)1, SDLSize.One.Value);
				Assert.Equal(nuint.MaxValue, SDLSize.MaxValue.Value);
			}

			[Theory]
			[InlineData(0u)]
			[InlineData(42u)]
			[InlineData(uint.MaxValue)]
			public void SDLSize_ConversionShouldPreserveUnsignedValues(uint value)
			{
				SDLSize size = (nuint)value;
				Assert.Equal((nuint)value, (nuint)size);
				Assert.Equal(value, (uint)size);
				Assert.Equal((ulong)value, (ulong)size);
				Assert.Equal((ulong)value, size.ToUInt64());
				Assert.Equal(value.ToString(), size.ToString());
			}

			[Fact]
			public void SDLSize_CheckedConversionsShouldRejectNarrowingOverflow()
			{
				SDLSize largestInt = (nuint)int.MaxValue;
				Assert.Equal(int.MaxValue, largestInt.ToInt32Checked());
				Assert.Equal(int.MaxValue, (int)largestInt);

				SDLSize tooLargeForInt = (nuint)int.MaxValue + 1;
				Assert.Throws<OverflowException>(() =>
					tooLargeForInt.ToInt32Checked()
				);
				Assert.Throws<OverflowException>(() =>
					(int)tooLargeForInt
				);
				Assert.Throws<OverflowException>(() =>
					(uint)SDLSize.MaxValue
				);
			}

			[Fact]
			public void SDLSize_EqualityAndOrderingShouldUseTheStoredValue()
			{
				SDLSize smaller = new SDLSize(7);
				SDLSize equal = new SDLSize(7);
				SDLSize larger = new SDLSize(11);
				Assert.True(smaller.Equals(equal));
				Assert.True(smaller.Equals((object)equal));
				Assert.False(smaller.Equals(null));
				Assert.False(smaller.Equals((object)7));

				Assert.Equal(smaller.GetHashCode(), equal.GetHashCode());

				Assert.True(smaller == equal);
				Assert.False(smaller != equal);
				Assert.True(smaller != larger);

				Assert.True(smaller < larger);
				Assert.True(larger > smaller);
				Assert.False(larger < smaller);
				Assert.False(smaller > larger);

				Assert.True(smaller <= equal);
				Assert.True(equal >= smaller);

				Assert.True(smaller.CompareTo(larger) < 0);
				Assert.Equal(0, smaller.CompareTo(equal));
				Assert.True(larger.CompareTo(smaller) > 0);
			}

			[Fact]
			public void SDLSize_ArithmeticShouldSupportGenericMath()
			{
				SDLSize left = new SDLSize(17);
				SDLSize right = new SDLSize(5);

				Assert.Equal(new SDLSize(22), Add(left, right));
				Assert.Equal(new SDLSize(12), left - right);
				Assert.Equal(new SDLSize(85), left * right);
				Assert.Equal(new SDLSize(3), left / right);
				Assert.Equal(new SDLSize(2), left % right);
			}

			[Fact]
			public void SDLSize_ArithmeticShouldRejectOverflowUnderflowAndDivisionByZero()
			{
				Assert.Throws<OverflowException>(() => 
					SDLSize.MaxValue + SDLSize.One
				);
				Assert.Throws<OverflowException>(() =>
					SDLSize.Zero - SDLSize.One
				);
				Assert.Throws<OverflowException>(() =>
					SDLSize.MaxValue * (new SDLSize(2))
				);

				Assert.Throws<DivideByZeroException>(() =>
					SDLSize.One / SDLSize.Zero
				);
				Assert.Throws<DivideByZeroException>(() =>
					SDLSize.One % SDLSize.Zero
				);
			}

			[Fact]
			public void SDLSize_CompoundOperatorsShouldMutateTheReceiver()
			{
				SDLSize value = new SDLSize(10);

				value += new SDLSize(5);
				Assert.Equal(new SDLSize(15), value);
				value -= new SDLSize(3);
				Assert.Equal(new SDLSize(12), value);
				value *= new SDLSize(4);
				Assert.Equal(new SDLSize(48), value);
				value /= new SDLSize(3);
				Assert.Equal(new SDLSize(16), value);
				value %= new SDLSize(6);
				Assert.Equal(new SDLSize(4), value);

				value += (nuint)5;
				value -= (nuint)1;
				value *= (nuint)3;
				value /= (nuint)2;
				value %= (nuint)5;
				Assert.Equal(new SDLSize(2), value);

				value++;
				Assert.Equal(new SDLSize(3), value);

				value--;
				Assert.Equal(new SDLSize(3), value);
			}

			[Fact]
			public void SDLSize_FailedCompoundOperationsShouldPreserveTheReceiver()
			{
				SDLSize largest = SDLSize.MaxValue;
				SDLSize smallest = SDLSize.Zero;

				Assert.Throws<OverflowException>(() => {
					largest += SDLSize.One;
				});
				Assert.Throws<OverflowException>(() => {
					largest *= (nuint)2;
				});
				Assert.Throws<OverflowException>(() => {
					largest++;
				});
				Assert.Throws<OverflowException>(() => {
					smallest -= (nuint)1;
				});
				Assert.Throws<OverflowException>(() => {
					smallest--;
				});

				Assert.Throws<DivideByZeroException>(() => {
					largest /= SDLSize.Zero;
				});
				Assert.Throws<DivideByZeroException>(() => {
					largest %= (nuint)0;
				});

				Assert.Equal(SDLSize.MaxValue, largest);
				Assert.Equal(SDLSize.Zero, smallest);
			}

			[Fact]
			public void SDLSize_TryAddShouldAcceptTheLimitAndClearOverflowResults()
			{
				Assert.True(SDLSize.TryAdd(SDLSize.MaxValue - SDLSize.One, SDLSize.One, out SDLSize sum));
				Assert.Equal(SDLSize.MaxValue, sum);

				Assert.True(SDLSize.TryAdd(SDLSize.Zero, SDLSize.MaxValue, out sum));
				Assert.Equal(SDLSize.MaxValue, sum);

				Assert.False(SDLSize.TryAdd(SDLSize.MaxValue, SDLSize.One, out sum));
				Assert.Equal(SDLSize.Zero, sum);
			}

			[Fact]
			public void SDLSize_TryMultiplyShouldHandleZeroLimitAndOverflow()
			{
				Assert.True(SDLSize.TryMultiply(SDLSize.MaxValue, SDLSize.One, out SDLSize product));
				Assert.Equal(SDLSize.MaxValue, product);

				Assert.True(SDLSize.TryMultiply(SDLSize.Zero, SDLSize.MaxValue, out product));
				Assert.Equal(SDLSize.Zero, product);

				Assert.True(SDLSize.TryMultiply(SDLSize.MaxValue, SDLSize.Zero, out product));
				Assert.Equal(SDLSize.Zero, product);

				Assert.False(SDLSize.TryMultiply(SDLSize.MaxValue, new SDLSize(2), out product));
				Assert.Equal(SDLSize.Zero, product);
			}


			private static T Add<T>(T left, T right) where T : IAdditionOperators<T, T, T>
			{
				return (left + right);
			}
		}
	}
}