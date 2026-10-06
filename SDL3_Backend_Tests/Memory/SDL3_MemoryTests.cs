using System;
using SDL3;
using SDL3.Unsafe;
using Xunit;

namespace SDL3_Backend_Tests
{
	namespace Memory
	{
		[Collection(SDLRuntimeTestCollection.Name)]
		public sealed class SDLMemoryTests
		{
			[Fact]
			public void PublicAllocate_ShouldReturnWritableOwnedMemoryAndReleaseIt()
			{
				using SDLMemoryBlock block = SDLMemory.Allocate((nuint)32);
				Assert.False(block.IsInvalid);
				Assert.False(block.IsReleased);
				Assert.False(block.IsAligned);
				Assert.False(block.Pointer.IsAligned);
				Assert.False(block.ConstPointer.IsAligned);
				Assert.Equal(block.Pointer.Address, block.ConstPointer.Address);
				Assert.Equal(new SDLSize(32), block.ByteLength);
				Assert.Equal(new SDLSize(32), block.ElementCount);
				Assert.Equal(SDLSize.One, block.ElementSize);
				block.Span.Fill(0xA5);
				Assert.All(block.Span.ToArray(), value => Assert.Equal((byte)0xA5, value));

				block.Free();
				block.ReleaseMemory();
				AssertReleased(block);
			}

			[Theory]
			[InlineData(false)]
			[InlineData(true)]
			public void PublicCalloc_ShouldZeroTheEntireAllocationAndExposeElementShape(bool useAlias)
			{
				using (SDLMemoryBlock block = ((Func<SDLMemoryBlock>)(() => {
					if (useAlias) {
						return SDLMemory.Calloc(new SDLSize(4), new SDLSize(8));
					}

					return SDLMemory.AllocateZeroed(new SDLSize(4), new SDLSize(8));
				}))()) 
				{
					Assert.Equal(new SDLSize(32), block.ByteLength);
					Assert.Equal(new SDLSize(4), block.ElementCount);
					Assert.Equal(new SDLSize(8), block.ElementSize);
					Assert.All(block.Span.ToArray(), value => Assert.Equal((byte)0, value));
					Assert.Equal(4, block.AsSpan<ulong>().Length);
					block.AsSpan<ulong>()[2] = 0xAABBCCDDEEFF1122ul;
					Assert.Equal(0xAABBCCDDEEFF1122ul, block.Read<ulong>(new SDLSize(16)));
				}
			}

			[Theory]
			[InlineData(false)]
			[InlineData(true)]
			public void PublicTypedCalloc_ShouldUseTheUnamangedElementSize(bool useAlias)
			{
				using (SDLMemoryBlock block = ((Func<SDLMemoryBlock>)(() => {
					if (useAlias) {
						return SDLMemory.Calloc<int>(new SDLSize(5));
					}

					return SDLMemory.AllocateZeroed<int>(new SDLSize(5));
				}))()) 
				{
					Assert.Equal(new SDLSize(20), block.ByteLength);
					Assert.Equal(new SDLSize(5), block.ElementCount);
					Assert.Equal(new SDLSize(4), block.ElementSize);
					Assert.Equal(new int[5], block.AsSpan<int>().ToArray());
				}
			}

			public void PublicCalloc_ShouldRejectOverflowBeforeAllocating()
			{
				Assert.Throws<OverflowException>(() =>
					SDLMemory.AllocateZeroed(SDLSize.MaxValue, new SDLSize(2))
				);
				Assert.Throws<OverflowException>(() =>
					SDLMemory.Calloc(SDLSize.MaxValue, new SDLSize(2))
				);
				Assert.Throws<OverflowException>(() =>
					SDLMemory.AllocateZeroed<int>(SDLSize.MaxValue)
				);
				Assert.Throws<OverflowException>(() =>
					SDLMemory.Calloc<int>(SDLSize.MaxValue)
				);
			}

