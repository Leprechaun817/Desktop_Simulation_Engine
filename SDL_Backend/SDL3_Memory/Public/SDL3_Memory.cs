using System;
using SDL3.Unsafe;

namespace SDL3
{
	/// <summary>
	///	Managed-facing SDL memory functions
	/// </summary>
	public static unsafe class SDLMemory
	{
		public static SDLMemoryBlock Allocate(SDLSize byteCnt)
		{
			VoidPtr mem = SDL3_MemoryUnsafe.Malloc(byteCnt);
			if (mem.IsNull) {
				throw new OutOfMemoryException($"SDL_malloc failed to allocate {(ulong)byteCnt} byte(s).");
			}

			return new SDLMemoryBlock(mem, byteCnt);
		}

		public static SDLMemoryBlock Allocate(size_t byteCnt)
		{
			return Allocate(new SDLSize(byteCnt));
		}

		public static SDLMemoryBlock AllocateZeroed(SDLSize elementCnt, SDLSize elementSize)
		{
			if (!SDLSize.TryMultiply(elementCnt, elementSize, out SDLSize totalBytes)) {
				throw new OverflowException("The requested SDL_calloc allocation size overflowed size_t");
			}

			VoidPtr mem = SDL3_MemoryUnsafe.Calloc(elementCnt, elementSize);
			if (mem.IsNull) {
				throw new OutOfMemoryException($"SDL_calloc failed to allocate {(ulong)elementCnt} element(s) of {(ulong)elementSize} byte(s).");
			}

			return new SDLMemoryBlock(mem, totalBytes, elementCnt, elementSize);
		}

		public static SDLMemoryBlock AllocateZeroed<T>(SDLSize elementCnt) where T : unmanaged
		{
			return AllocateZeroed(elementCnt, new SDLSize((size_t)RuntimeUnsafe.SizeOf<T>()));
		}

		public static SDLMemoryBlock Calloc(SDLSize elementCnt, SDLSize elementSize)
		{
			return AllocateZeroed(elementCnt, elementSize);
		}

		public static SDLMemoryBlock Calloc<T>(SDLSize elementCnt) where T : unmanaged
		{
			return AllocateZeroed<T>(elementCnt);
		}

		public static SDLAlignedMemoryBlock AllocateAligned(SDLSize alignment, SDLSize byteCnt)
		{
			VoidPtr mem = SDL3_MemoryUnsafe.AlignedAlloc(alignment, byteCnt);
			if (mem.IsNull) {
				throw new OutOfMemoryException($"SDL_aligned_alloc failed to allocate {(ulong)byteCnt} byte(s) with {(ulong)alignment}-byte alignment");
			}

			return new SDLAlignedMemoryBlock(mem, byteCnt, alignment);
		}

		public static void Reallocate(SDLMemoryBlock block, SDLSize newByteLen)
		{
			ArgumentNullException.ThrowIfNull(block);

			block.Reallocate(newByteLen);
		}

		public static void Reallocate(SDLMemoryBlock block, SDLSize newElementCnt, SDLSize newElementSize)
		{
			ArgumentNullException.ThrowIfNull(block);

			block.Reallocate(newElementCnt, newElementSize);
		}

		public static bool TryReallocate(SDLMemoryBlock block, SDLSize newByteLen)
		{
			ArgumentNullException.ThrowIfNull(block);

			return block.TryReallocate(newByteLen);
		}

		public static bool TryReallocate(SDLMemoryBlock block, SDLSize newElementCnt, SDLSize newElementSize)
		{
			ArgumentNullException.ThrowIfNull(block);

			return block.TryReallocate(newElementCnt, newElementSize);
		}

		public static void Free(SDLMemoryBlock block)
		{
			ArgumentNullException.ThrowIfNull(block);

			block.Dispose();
		}

		public static void ReleaseMemory(SDLMemoryBlock block)
		{
			Free(block);
		}

