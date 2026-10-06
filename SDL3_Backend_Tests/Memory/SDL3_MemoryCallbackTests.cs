using System;
using System.Runtime.InteropServices;
using SDL3;
using SDL3.Unsafe;
using Xunit;

namespace SDL3_Backend_Tests
{
	namespace Memory
	{
		[Collection(SDLRuntimeTestCollection.Name)]
		public sealed unsafe class SDLMemoryCallbackTests
		{
			[Fact]
			public void MemoryFunctionTables_ShouldExposeCompleteComparableAllocatorTables()
			{
				SDLMemoryFunctionPointerTable current = SDLMemory.GetMemoryFunctions();
				SDLMemoryFunctionPointerTable original = SDLMemory.GetOriginalMemoryFunctions();
				Assert.True(current.IsComplete);
				Assert.True(original.IsComplete);
				current.ThrowIfIncomplete();
				original.ThrowIfIncomplete();

				SDLMemoryFunctionPointerTable copy = new(current.Malloc, current.Calloc, current.Realloc, current.Free);
				Assert.Equal(current, copy);
				Assert.True(current == copy);
				Assert.False(current != copy);
				Assert.True(current.Equals((object)copy));
				Assert.Equal(current.GetHashCode(), copy.GetHashCode());
				Assert.NotEqual(default, current);

				Assert.False(current.Equals(null));
				Assert.False(current.Equals((object)123));
				Assert.Equal(original, SDL3_MemoryUnsafe.GetOriginalMemoryFunctions());
				Assert.Equal(current, SDL3_MemoryUnsafe.GetMemoryFunctions());
			}

			[Theory]
			[InlineData(0)]
			[InlineData(1)]
			[InlineData(2)]
			[InlineData(3)]
			public void MemoryFunctionTables_ShouldRejectEveryIncompleteTable(int missingCallback)
			{
				SDLMemoryFunctionPointerTable original = SDLMemory.GetOriginalMemoryFunctions();
				SDLMemoryFunctionPointerTable incomplete = default;
				if (missingCallback == 0) {
					incomplete = new SDLMemoryFunctionPointerTable(null, original.Calloc, original.Realloc, original.Free);
				}
				else if (missingCallback == 1) {
					incomplete = new SDLMemoryFunctionPointerTable(original.Malloc, null, original.Realloc, original.Free);
				}
				else if (missingCallback == 2) {
					incomplete = new SDLMemoryFunctionPointerTable(original.Malloc, original.Calloc, null, original.Free);
				}
				else if (missingCallback == 3) {
					incomplete = new SDLMemoryFunctionPointerTable(original.Malloc, original.Calloc, original.Realloc, null);
				}

				Assert.False(incomplete.IsComplete);
				Assert.Throws<InvalidOperationException>(() =>
					incomplete.ThrowIfIncomplete()
				);
				Assert.Throws<InvalidOperationException>(() =>
					SDL3_MemoryUnsafe.SetMemoryFunctions(incomplete)
				);
				Assert.Throws<InvalidOperationException>(() =>
					SDLMemory.SetMemoryFunctionTable(incomplete)
				);
			}

			[Fact]
			public void MemoryFunctionTables_ShouldRejectDefaultAndNullAdapters()
			{
				Assert.False(default(SDLMemoryFunctionPointerTable).IsComplete);
				Assert.Throws<InvalidOperationException>(() =>
					default(SDLMemoryFunctionPointerTable).ThrowIfIncomplete()
				);
				Assert.Throws<InvalidOperationException>(() =>
					SDLMemory.SetMemoryFunctionTable(default(SDLMemoryFunctionPointerTable))
				);
				Assert.Throws<ArgumentNullException>(() =>
					SDLMemory.SetMemoryFunctionTable((SDLMemoryCallbackAdapterTable)null!)
				);
				Assert.Throws<ArgumentNullException>(() =>
					SDL3_MemoryUnsafe.SetMemoryFunctions((SDLMemoryCallbackAdapterTable)null!)
				);
			}

			[Theory]
			[InlineData(0, "malloc")]
			[InlineData(1, "calloc")]
			[InlineData(2, "realloc")]
			[InlineData(3, "free")]
			public void CallbackAdapters_ShouldRejectEveryNullDelegate(int missingCallback, string parameterName)
			{
				TrackingAllocator allocator = new TrackingAllocator();
				ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
					SDLMemoryCallbackAdapters.Create(missingCallback == 0 ? null! : allocator.Malloc,
													 missingCallback == 1 ? null! : allocator.Calloc,
													 missingCallback == 2 ? null! : allocator.Realloc,
													 missingCallback == 3 ? null! : allocator.Free)
				);

				Assert.Equal(parameterName, exception.ParamName);
			}