			[Fact]
			public void PublicReallocate_ShouldPreserveThePrefixWhenGrowingAndShrinking()
			{
				using (SDLMemoryBlock block = SDLMemory.Allocate(new SDLSize(8))) 
				{
					byte[] original = { 1, 2, 3, 4, 5, 6, 7, 8 };
					block.Write(SDLSize.Zero, original.AsSpan());
					SDLMemory.Reallocate(block, new SDLSize(32));
					Assert.Equal(new SDLSize(32), block.ByteLength);
					Assert.Equal(new SDLSize(32), block.ElementCount);
					Assert.Equal(SDLSize.One, block.ElementSize);
					Assert.Equal(original, block.Span[..8].ToArray());

					Assert.True(SDLMemory.TryReallocate(block, new SDLSize(4)));
					Assert.Equal(new SDLSize(4), block.ByteLength);
					Assert.Equal(new byte[] { 1, 2, 3, 4 }, block.Span.ToArray());
				}
			}

			[Fact]
			public void PublicReallocate_ShouldUpdateAndResetElementShape()
			{
				using (SDLMemoryBlock block = SDLMemory.Calloc<int>(new SDLSize(3))) 
				{
					block.AsSpan<int>()[1] = 12345;
					SDLMemory.Reallocate(block, new SDLSize(6), new SDLSize(sizeof(int)));
					Assert.Equal(new SDLSize(6), block.ElementCount);
					Assert.Equal(new SDLSize(4), block.ElementSize);
					Assert.Equal(new SDLSize(24), block.ByteLength);
					Assert.Equal(12345, block.AsSpan<int>()[1]);

					Assert.True(SDLMemory.TryReallocate(block, new SDLSize(2), new SDLSize(sizeof(int))));
					Assert.Equal(new SDLSize(2), block.ElementCount);
					Assert.Equal(new SDLSize(8), block.ByteLength);
					Assert.Equal(12345, block.AsSpan<int>()[1]);

					block.Reallocate(new SDLSize(16));
					Assert.Equal(new SDLSize(16), block.ElementCount);
					Assert.Equal(SDLSize.One, block.ElementSize);
				}
			}

			[Fact]
			public void PublicReallocate_OverflowShouldPreserveOwnershipShapeAndContents()
			{
				using (SDLMemoryBlock block = SDLMemory.Calloc<int>(new SDLSize(2))) 
				{
					block.AsSpan<int>()[0] = 5678;
					nint address = block.Pointer.Address;
					Assert.Throws<OverflowException>(() =>
						block.Reallocate(SDLSize.MaxValue, new SDLSize(2))
					);
					Assert.Throws<OverflowException>(() =>
						block.TryReallocate(SDLSize.MaxValue, new SDLSize(2))
					);

					Assert.Equal(address, block.Pointer.Address);
					Assert.Equal(new SDLSize(8), block.ByteLength);
					Assert.Equal(new SDLSize(2), block.ElementCount);
					Assert.Equal(new SDLSize(4), block.ElementSize);
					Assert.Equal(5678, block.AsSpan<int>()[0]);
				}
			}

			[Fact]
			public void ZeroByteAllocations_ShouldRemainOwnedAndRejectCursorAttachment()
			{
				using (SDLMemoryBlock block = SDLMemory.Allocate(SDLSize.Zero)) 
				{
					Assert.False(block.IsInvalid);
					Assert.Equal(SDLSize.Zero, block.ByteLength);
					Assert.True(block.Span.IsEmpty);
					Assert.Throws<InvalidOperationException>(() =>
						block.GetCursor()
					);
					Assert.False(new SDLMemoryCursor().TryAttach(block));

					block.Reallocate(new SDLSize(8));
					block.Span.Fill(0x5A);
					// SDL_realloc(mem, 0) allocates at least one native byte; the wrapper's logical length is zero.
					block.Reallocate(SDLSize.Zero);
					Assert.False(block.IsReleased);
					Assert.True(block.Span.IsEmpty);
					Assert.Equal(SDLSize.Zero, block.ElementCount);
				}
			}

