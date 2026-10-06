using System;
using SDL3;
using SDL3.Unsafe;
using Xunit;

using MemoryUnsafe = SDL3.Unsafe.SDL3_MemoryUnsafe;

namespace SDL3_Backend_Tests
{
	namespace Memory
	{
		[Collection(SDLRuntimeTestCollection.Name)]
		public sealed unsafe class SDLMemoryUnsafeTests
		{
			[Fact]
			public void UnsafeMallocAndRealloc_ShouldReturnWritableNormalMemoryAndPreserveBytes()
			{
				VoidPtr mem = MemoryUnsafe.Malloc(new SDLSize(8));
				try {
					Assert.False(mem.IsNull);
					Assert.False(mem.IsAligned);
					new Span<byte>((void*)mem, 8).Fill(0xA5);
					VoidPtr resizedMem = MemoryUnsafe.Realloc(mem, new SDLSize(32));
					Assert.False(resizedMem.IsNull);

					mem = resizedMem;
					Assert.False(mem.IsAligned);
					Assert.Equal(new byte[] { 0xA5, 0xA5, 0xA5, 0xA5, 0xA5, 0xA5, 0xA5, 0xA5 }, new ReadOnlySpan<byte>((void*)mem, 8).ToArray());
				}
				finally 
				{
					MemoryUnsafe.Free(mem);
				}
			}

			[Fact]
			public void UnsafeTryMallocAndRealloc_ShouldReportSuccessAndPreserveThePrefix()
			{
				bool allocated = MemoryUnsafe.TryMalloc(new SDLSize(8), out VoidPtr mem);
				try {
					Assert.True(allocated);
					Assert.False(mem.IsNull);
					((byte*)(void*)mem)[0] = 0x5A;
					bool success = MemoryUnsafe.TryRealloc(mem, new SDLSize(16), out VoidPtr resizedMem);
					if (success) {
						mem = resizedMem;
					}

					Assert.True(success);
					Assert.Equal((byte)0x5A, ((byte*)(void*)mem)[0]);
				}
				finally 
				{
					MemoryUnsafe.Free(mem);
				}
			}

			[Theory]
			[InlineData(false)]
			[InlineData(true)]
			public void UnsafeCalloc_ShouldZeroEveryElement(bool useTry)
			{
				VoidPtr mem;
				bool allocated;
				if (useTry) {
					allocated = MemoryUnsafe.TryCalloc(new SDLSize(4), new SDLSize(8), out mem);
				}
				else {
					mem = MemoryUnsafe.Calloc(new SDLSize(4), new SDLSize(8));
					allocated = !mem.IsNull;
				}

				try 
				{
					Assert.True(allocated);
					Assert.False(mem.IsAligned);
					Assert.Equal(new byte[32], new ReadOnlySpan<byte>((void*)mem, 32).ToArray());
				}
				finally
				{
					MemoryUnsafe.Free(mem);
				}
			}

			[Theory]
			[InlineData(false)]
			[InlineData(true)]
			public void UnsafeAlignedAlloc_ShouldReturnAlignedMemoryWithProvenance(bool useTry)
			{
				VoidPtr mem;
				bool allocated;
				if (useTry) {
					allocated = MemoryUnsafe.TryAlignedAlloc(new SDLSize(32), new SDLSize(64), out mem);
				}
				else {
					mem = MemoryUnsafe.AlignedAlloc(new SDLSize(32), new SDLSize(64));
					allocated = !mem.IsNull;
				}

				try {
					Assert.True(allocated);
					Assert.True(mem.IsAligned);
					Assert.Equal((nuint)0, (nuint)mem.Address % 32);
					new Span<byte>((void*)mem, 64).Fill(0x7B);
					Assert.Equal((byte)0x7B, ((byte*)(void*)mem)[63]);
				}
				finally 
				{
					MemoryUnsafe.AlignedFree(mem);
				}
			}