			[Fact]
			public void CallbackAdapters_ShouldRejectNullFunctionPointers()
			{
				Assert.Throws<ArgumentNullException>(() =>
					SDLMemoryCallbackAdapters.GetMallocDelegate(null)
				);
				Assert.Throws<ArgumentNullException>(() =>
					SDLMemoryCallbackAdapters.GetCallocDelegate(null)
				);
				Assert.Throws<ArgumentNullException>(() =>
					SDLMemoryCallbackAdapters.GetReallocDelegate(null)
				);
				Assert.Throws<ArgumentNullException>(() =>
					SDLMemoryCallbackAdapters.GetFreeDelegate(null)
				);
			}

			[Theory]
			[InlineData(typeof(SDLMallocCallback))]
			[InlineData(typeof(SDLCallocCallback))]
			[InlineData(typeof(SDLReallocCallback))]
			[InlineData(typeof(SDLFreeCallback))]
			public void CallbackDelegates_ShouldUseCdecl(Type delegateType)
			{
				UnmanagedFunctionPointerAttribute attribute = Assert.IsType<UnmanagedFunctionPointerAttribute>(Attribute.GetCustomAttribute(delegateType, 
																											   typeof(UnmanagedFunctionPointerAttribute)));
				Assert.Equal(CallingConvention.Cdecl, attribute.CallingConvention);
			}

			[Fact]
			public void CallbackAdapters_ShouldRootDelegatesAndExposeCallableRoundTripPointers()
			{
				TrackingAllocator allocator = new TrackingAllocator();
				SDLMallocCallback malloc = allocator.Malloc;
				SDLCallocCallback calloc = allocator.Calloc;
				SDLReallocCallback realloc = allocator.Realloc;
				SDLFreeCallback free = allocator.Free;
				SDLMemoryCallbackAdapterTable adapter = SDLMemoryCallbackAdapters.Create(malloc, calloc, realloc, free);

				Assert.Same(malloc, adapter.MallocDelegate);
				Assert.Same(calloc, adapter.CallocDelegate);
				Assert.Same(realloc, adapter.ReallocDelegate);
				Assert.Same(free, adapter.FreeDelegate);
				Assert.True(adapter.FunctionPointers.IsComplete);

				Assert.Same(malloc, SDLMemoryCallbackAdapters.GetMallocDelegate(adapter.MallocPointer));
				Assert.Same(calloc, SDLMemoryCallbackAdapters.GetCallocDelegate(adapter.CallocPointer));
				Assert.Same(realloc, SDLMemoryCallbackAdapters.GetReallocDelegate(adapter.ReallocPointer));
				Assert.Same(free, SDLMemoryCallbackAdapters.GetFreeDelegate(adapter.FreePointer));

				GC.Collect();
				GC.WaitForPendingFinalizers();
				void* mem = adapter.MallocPointer(8);
				try 
				{
					Assert.True(mem != null);
					((byte*)mem)[0] = 0x5A;
					void* resized = adapter.ReallocPointer(mem, 16);
					Assert.True(resized != null);
					mem = resized;
					Assert.Equal((byte)0x5A, ((byte*)mem)[0]);
				}
				finally
				{
					adapter.FreePointer(mem);
				}

				void* zeroed = adapter.CallocPointer(4, 4);
				try {
					Assert.True(zeroed != null);
					Assert.Equal(new byte[16], new ReadOnlySpan<byte>(zeroed, 16).ToArray());
				}
				finally 
				{
					adapter.FreePointer(zeroed);
				}

				Assert.Equal(1, allocator.MallocCnt);
				Assert.Equal(1, allocator.CallocCnt);
				Assert.Equal(1, allocator.ReallocCnt);
				Assert.Equal(2, allocator.FreeCnt);
				GC.KeepAlive(adapter);
			}

			[Fact]
			public void OriginalAllocatorDelegates_ShouldAllocateResizeAndReleaseNativeMemory()
			{
				SDLMemoryFunctionPointerTable original = SDLMemory.GetOriginalMemoryFunctions();
				SDLMallocCallback malloc = SDLMemoryCallbackAdapters.GetMallocDelegate(original.Malloc);
				SDLCallocCallback calloc = SDLMemoryCallbackAdapters.GetCallocDelegate(original.Calloc);
				SDLReallocCallback realloc = SDLMemoryCallbackAdapters.GetReallocDelegate(original.Realloc);
				SDLFreeCallback free = SDLMemoryCallbackAdapters.GetFreeDelegate(original.Free);
				void* mem = malloc(8);
				try 
				{
					Assert.True(mem != null);
					((byte*)mem)[0] = 0x33;
					void* resized = realloc(mem, 32);
					Assert.True(resized != null);
					mem = resized;
					Assert.Equal((byte)0x33, ((byte*)mem)[0]);
				}
				finally
				{
					free(mem);
				}

				void* zeroed = calloc(3, 4);
				try {
					Assert.True(zeroed != null);
					Assert.Equal(new byte[12], new ReadOnlySpan<byte>(zeroed, 12).ToArray());
				}
				finally 
				{
					free(zeroed);
				}
			}