			[Fact]
			public void TypedSpan_ShouldRejectPartialElements()
			{
				using (SDLMemoryBlock block = SDLMemory.Allocate(new SDLSize(3))) 
				{
					Assert.Equal(3, block.AsSpan<byte>().Length);
					Assert.Throws<InvalidOperationException>(() =>
						block.AsSpan<ushort>()
					);
				}
			}

			[Theory]
			[InlineData(16)]
			[InlineData(32)]
			[InlineData(64)]
			public void PublicAlignedAllocation_ShouldPreserveAlignmentAndReleaseWithTheAlignedAllocator(int alignment)
			{
				using SDLAlignedMemoryBlock block = SDLMemory.AllocateAligned(new SDLSize((nuint)alignment), new SDLSize(128));
				Assert.True(block.IsAligned);
				Assert.True(block.Pointer.IsAligned);
				Assert.True(block.ConstPointer.IsAligned);
				Assert.True(block.IsCorrectlyAligned);

				Assert.Equal((nuint)0, (nuint)block.Pointer.Address % (nuint)alignment);
				Assert.Equal(new SDLSize((nuint)alignment), block.Alignment);
				Assert.Equal(new SDLSize(128), block.ByteLength);

				block.AsSpan<uint>().Fill(0x11223344u);
				Assert.All(block.AsSpan<uint>().ToArray(), value => Assert.Equal(0x11223344u, value));
				block.ReleaseMemory();
				block.Free();
				AssertReleased(block);
				Assert.Throws<ObjectDisposedException>(() =>
					block.IsCorrectlyAligned
				);
			}

			[Fact]
			public void PublicFreeAliases_ShouldBeIdempotentForBothAllocatorFamilies()
			{
				using SDLMemoryBlock normal = SDLMemory.Allocate(new SDLSize(8));
				SDLMemory.Free(normal);
				SDLMemory.ReleaseMemory(normal);
				AssertReleased(normal);

				using SDLAlignedMemoryBlock aligned = SDLMemory.AllocateAligned(new SDLSize(16), new SDLSize(32));
				SDLMemory.ReleaseMemory(aligned);
				SDLMemory.Free(aligned);
				AssertReleased(aligned);
			}

			[Fact]
			public void PublicHandleFunctions_ShouldRejectNullOwners()
			{
				Assert.Throws<ArgumentNullException>(() =>
					SDLMemory.Reallocate(null!, SDLSize.One)
				);
				Assert.Throws<ArgumentNullException>(() =>
					SDLMemory.Reallocate(null!, SDLSize.One, SDLSize.One)
				);
				Assert.Throws<ArgumentNullException>(() =>
					SDLMemory.TryReallocate(null!, SDLSize.One)
				);
				Assert.Throws<ArgumentNullException>(() =>
					SDLMemory.TryReallocate(null!, SDLSize.One, SDLSize.One)
				);
				Assert.Throws<ArgumentNullException>(() =>
					SDLMemory.Free((SDLMemoryBlock)null!)
				);
				Assert.Throws<ArgumentNullException>(() =>
					SDLMemory.Free((SDLAlignedMemoryBlock)null!)
				);
				Assert.Throws<ArgumentNullException>(() =>
					SDLMemory.ReleaseMemory((SDLMemoryBlock)null!)
				);
				Assert.Throws<ArgumentNullException>(() =>
					SDLMemory.ReleaseMemory((SDLAlignedMemoryBlock)null!)
				);
			}

