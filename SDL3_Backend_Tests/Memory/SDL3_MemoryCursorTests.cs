using System;
using SDL3;
using SDL3.Unsafe;
using Xunit;

namespace SDL3_Backend_Tests
{
	namespace Memory
	{
		[Collection(SDLRuntimeTestCollection.Name)]
		public sealed class SDLMemoryCursorTests
		{
			[Fact]
			public void Cursor_ShouldAttachAtAnInteriorOffsetAndExposeBorrowedAddresses()
			{
				using (SDLMemoryBlock block = SDLMemory.Allocate(new SDLSize(16))) 
				{
					SDLMemoryCursor cursor = block.GetCursor(new SDLSize(3));
					Assert.True(cursor.IsAttached);
					Assert.Same(block, cursor.Owner);

					Assert.Equal(new SDLSize(3), cursor.ByteOffset);
					Assert.Equal(new SDLSize(13), cursor.RemainingBytes);
					Assert.False(cursor.IsAtStart);
					Assert.False(cursor.IsAtEnd);
					Assert.Equal(block.Pointer.Address + 3, cursor.Pointer.Address);
					Assert.Equal(cursor.Pointer.Address, cursor.ConstPointer.Address);
					Assert.Equal(cursor.ByteOffset, cursor.Pointer.ByteOffset);
					Assert.Equal(cursor.ByteOffset, cursor.ConstPointer.ByteOffset);
					Assert.Equal(cursor.Pointer, cursor.GetRawMemoryAddress());
					Assert.Equal(cursor.ConstPointer, cursor.GetRawConstMemoryAddress());
					Assert.False(cursor.Pointer.IsAligned);
					Assert.False(cursor.ConstPointer.IsAligned);
				}
			}

			[Fact]
			public void Cursor_ShouldPreserveAlignedOwnerProvenanceAtUnalignedOffsets()
			{
				using (SDLAlignedMemoryBlock block = SDLMemory.AllocateAligned(new SDLSize(32), new SDLSize(64))) 
				{
					SDLMemoryCursor cursor = block.GetCursor(SDLSize.One);
					Assert.True(cursor.Pointer.IsAligned);
					Assert.True(cursor.ConstPointer.IsAligned);
					Assert.Equal((nuint)1, (nuint)cursor.Pointer.Address % 32);

					cursor.Write(0x11223344u);
					Assert.Equal(0x11223344u, cursor.Read<uint>());
				}
			}

			[Theory]
			[InlineData(1)]
			[InlineData(8)]
			public void Cursor_MovementShouldStayInsideTheAllocation(int length)
			{
				using (SDLMemoryBlock block = SDLMemory.Allocate(new SDLSize((nuint)length))) 
				{
					SDLMemoryCursor cursor = block.GetCursor();
					Assert.True(cursor.IsAtStart);
					Assert.Equal(new SDLSize((nuint)length), cursor.RemainingBytes);
					Assert.True(cursor.CanAdvance(new SDLSize((nuint)(length - 1))));
					Assert.False(cursor.CanRewind(SDLSize.One));
					Assert.False(cursor.TryRewind(SDLSize.One));
					Assert.False(cursor.TryAdvance(SDLSize.MaxValue));
					Assert.Equal(SDLSize.Zero, cursor.ByteOffset);

					Assert.Same(cursor, cursor.MoveToEnd());
					Assert.True(cursor.IsAtEnd);
					Assert.Equal(new SDLSize((nuint)(length - 1)), cursor.ByteOffset);
					Assert.Equal(SDLSize.One, cursor.RemainingBytes);
					Assert.False(cursor.TryAdvance(SDLSize.One));
					Assert.False(cursor.TrySeek(block.ByteLength));

					Assert.Throws<ArgumentOutOfRangeException>(() =>
						cursor.Advance(SDLSize.One)
					);
					Assert.Throws<ArgumentOutOfRangeException>(() =>
						cursor.Seek(block.ByteLength)
					);

					Assert.True(cursor.TryAdvance(SDLSize.Zero));
					Assert.True(cursor.TryRewind(SDLSize.Zero));
					Assert.Same(cursor, cursor.Reset());
					Assert.True(cursor.IsAtStart);
					Assert.Throws<ArgumentOutOfRangeException>(() =>
						cursor.Rewind(SDLSize.One)
					);
				}
			}