			[Fact]
			public void UnsafeAllocatorGuards_ShouldRejectMismatchedAllocationFamilies()
			{
				using (SDLMemoryBlock normalMem = SDLMemory.Allocate(new SDLSize(16)))
				using (SDLAlignedMemoryBlock alignedMem = SDLMemory.AllocateAligned(new SDLSize(16), new SDLSize(32))) {
					Assert.Throws<InvalidOperationException>(() =>
						MemoryUnsafe.Free(alignedMem.Pointer)
					);
					Assert.Throws<InvalidOperationException>(() =>
						MemoryUnsafe.Realloc(alignedMem.Pointer, new SDLSize(64))
					);
					Assert.Throws<InvalidOperationException>(() =>
						MemoryUnsafe.TryRealloc(alignedMem.Pointer, new SDLSize(64), out _)
					);
					Assert.Throws<InvalidOperationException>(() =>
						MemoryUnsafe.AlignedFree(normalMem.Pointer);
					);

					Assert.Throws<ArgumentException>(() =>
						new SDLMemoryBlock(alignedMem.Pointer, alignedMem.ByteLength)
					);
					Assert.Throws<ArgumentException>(() =>
						new SDLAlignedMemoryBlock(normalMem.Pointer, normalMem.ByteLength, new SDLSize(16))
					);
					Assert.False(normalMem.IsReleased);
					Assert.False(alignedMem.IsReleased);

					normalMem.Span[0] = 0x11;
					alignedMem.Span[0] = 0x22;
					Assert.Equal((byte)0x11, normalMem.Span[0]);
					Assert.Equal((byte)0x22, alignedMem.Span[0]);
				}
			}

			[Fact]
			public void UnsafeFree_ShouldAcceptNullAndReallocNullShouldAllocate()
			{
				MemoryUnsafe.Free(VoidPtr.Null);
				MemoryUnsafe.AlignedFree(VoidPtr.Null);
				VoidPtr mem = MemoryUnsafe.Realloc(VoidPtr.Null, new SDLSize(8));
				try {
					Assert.False(mem.IsNull);
					Assert.False(mem.IsAligned);
				}
				finally 
				{
					MemoryUnsafe.Free(mem);
				}
			}

			[Theory]
			[InlineData(false)]
			[InlineData(true)]
			public void UnsafeMemoryOperations_ShouldPreserveTheDestinationAddressAndProvenance(bool aligned)
			{
				uint* dest = stackalloc uint[4];
				uint* src = stackalloc uint[] { 0x11223344u, 0x55667788u };
				new Span<uint>(dest, 4).Fill(0xCCCCCCCCu);
				VoidPtr pointer = new VoidPtr(dest, aligned);
				VoidPtr copied = MemoryUnsafe.Memcpy(pointer, ConstVoidPtr.From(src), new SDLSize(8));
				Assert.Equal(pointer.Address, copied.Address);
				Assert.Equal(aligned, copied.IsAligned);
				Assert.Equal(new uint[] { 0x11223344u, 0x55667788u, 0xCCCCCCCCu, 0xCCCCCCCCu }, new ReadOnlySpan<uint>(dest, 4).ToArray());

				VoidPtr set = MemoryUnsafe.Memset(pointer, (byte)0xA5, new SDLSize(4));
				Assert.Equal(pointer.Address, set.Address);
				Assert.Equal(aligned, set.IsAligned);
				Assert.Equal(0xA5A5A5A5u, dest[0]);

				VoidPtr moved = MemoryUnsafe.Memmove(pointer, ConstVoidPtr.From(src), new SDLSize(4));
				Assert.Equal(pointer.Address, moved.Address);
				Assert.Equal(aligned, moved.IsAligned);
				Assert.Equal(0x11223344u, dest[0]);

				VoidPtr set4 = MemoryUnsafe.Memset4(pointer, 0xABCDEF01u, new SDLSize(2));
				Assert.Equal(pointer.Address, set4.Address);
				Assert.Equal(aligned, set4.IsAligned);

				Assert.Equal(new uint[] { 0xABCDEF01u, 0xABCDEF01u, 0xCCCCCCCCu, 0xCCCCCCCCu }, new ReadOnlySpan<uint>(dest, 4).ToArray());
			}