			[Fact]
			public void TypedReadWrite_ShouldSupportUnalignedOffsetsAndRejectInvalidRanges()
			{
				using (SDLMemoryBlock block = SDLMemory.Calloc<byte>(new SDLSize(12))) 
				{
					block.Write(new SDLSize(1), 0x11223344u);
					Assert.Equal(0x11223344u, block.Read<uint>(new SDLSize(1)));
					Assert.True(block.TryWrite(new SDLSize(8), 0xAABBCCDDu));
					Assert.True(block.TryRead(new SDLSize(8), out uint value));
					Assert.Equal(0xAABBCCDDu, value);

					byte[] before = block.Span.ToArray();
					Assert.False(block.TryWrite(new SDLSize(9), 1u));
					Assert.False(block.TryWrite(SDLSize.MaxValue, (byte)1));
					Assert.False(block.TryRead(new SDLSize(9), out value));

					Assert.Equal(0u, value);
					Assert.False(block.TryRead(SDLSize.MaxValue, out value));
					Assert.Equal(0u, value);
					Assert.Throws<InvalidOperationException>(() =>
						block.Read<uint>(new SDLSize(9))
					);
					Assert.Throws<InvalidOperationException>(() =>
						block.Write(new SDLSize(9), 1u)
					);
					Assert.Equal(before, block.Span.ToArray());
				}
			}

			[Fact]
			public void SpanReadWrite_ShouldCheckBoundsBeforeChangingEitherBuffer()
			{
				using (SDLMemoryBlock block = SDLMemory.Calloc<byte>(new SDLSize(8))) 
				{
					byte[] source = { 11, 22, 33 };
					block.Write(new SDLSize(5), source.AsSpan());
					byte[] destination = new byte[3];

					block.Read(new SDLSize(5), destination.AsSpan());
					Assert.Equal(source, destination);
					Array.Fill(destination, (byte)0xCC);
					Assert.False(block.TryRead(new SDLSize(6), destination.AsSpan()));
					Assert.Equal(new byte[] { 0xCC, 0xCC, 0xCC }, destination);
					Assert.False(block.TryWrite(new SDLSize(6), source.AsSpan()));
					Assert.Equal(new byte[] { 0, 0, 0, 0, 0, 11, 22, 33 }, block.Span.ToArray());

					Assert.Throws<InvalidOperationException>(() =>
						block.Read(new SDLSize(6), destination.AsSpan())
					);
					Assert.Throws<InvalidOperationException>(() =>
						block.Write(new SDLSize(6), source.AsSpan())
					);

					Assert.True(block.TryRead(block.ByteLength, Span<byte>.Empty));
					Assert.True(block.TryWrite(block.ByteLength, ReadOnlySpan<byte>.Empty));
					Assert.False(block.TryRead(SDLSize.MaxValue, Span<byte>.Empty));
					Assert.False(block.TryWrite(SDLSize.MaxValue, ReadOnlySpan<byte>.Empty));
				}
			}

			[Fact]
			public void PublicCopy_ShouldCopyOnlyTheSourceLength()
			{
				byte[] source = { 1, 2, 3 };
				byte[] destination = { 9, 9, 9, 9, 9 };
				SDLMemory.Copy(destination, source);
				Assert.Equal(new byte[] { 1, 2, 3, 9, 9 }, destination);
				Assert.Equal(new byte[] { 1, 2, 3 }, source);
				Assert.Throws<ArgumentException>(() =>
					SDLMemory.Copy(new byte[2], source)
				);
			}

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public void PublicMove_ShouldHandleOverlappingSpansInBothDirections(bool moveRight)
			{
				byte[] values = { 1, 2, 3, 4, 5 };
				if (moveRight) {
					SDLMemory.Move(values.AsSpan(1), values.AsSpan(0, 4));
					Assert.Equal(new byte[] { 1, 1, 2, 3, 4 }, values);
				}
				else {
					SDLMemory.Move(values.AsSpan(0, 4), values.AsSpan(1));
					Assert.Equal(new byte[] { 2, 3, 4, 5, 5 }, values);
				}

				Assert.Throws<ArgumentException>(() =>
					SDLMemory.Move(new byte[2], values)
				);
			}