			[Fact]
			public void Cursor_MovementAndOperatorsShouldMutateTheSameCursor()
			{
				using (SDLMemoryBlock block = SDLMemory.Allocate(new SDLSize(8))) 
				{
					SDLMemoryCursor cursor = block.GetCursor();
					SDLMemoryCursor original = cursor;

					cursor++;
					Assert.Same(original, cursor);
					Assert.Equal(SDLSize.One, cursor.ByteOffset);

					cursor--;
					Assert.Equal(SDLSize.Zero, cursor.ByteOffset);
					Assert.Same(cursor, cursor.Advance(new SDLSize(5)));
					Assert.Same(cursor, cursor.Rewind(new SDLSize(2)));

					Assert.Equal(new SDLSize(3), cursor.ByteOffset);
					Assert.True(cursor.TrySeek(new SDLSize(6)));
					Assert.True(cursor.TryRewind(new SDLSize(6)));
					Assert.True(cursor.IsAtStart);
				}
			}

			[Fact]
			public void Cursor_AttachShouldValidateOwnersAndPErserveStateOnFailure()
			{
				using (SDLMemoryBlock first = SDLMemory.Allocate(new SDLSize(8)))
				using (SDLMemoryBlock second = SDLMemory.Allocate(new SDLSize(4)))
				using (SDLMemoryBlock empty = SDLMemory.Allocate(SDLSize.Zero))
				using (SDLMemoryBlock released = SDLMemory.Allocate(SDLSize.One)) 
				{
					released.Dispose();
					SDLMemoryCursor cursor = first.GetCursor(new SDLSize(2));

					Assert.False(cursor.TryAttach(null));
					Assert.False(cursor.TryAttach(empty));
					Assert.False(cursor.TryAttach(released));
					Assert.False(cursor.TryAttach(second, second.ByteLength));

					Assert.Throws<ArgumentNullException>(() =>
						cursor.Attach(null!)
					);
					Assert.Throws<InvalidOperationException>(() =>
						cursor.Attach(empty)
					);
					Assert.Throws<ObjectDisposedException>(() =>
						cursor.Attach(released)
					);
					Assert.Throws<ArgumentOutOfRangeException>(() =>
						cursor.Attach(second, second.ByteLength)
					);

					Assert.Same(first, cursor.Owner);
					Assert.Equal(new SDLSize(2), cursor.ByteOffset);
					Assert.Same(cursor, cursor.Attach(second));
					Assert.Same(second, cursor.Owner);
					Assert.True(cursor.IsAtStart);
					Assert.True(cursor.TryAttach(first, SDLSize.One));
					Assert.Same(first, cursor.Owner);
					Assert.Equal(SDLSize.One, cursor.ByteOffset);
				}
			}

			[Fact]
			public void DetachedCursor_ShouldRejectAccessAndBeReusable()
			{
				SDLMemoryCursor cursor = new SDLMemoryCursor();
				Assert.False(cursor.IsAttached);
				Assert.Null(cursor.Owner);
				Assert.False(cursor.TryAdvance(SDLSize.Zero));
				Assert.False(cursor.TryRewind(SDLSize.Zero));
				Assert.False(cursor.TrySeek(SDLSize.Zero));
				Assert.False(cursor.TryRead(out int value));
				Assert.Equal(0, value);
				Assert.False(cursor.TryWrite(123));
				Assert.False(cursor.TryRead(Span<byte>.Empty));
				Assert.False(cursor.TryWrite(ReadOnlySpan<byte>.Empty));
				Assert.Throws<InvalidOperationException>(() => 
					cursor.Pointer
				);
				Assert.Throws<InvalidOperationException>(() =>
					cursor.RemainingBytes
				);
				Assert.Throws<InvalidOperationException>(() =>
					cursor.Reset()
				);
				Assert.Throws<InvalidOperationException>(() =>
					cursor.Advance(SDLSize.Zero)
				);

				using (SDLMemoryBlock block = SDLMemory.Allocate(new SDLSize(8))) 
				{
					Assert.Same(cursor, cursor.Attach(block, new SDLSize(3)));
					Assert.Same(cursor, cursor.Detach());
					Assert.False(cursor.IsAttached);
					Assert.Null(cursor.Owner);
					Assert.Equal(SDLSize.Zero, cursor.ByteOffset);
					Assert.True(cursor.TryAttach(block));
					Assert.True(cursor.IsAtStart);
				}
			}