			private static void RunAllocatorScenario()
			{
				//Only the isolated test host installs callbacks. They forward to the original allocator,
				//and the adapter stays rooted until every owned block has been freed and SDL is restored.
				SDLMemoryFunctionPointerTable previous = SDLMemory.GetMemoryFunctions();
				TrackingAllocator allocator = new TrackingAllocator();
				SDLMemoryCallbackAdapterTable adapter = SDLMemoryCallbackAdapters.Create(allocator.Malloc, allocator.Calloc, allocator.Realloc, allocator.Free);
				Assert.True(SDLMemory.SetMemoryFunctionTable(adapter));

				try 
				{
					Assert.Equal(adapter.FunctionPointers, SDLMemory.GetMemoryFunctions());
					using (SDLMemoryBlock allocated = SDLMemory.Allocate(new SDLSize(8))) 
					{
						allocated.Span.Fill(0x5A);
						allocated.Reallocate(new SDLSize(16));
						Assert.Equal(new byte[] { 0x5A, 0x5A, 0x5A, 0x5A, 0x5A, 0x5A, 0x5A, 0x5A }, allocated.Span[..8].ToArray());
						nint address = allocated.Pointer.Address;

						allocator.FailNextRealloc = true;
						Assert.False(allocated.TryReallocate(new SDLSize(64)));

						allocator.FailNextRealloc = true;
						Assert.False(allocated.TryReallocate(new SDLSize(16), new SDLSize(4)));

						allocator.FailNextRealloc = true;
						Assert.Throws<OutOfMemoryException>(() =>
							allocated.Reallocate(new SDLSize(64))
						);

						allocator.FailNextRealloc = true;
						Assert.Throws<OutOfMemoryException>(() =>
							allocated.Reallocate(new SDLSize(16), new SDLSize(4))
						);

						Assert.Equal(address, allocated.Pointer.Address);
						Assert.Equal(new SDLSize(16), allocated.ByteLength);
						Assert.Equal(new SDLSize(16), allocated.ElementCount);
						Assert.Equal(SDLSize.One, allocated.ElementSize);
						Assert.Equal((byte)0x5A, allocated.Span[0]);

						allocator.FailNextRealloc = true;
						Assert.False(SDL3_MemoryUnsafe.TryRealloc(allocated.Pointer, new SDLSize(64), out VoidPtr failed));
						Assert.True(failed.IsNull);
						Assert.Equal(address, allocated.Pointer.Address);
						Assert.True(allocated.TryReallocate(new SDLSize(32)));
						Assert.Equal((byte)0x5A, allocated.Span[0]);
					}

					using (SDLMemoryBlock zeroed = SDLMemory.Calloc<int>(new SDLSize(4))) 
					{
						Assert.Equal(new int[4], zeroed.AsSpan<int>().ToArray());
					}

					allocator.FailNextMalloc = true;
					Assert.False(SDL3_MemoryUnsafe.TryMalloc(new SDLSize(8), out VoidPtr failedMalloc));
					Assert.True(failedMalloc.IsNull);

					allocator.FailNextMalloc = true;
					Assert.Throws<OutOfMemoryException>(() =>
						SDLMemory.Allocate(new SDLSize(8))
					);

					allocator.FailNextMalloc = true;
					Assert.False(SDL3_MemoryUnsafe.TryCalloc(new SDLSize(2), new SDLSize(4), out VoidPtr failedCalloc));
					Assert.True(failedCalloc.IsNull);

					allocator.FailNextMalloc = true;
					Assert.Throws<OutOfMemoryException>(() =>
						SDLMemory.Calloc<int>(new SDLSize(2))
					);

					Assert.True(allocator.MallocCnt >= 3);
					Assert.True(allocator.CallocCnt >= 3);
					Assert.True(allocator.ReallocCnt >= 7);
					Assert.True(allocator.FreeCnt >= 2);
				}
				finally
				{
					Assert.True(SDLMemory.SetMemoryFunctionTable(previous));
					GC.KeepAlive(adapter);
				}

				Assert.Equal(previous, SDLMemory.GetMemoryFunctions());
			}

			private sealed class TrackingAllocator
			{
				private readonly SDLMemoryFunctionPointerTable original = SDLMemory.GetOriginalMemoryFunctions();
				public int MallocCnt;
				public int CallocCnt;
				public int ReallocCnt;
				public int FreeCnt;
				public bool FailNextMalloc;
				public bool FailNextCalloc;
				public bool FailNextRealloc;

				public void* Malloc(nuint size)
				{
					MallocCnt++;
					if (FailNextMalloc) {
						FailNextMalloc = false;
						return null;
					}

					return original.Malloc(size);
				}

				public void* Calloc(nuint cnt, nuint size)
				{
					CallocCnt++;
					if (FailNextCalloc) {
						FailNextCalloc = false;
						return null;
					}

					return original.Calloc(cnt, size);
				}

				public void* Realloc(void* mem, nuint size)
				{
					ReallocCnt++;
					if (FailNextRealloc) {
						FailNextRealloc = false;
						return null;
					}

					return original.Realloc(mem, size);
				}

				public void Free(void* mem)
				{
					FreeCnt++;
					original.Free(mem);
				}



			}
		}
	}
}