			[Fact]
			public void PublicSet_ShouldFillOnlyTheRequestByteOrDwordSpan()
			{
				byte[] bytes = { 1, 2, 3, 4 };
				SDLMemory.Set(bytes.AsSpan(1, 2), 0xA5);
				Assert.Equal(new byte[] { 1, 0xA5, 0xA5, 4 }, bytes);

				uint[] words = { 1, 2, 3, 4 };
				SDLMemory.Set32(words.AsSpan(1, 2), 0x11223344u);
				Assert.Equal(new uint[] { 1, 0x11223344u, 0x11223344u, 4 }, words);
			}

			[Theory]
			[InlineData(new byte[] { }, new byte[] { }, 0)]
			[InlineData(new byte[] { }, new byte[] { 0 }, -1)]
			[InlineData(new byte[] { 0 }, new byte[] { }, 1)]
			[InlineData(new byte[] { 1, 2 }, new byte[] { 1, 2 }, 0)]
			[InlineData(new byte[] { 1, 2 }, new byte[] { 1, 2, 3 }, -1)]
			[InlineData(new byte[] { 1, 2, 3 }, new byte[] { 1, 2 }, 1)]
			[InlineData(new byte[] { 1, 255 }, new byte[] { 1, 127 }, 1)]
			[InlineData(new byte[] { 0, 255 }, new byte[] { 1 }, -1)]
			public void PublicCompare_ShouldUseUsignedLexicographicOrdering(byte[] left, byte[] right, int expectedSign)
			{
				Assert.Equal(expectedSign, Math.Sign(SDLMemory.Compare(left, right)));
				Assert.Equal(expectedSign == 0, SDLMemory.SequenceEqual(left, right));
			}

			[Fact]
			public void PublicSpanHelpers_ShouldAcceptEmptyInputs()
			{
				SDLMemory.Copy(Span<byte>.Empty, ReadOnlySpan<byte>.Empty);
				SDLMemory.Move(Span<byte>.Empty, ReadOnlySpan<byte>.Empty);
				SDLMemory.Set(Span<byte>.Empty, 0xAA);
				SDLMemory.Set32(Span<uint>.Empty, 0x11223344u);
				Assert.Equal(0, SDLMemory.Compare(ReadOnlySpan<byte>.Empty, ReadOnlySpan<byte>.Empty));
				Assert.True(SDLMemory.SequenceEqual(ReadOnlySpan<byte>.Empty, ReadOnlySpan<byte>.Empty));
			}

			private static void AssertReleased(SDLMemoryHandle block)
			{
				Assert.True(block.IsClosed);
				Assert.True(block.IsInvalid);
				Assert.True(block.IsReleased);
				Assert.Equal(SDLSize.Zero, block.ByteLength);
				Assert.False(block.IsAligned);

				Assert.Throws<ObjectDisposedException>(() =>
					block.Pointer
				);
				Assert.Throws<ObjectDisposedException>(() =>
					block.ConstPointer
				);
				Assert.Throws<ObjectDisposedException>(() =>
					_ = block.Span
				);
				Assert.Throws<ObjectDisposedException>(() =>
					block.AsSpan<int>()
				);
				Assert.Throws<ObjectDisposedException>(() =>
					block.Read<byte>(SDLSize.Zero)
				);
				Assert.Throws<ObjectDisposedException>(() =>
					block.TryRead(SDLSize.Zero, out byte _)
				);
				Assert.Throws<ObjectDisposedException>(() =>
					block.Write(SDLSize.Zero, (byte)1)
				);
				Assert.Throws<ObjectDisposedException>(() =>
					block.TryWrite(SDLSize.Zero, (byte)1)
				);
				Assert.Throws<ObjectDisposedException>(() =>
					block.GetCursor()
				);
			}
		}
	}
}