			[Fact]
			public void Cursor_TypedAccessShouldSupportUnalignedValuesWithoutImplicitMovement()
			{
				using (SDLMemoryBlock block = SDLMemory.Calloc<byte>(new SDLSize(16))) 
				{
					SDLMemoryCursor cursor = block.GetCursor(SDLSize.One);
					cursor.Write(0x11223344u);
					Assert.Equal(SDLSize.One, cursor.ByteOffset);
					Assert.Equal(0x11223344u, cursor.Read<uint>());
					Assert.True(cursor.TryRead(out uint value));
					Assert.Equal(0x11223344u, value);
					Assert.True(cursor.TryWrite((ushort)0xAABB));
					Assert.Equal((ushort)0xAABB, block.Read<ushort>(SDLSize.One));

					cursor.MoveToEnd();
					Assert.False(cursor.TryRead(out uint failedValue));
					Assert.Equal(0u, failedValue);
					Assert.False(cursor.TryWrite(123u));
					Assert.Throws<InvalidOperationException>(() =>
						cursor.Read<uint>()
					);
					Assert.Throws<InvalidOperationException>(() =>
						cursor.Write(123u)
					);

					cursor.Write((byte)0x5A);
					Assert.Equal((byte)0x5A, cursor.Read<byte>());
				}
			}

			[Fact]
			public void Cursor_TypedAccessAndAdvanceShouldRequireAnInteriorResultingOffset()
			{
				using (SDLMemoryBlock block = SDLMemory.Calloc<uint>(new SDLSize(2))) 
				{
					SDLMemoryCursor cursor = block.GetCursor();
					cursor.WriteAndAdvance(0x11223344u);
					Assert.Equal(new SDLSize(4), cursor.ByteOffset);
					byte[] before = block.Span.ToArray();
					Assert.False(cursor.TryWriteAndAdvance(0xAABBCCDDu));
					Assert.Throws<InvalidOperationException>(() =>
						cursor.WriteAndAdvance(0xAABBCCDDu)
					);

					Assert.Equal(before, block.Span.ToArray());
					Assert.Equal(new SDLSize(4), cursor.ByteOffset);
					cursor.Reset();

					Assert.Equal(0x11223344u, cursor.ReadAndAdvance<uint>());
					Assert.Equal(new SDLSize(4), cursor.ByteOffset);

					Assert.False(cursor.TryReadAndAdvance(out uint value));
					Assert.Equal(0u, value);
					Assert.Throws<InvalidOperationException>(() =>
						cursor.ReadAndAdvance<uint>()
					);

					//The current contract keeps the cursor on a real byte, including after access.
					Assert.Equal(new SDLSize(4), cursor.ByteOffset);
					Assert.True(cursor.TryWriteAndAdvance((byte)0x7A));
					Assert.Equal(new SDLSize(5), cursor.ByteOffset);
				}
			}

			[Fact]
			public void Cursor_SpanAccessShouldRespectBoundsAndLeavePositionUnchanged()
			{
				using (SDLMemoryBlock block = SDLMemory.Calloc<byte>(new SDLSize(8))) 
				{
					SDLMemoryCursor cursor = block.GetCursor(new SDLSize(5));
					byte[] source = { 11, 22, 33 };
					cursor.Write((ReadOnlySpan<byte>)source);
					byte[] destination = new byte[3];
					cursor.Read(destination.AsSpan());
					Assert.Equal(source, destination);
					Assert.Equal(new SDLSize(5), cursor.ByteOffset);
					byte[] oversized = { 0xCC, 0xCC, 0xCC, 0xCC };
					Assert.False(cursor.TryRead(oversized.AsSpan()));
					Assert.False(cursor.TryWrite((ReadOnlySpan<byte>)oversized));
					Assert.Equal(new byte[] { 0xCC, 0xCC, 0xCC, 0xCC }, oversized);
					Assert.Throws<InvalidOperationException>(() =>
						cursor.Read(oversized.AsSpan())
					);
					Assert.Throws<InvalidOperationException>(() =>
						cursor.Write((ReadOnlySpan<byte>)oversized)
					);
					Assert.Equal(new byte[] { 0, 0, 0, 0, 0, 11, 22, 33 }, block.Span.ToArray());
				}
			}