			[Theory]
			[InlineData(0, 0)]
			[InlineData(0x1234, 0x34)]
			[InlineData(-1, 0xFF)]
			public void UnsafeMemset_ShouldUseOnlyTheLowByteOfAnInteger(int value, int expected)
			{
				byte* mem = stackalloc byte[] { 1, 2, 3, 4 };
				VoidPtr pointer = new VoidPtr(mem + 1);
				Assert.Equal(pointer, MemoryUnsafe.Memset(pointer, value, new SDLSize(2)));
				Assert.Equal(new byte[] { 1, (byte)expected, (byte)expected, 4 }, new ReadOnlySpan<byte>(mem, 4).ToArray());
			}

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public void UnsafeMemmove_ShouldSupportOverlapInBothDirections(bool moveRight)
			{
				byte* mem = stackalloc byte[] { 1, 2, 3, 4, 5 };
				VoidPtr dest;
				if (moveRight) {
					dest = new VoidPtr(mem + 1);
				}
				else {
					dest = new VoidPtr(mem);
				}

				ConstVoidPtr src;
				if (moveRight) {
					src = new ConstVoidPtr(mem);
				}
				else {
					src = new ConstVoidPtr(mem + 1);
				}

				Assert.Equal(dest, MemoryUnsafe.Memmove(dest, src, new SDLSize(4)));
				if (moveRight) {
					Assert.Equal(new byte[] { 1, 1, 2, 3, 4 }, new ReadOnlySpan<byte>(mem, 5).ToArray());
				}
				else {
					Assert.Equal(new byte[] { 2, 3, 4, 5, 5 }, new ReadOnlySpan<byte>(mem, 5).ToArray());
				}
			}

			[Fact]
			public void UnsafeMemcmp_ShouldCompareUnsignedBytesAndHonorRequestedLength()
			{
				byte* left = stackalloc byte[] { 1, 255 };
				byte* right = stackalloc byte[] { 1, 127 };
				Assert.Equal(0, MemoryUnsafe.Memcmp(ConstVoidPtr.From(left), ConstVoidPtr.From(right), SDLSize.Zero));
				Assert.Equal(0, MemoryUnsafe.Memcmp(ConstVoidPtr.From(left), ConstVoidPtr.From(right), SDLSize.One));
				Assert.True(MemoryUnsafe.Memcmp(ConstVoidPtr.From(left), ConstVoidPtr.From(right), new SDLSize(2)) > 0);
				Assert.True(MemoryUnsafe.Memcmp(ConstVoidPtr.From(right), ConstVoidPtr.From(left), new SDLSize(2)) < 0);
			}

			[Fact]
			public void AllocationCount_ShouldReturnToItsBaselineAfterRelease()
			{
				int baseline = SDLMemory.GetNumAllocations();
				Assert.True(baseline >= -1);
				using (SDLMemoryBlock block = SDLMemory.Allocate(new SDLSize(16))) 
				{
					if (baseline < 0) {
						Assert.Equal(-1, SDLMemory.GetNumAllocations());
					}
					else {
						Assert.Equal(baseline + 1, SDLMemory.GetNumAllocations());
					}

					block.Reallocate(new SDLSize(32));
					if (baseline < 0) {
						Assert.Equal(-1, SDLMemory.GetNumAllocations());
					}
					else {
						Assert.Equal(baseline + 1, SDLMemory.GetNumAllocations());
					}
				}

				Assert.Equal(baseline, SDLMemory.GetNumAllocations());
			}
			
		}
	}
}