		public static void Free(SDLAlignedMemoryBlock alignedBlock)
		{
			ArgumentNullException.ThrowIfNull(alignedBlock);

			alignedBlock.Dispose();
		}

		public static void ReleaseMemory(SDLAlignedMemoryBlock alignedBlock)
		{
			Free(alignedBlock);
		}

		public static SDLMemoryFunctionPointerTable GetMemoryFunctions()
		{
			return SDL3_MemoryUnsafe.GetMemoryFunctions();
		}

		public static SDLMemoryFunctionPointerTable GetOriginalMemoryFunctions()
		{
			return SDL3_MemoryUnsafe.GetOriginalMemoryFunctions();
		}

		public static bool SetMemoryFunctionTable(SDLMemoryFunctionPointerTable functionTable)
		{
			return SDL3_MemoryUnsafe.SetMemoryFunctions(functionTable);
		}

		public static bool SetMemoryFunctionTable(SDLMemoryCallbackAdapterTable adapterTable)
		{
			return SDL3_MemoryUnsafe.SetMemoryFunctions(adapterTable);
		}

		public static void Copy(Span<byte> dest, ReadOnlySpan<byte> src)
		{
			if (dest.Length < src.Length) {
				throw new ArgumentException("Destination span must be at least as large as the source span", nameof(dest));
			}

			if (src.Length == 0) {
				return;
			}

			fixed (bptr destPtr = dest)
			fixed (bptr srcPtr = src) 
			{
				SDL3_MemoryUnsafe.Memcpy(new VoidPtr(destPtr), ConstVoidPtr.From(srcPtr), new SDLSize((size_t)src.Length));
			}
		}

		public static void Move(Span<byte> dest, ReadOnlySpan<byte> src)
		{
			if (dest.Length < src.Length) {
				throw new ArgumentException("Destination span must be at least as large as the source span.", nameof(dest));
			}

			if (src.Length == 0) {
				return;
			}

			fixed (bptr destPtr = dest)
			fixed (bptr srcPtr = src) 
			{
				SDL3_MemoryUnsafe.Memmove(new VoidPtr(destPtr), ConstVoidPtr.From(srcPtr), new SDLSize((size_t)src.Length));
			}
		}

		public static void Set(Span<byte> dest, byte value)
		{
			if (dest.Length == 0) {
				return;
			}

			fixed (bptr destPtr = dest)
			{
				SDL3_MemoryUnsafe.Memset(new VoidPtr(destPtr), value, new SDLSize((size_t)dest.Length));	
			}
		}

		public static void Set32(Span<uint> dest, uint value)
		{
			if (dest.Length == 0) {
				return;
			}

			fixed (uiptr destPtr = dest) 
			{
				SDL3_MemoryUnsafe.Memset4(new VoidPtr(destPtr), value, new SDLSize((size_t)dest.Length));
			}
		}

		public static int Compare(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
		{
			int sharedLength = Math.Min(left.Length, right.Length);
			int result = 0;

			if (sharedLength > 0) {
				fixed (bptr leftPtr = left)
				fixed (bptr rightPtr = right) 
				{
					result = SDL3_MemoryUnsafe.Memcmp(ConstVoidPtr.From(leftPtr), ConstVoidPtr.From(rightPtr), new SDLSize((size_t)sharedLength));
				}
			}

			if (result != 0) {
				return result;
			}

			return left.Length.CompareTo(right.Length);
		}

		public static bool SequenceEqual(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
		{
			if (left.Length != right.Length) {
				return false;
			}

			if (left.Length == 0) {
				return true;
			}

			fixed (bptr leftPtr = left)
			fixed (bptr rightPtr = right) 
			{
				if (SDL3_MemoryUnsafe.Memcmp(ConstVoidPtr.From(leftPtr), ConstVoidPtr.From(rightPtr), new SDLSize((size_t)left.Length)) == 0) {
					return true;
				}

				return false;
			}
		}

		public static int GetNumAllocations()
		{
			return SDL3_MemoryUnsafe.GetNumAllocations();
		}
	}
}