			[Fact]
			public void Cursor_SpanAccessAndAdvanceShouldBeAtomicAtTheEndBoundary()
			{
				using (SDLMemoryBlock block = SDLMemory.Calloc<byte>(new SDLSize(8))) 
				{
					SDLMemoryCursor cursor = block.GetCursor();
					byte[] source = { 1, 2, 3 };
					cursor.WriteAndAdvance((ReadOnlySpan<byte>)source);
					Assert.Equal(new SDLSize(3), cursor.ByteOffset);
					cursor.Reset();

					byte[] destination = new byte[3];
					cursor.ReadAndAdvance(destination.AsSpan());
					Assert.Equal(source, destination);
					Assert.Equal(new SDLSize(3), cursor.ByteOffset);

					cursor.Seek(new SDLSize(5));
					byte[] before = block.Span.ToArray();

					Array.Fill(destination, (byte)0xCC);
					Assert.False(cursor.TryReadAndAdvance(destination.AsSpan()));
					Assert.False(cursor.TryWriteAndAdvance((ReadOnlySpan<byte>)source));
					Assert.Throws<InvalidOperationException>(() =>
						cursor.ReadAndAdvance(destination.AsSpan())
					);
					Assert.Throws<InvalidOperationException>(() =>
						cursor.WriteAndAdvance((ReadOnlySpan<byte>)source)
					);

					Assert.Equal(new byte[] { 0xCC, 0xCC, 0xCC }, destination);
					Assert.Equal(before, block.Span.ToArray());
					Assert.Equal(new SDLSize(5), cursor.ByteOffset);
					Assert.True(cursor.TryReadAndAdvance(Span<byte>.Empty));
					Assert.True(cursor.TryWriteAndAdvance(ReadOnlySpan<byte>.Empty));
					Assert.Equal(new SDLSize(5), cursor.ByteOffset);
				}
			}

			[Fact]
			public void HandCursorOverloads_ShouldValidateOwnershipAndUseTheCursorOffset()
			{
				using (SDLMemoryBlock block = SDLMemory.Calloc<byte>(new SDLSize(16)))
				using (SDLMemoryBlock other = SDLMemory.Allocate(new SDLSize(16))) 
				{
					SDLMemoryCursor cursor = block.GetCursor(new SDLSize(3));
					SDLMemoryCursor foreign = other.GetCursor();
					block.Write(cursor, 12345);
					Assert.Equal(12345, block.Read<int>(cursor));
					Assert.True(block.TryWrite(cursor, 54321));
					Assert.True(block.TryRead(cursor, out int value));
					Assert.Equal(54321, value);

					byte[] source = { 7, 8, 9 };
					block.Write(cursor, (ReadOnlySpan<byte>)source);
					byte[] destination = new byte[3];
					block.Read(cursor, destination.AsSpan());
					Assert.Equal(source, destination);

					Assert.True(block.TryWrite(cursor, (ReadOnlySpan<byte>)source));
					Assert.True(block.TryRead(cursor, destination.AsSpan()));
					Assert.False(block.TryRead(foreign, out value));
					Assert.Equal(0, value);

					Assert.False(block.TryWrite(foreign, 1));
					Assert.False(block.TryRead(foreign, destination.AsSpan()));
					Assert.False(block.TryWrite(foreign, (ReadOnlySpan<byte>)source));
					Assert.False(block.TryRead((SDLMemoryCursor?)null, out value));
					Assert.False(block.TryWrite((SDLMemoryCursor?)null, 1));

					Assert.Throws<ArgumentNullException>(() =>
						block.Read<int>((SDLMemoryCursor)null!)
					);
					Assert.Throws<InvalidOperationException>(() =>
						block.Read<int>(foreign)
					);
					Assert.Throws<InvalidOperationException>(() =>
						block.Write(foreign, 1)
					);
					Assert.Throws<InvalidOperationException>(() =>
						block.Read(foreign, destination.AsSpan())
					);
					Assert.Throws<InvalidOperationException>(() =>
						block.Write(foreign, (ReadOnlySpan<byte>)source)
					);

					Assert.Equal(new SDLSize(3), cursor.ByteOffset);
				}
			}

			[Fact]
			public void HandleCursorTryRead_ShouldCopyOverlappingMemoryExactlyOnce()
			{
				using (SDLMemoryBlock block = SDLMemory.Allocate(new SDLSize(5))) 
				{
					block.Write(SDLSize.Zero, (ReadOnlySpan<byte>)new byte[] { 1, 2, 3, 4, 5 });
					SDLMemoryCursor cursor = block.GetCursor();

					Assert.True(block.TryRead(cursor, block.Span.Slice(1, 4)));
					Assert.Equal(new byte[] { 1, 1, 2, 3, 4 }, block.Span.ToArray());
					Assert.Equal(SDLSize.Zero, cursor.ByteOffset);
				}
			}

			[Fact]
			public void Cursor_ShouldFollowReallocationAndRecoverFromAnOffsetInvalidatedByShrinking()
			{
				using (SDLMemoryBlock block = SDLMemory.Calloc<byte>(new SDLSize(16))) 
				{
					SDLMemoryCursor cursor = block.GetCursor(new SDLSize(6));
					cursor.Write((byte)0x5A);
					block.Reallocate(new SDLSize(32));
					Assert.Equal(block.Pointer.Address + 6, cursor.Pointer.Address);
					Assert.Equal((byte)0x5A, cursor.Read<byte>());
					Assert.Equal(new SDLSize(26), cursor.RemainingBytes);

					block.Reallocate(new SDLSize(4));
					Assert.False(cursor.TryRead(out byte _));
					Assert.False(cursor.TryAdvance(SDLSize.Zero));
					Assert.Throws<InvalidOperationException>(() =>
						cursor.Pointer
					);
					Assert.Throws<InvalidOperationException>(() =>
						cursor.RemainingBytes
					);

					Assert.True(cursor.TrySeek(SDLSize.One));
					Assert.Equal(block.Pointer.Address + 1, cursor.Pointer.Address);
					Assert.Same(cursor, cursor.Reset());
					Assert.True(cursor.IsAtStart);
				}		
			}

			[Fact]
			public void Cursor_ShouldDetectReleasedOwnersBeforeAccessingNativeMemory()
			{
				using (SDLMemoryBlock block = SDLMemory.Allocate(new SDLSize(8))) 
				{
					SDLMemoryCursor cursor = block.GetCursor();
					block.Dispose();

					Assert.False(cursor.TrySeek(SDLSize.Zero));
					Assert.False(cursor.TryAdvance(SDLSize.Zero));
					Assert.False(cursor.TryRewind(SDLSize.Zero));
					Assert.False(cursor.TryRead(out int value));
					Assert.Equal(0, value);

					Assert.False(cursor.TryWrite(123));
					Assert.False(cursor.TryReadAndAdvance(out value));
					Assert.False(cursor.TryWriteAndAdvance(123));
					Assert.False(cursor.TryRead(Span<byte>.Empty));
					Assert.False(cursor.TryWrite(ReadOnlySpan<byte>.Empty));

					Assert.Throws<ObjectDisposedException>(() =>
						cursor.Pointer
					);
					Assert.Throws<ObjectDisposedException>(() =>
						cursor.ConstPointer
					);
					Assert.Throws<ObjectDisposedException>(() =>
						cursor.RemainingBytes
					);
					Assert.Throws<ObjectDisposedException>(() =>
						cursor.Reset()
					);
					Assert.Throws<ObjectDisposedException>(() =>
						cursor.MoveToEnd()
					);
					Assert.Throws<ObjectDisposedException>(() =>
						block.Reallocate(new SDLSize(16))
					);
					Assert.Throws<ObjectDisposedException>(() =>
						block.TryReallocate(new SDLSize(16))
					);
				}
			}
		}
	}
}