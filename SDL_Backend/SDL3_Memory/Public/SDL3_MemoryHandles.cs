using System;
using System.Runtime.InteropServices;
using SDL3.Unsafe;

using static SDL3.SDL3_StdIncMacros;

namespace SDL3
{
	/// <summary>
	/// SDLMemoryHandle is the common SafeHandle base for SDL-owned unmanaged memory.
	/// Any APIs that need to distinguish normal SDL memory from aligned SDL memory
	/// should use the SDLMemoryBlock or SDLAlignedMemoryBlock respectively.
	/// </summary>

	public abstract unsafe class SDLMemoryHandle : SafeHandle
	{
		private SDLSize byteLen;
		private bool isAligned;

		public SDLSize ByteLength
		{
			get
			{
				return byteLen;
			}
		}

		public bool IsReleased
		{
			get
			{
				if (IsClosed || IsInvalid) {
					return true;
				}

				return false;
			}
		}

		public bool IsAligned
		{
			get
			{
				return isAligned;
			}
		}

		public VoidPtr Pointer
		{
			get
			{
				ThrowIfInvalidOrClosed();

				return new VoidPtr((vptr)handle, isAligned);
			}
		}

		public ConstVoidPtr ConstPointer
		{
			get
			{
				ThrowIfInvalidOrClosed();

				return new ConstVoidPtr((vptr)handle, isAligned);
			}
		}

		public SDLMemoryCursor GetCursor()
		{
			return GetCursor(SDLSize.Zero);
		}

		public SDLMemoryCursor GetCursor(SDLSize byteOffset)
		{
			return new SDLMemoryCursor(this, byteOffset);
		}

		public Span<byte> Span
		{
			get
			{
				ThrowIfInvalidOrClosed();

				if ((size_t)byteLen > (size_t)SDL_MAX_SINT32) {
					throw new InvalidOperationException("This SDL allocation is too large to expose as Span<byte>.");
				}

				return new Span<byte>((vptr)handle, byteLen.ToInt32Checked());
			}
		}

		public override bool IsInvalid
		{
			get
			{
				if (handle == mptr.Zero || handle == new mptr(-1)) {
					return true;
				}

				return false;
			}
		}

		protected SDLMemoryHandle(VoidPtr mem, SDLSize byteLen) : base(mptr.Zero, true)
		{
			SetHandle((mptr)mem);
			this.byteLen = byteLen;
			isAligned = mem.IsAligned;
		}

		public Span<T> AsSpan<T>() where T : unmanaged
		{
			ThrowIfInvalidOrClosed();

			int elementByteLength = RuntimeUnsafe.SizeOf<T>();
			if ((size_t)byteLen % (size_t)elementByteLength != 0) {
				throw new InvalidOperationException($"This SDL allocation's byte length is not evenly divisible by {typeof(T).Name}'s size.");
			}

			size_t elementCount = (size_t)byteLen / (size_t)elementByteLength;
			if (elementCount > (size_t)SDL_MAX_SINT32) {
				throw new InvalidOperationException($"This SDL allocation is too large to expose as a Span<{typeof(T).Name}>.");
			}

			return new Span<T>((vptr)handle, checked((int)elementCount));
		}

		public T Read<T>(SDLSize byteOffset) where T : unmanaged
		{
			if (!TryRead(byteOffset, out T value)) {
				throw new InvalidOperationException($"Cannot read {RuntimeUnsafe.SizeOf<T>()} byte(s) at offset {(ulong)byteOffset}");
			}

			return value;
		}

		public T Read<T>(SDLMemoryCursor cursor) where T : unmanaged
		{
			return Read<T>(GetCursorByteOffset(cursor));
		}

		public bool TryRead<T>(SDLSize byteOffset, out T value) where T : unmanaged
		{
			ThrowIfInvalidOrClosed();

			SDLSize byteCount = SizeOf<T>();
			if (!CanAccessRange(byteOffset, byteCount)) {
				value = default;

				return false;
			}

			value = RuntimeUnsafe.ReadUnaligned<T>(GetPointerAtOffset(byteOffset));

			return true;
		}

		public bool TryRead<T>(SDLMemoryCursor? cursor, out T value) where T : unmanaged
		{
			if (!TryGetCursorByteOffset(cursor, out SDLSize byteOffset)) {
				value = default;

				return false;
			}

			return TryRead(byteOffset, out value);
		}

		public void Write<T>(SDLSize byteOffset, T value) where T : unmanaged
		{
			if (!TryWrite(byteOffset, value)) {
				throw new InvalidOperationException($"Cannot write {RuntimeUnsafe.SizeOf<T>()} byte(s) at offset {(ulong)byteOffset}");
			}
		}

		public void Write<T>(SDLMemoryCursor cursor, T value) where T : unmanaged
		{
			Write(GetCursorByteOffset(cursor), value);
		}

		public bool TryWrite<T>(SDLSize byteOffset, T value) where T : unmanaged
		{
			ThrowIfInvalidOrClosed();

			SDLSize byteCount = SizeOf<T>();
			if (!CanAccessRange(byteOffset, byteCount)) {
				return false;
			}

			RuntimeUnsafe.WriteUnaligned(GetPointerAtOffset(byteOffset), value);

			return true;
		}

		public bool TryWrite<T>(SDLMemoryCursor? cursor, T value) where T : unmanaged
		{
			if (TryGetCursorByteOffset(cursor, out SDLSize byteOffset) && TryWrite(byteOffset, value)) {
				return true;
			}

			return false;
		}

		public void Read(SDLSize byteOffset, Span<byte> dest)
		{
			if (!TryRead(byteOffset, dest)) {
				throw new InvalidOperationException($"Cannot read {dest.Length} byte(s) at offset {(ulong)byteOffset}.");
			}
		}

		public void Read(SDLMemoryCursor cursor, Span<byte> dest)
		{
			Read(GetCursorByteOffset(cursor), dest);
		}

		public bool TryRead(SDLSize byteOffset, Span<byte> dest)
		{
			ThrowIfInvalidOrClosed();

			SDLSize byteCnt = new SDLSize((size_t)dest.Length);
			if (!CanAccessRange(byteOffset, byteCnt)) {
				return false;
			}

			if (dest.Length != 0) {
				new ReadOnlySpan<byte>(GetPointerAtOffset(byteOffset), dest.Length).CopyTo(dest);
			}

			return true;
		}

		public bool TryRead(SDLMemoryCursor? cursor, Span<byte> dest)
		{
			if (TryGetCursorByteOffset(cursor, out SDLSize byteOffset) && TryRead(byteOffset, dest)) {
				return true;
			}

			return false;
		}

		public void Write(SDLSize byteOffset, ReadOnlySpan<byte> src)
		{
			if (!TryWrite(byteOffset, src)) {
				throw new InvalidOperationException($"Cannot write {src.Length} byte(s) at offset {(ulong)byteOffset}.");
			}
		}

		public void Write(SDLMemoryCursor cursor, ReadOnlySpan<byte> src)
		{
			Write(GetCursorByteOffset(cursor), src);
		}

		public bool TryWrite(SDLSize byteOffset, ReadOnlySpan<byte> src)
		{
			ThrowIfInvalidOrClosed();

			SDLSize byteCnt = new SDLSize((size_t)src.Length);
			if (!CanAccessRange(byteOffset, byteCnt)) {
				return false;
			}

			if (src.Length != 0) {
				src.CopyTo(new Span<byte>(GetPointerAtOffset(byteOffset), src.Length));
			}

			return true;
		}

		public bool TryWrite(SDLMemoryCursor? cursor, ReadOnlySpan<byte> src)
		{
			if (TryGetCursorByteOffset(cursor, out SDLSize byteOffset) && TryWrite(byteOffset, src)) {
				return true;
			}

			return false;
		}

		protected void ThrowIfInvalidOrClosed()
		{
			if (IsClosed) {
				throw new ObjectDisposedException(GetType().Name);
			}

			if (IsInvalid) {
				throw new InvalidOperationException("The SDL memory handle is invalid.");
			}
		}

		internal void ThrowIfInvalidOrClosedForCursor()
		{
			ThrowIfInvalidOrClosed();
		}

		private bool CanAccessRange(SDLSize byteOffset, SDLSize byteCnt)
		{
			if (byteOffset > byteLen) {
				return false;
			}

			if (byteCnt <= (byteLen - byteOffset)) {
				return true;
			}

			return false;
		}

		private SDLSize GetCursorByteOffset(SDLMemoryCursor cursor)
		{
			ArgumentNullException.ThrowIfNull(cursor);

			if (!ReferenceEquals(cursor.Owner, this)) {
				throw new InvalidOperationException("The SDL memory cursor is not attached to this memory block.");
			}

			return cursor.ByteOffset;
		}

		private bool TryGetCursorByteOffset(SDLMemoryCursor? cursor, out SDLSize byteOffset)
		{
			if (cursor == null || !ReferenceEquals(cursor.Owner, this)) {
				byteOffset = default;

				return false;
			}

			byteOffset = cursor.ByteOffset;

			return true;
		}

		private bptr GetPointerAtOffset(SDLSize byteOffset)
		{
			bptr basePointer = (bptr)handle;

			return (bptr)(basePointer + (size_t)byteOffset);
		}

		private static SDLSize SizeOf<T>() where T : unmanaged
		{
			return new SDLSize((size_t)RuntimeUnsafe.SizeOf<T>());
		}

		protected void MarkHandleReleased()
		{
			handle = mptr.Zero;
			byteLen = SDLSize.Zero;
			isAligned = false;
		}

		protected void ReplaceHandle(VoidPtr newMem, SDLSize newByteLen)
		{
			if (newMem.IsNull) {
				throw new ArgumentException("Cannot replace an SDL memory handle with a null pointer.", nameof(newMem));
			}

			handle = (mptr)newMem;
			byteLen = newByteLen;
			isAligned = newMem.IsAligned;
		}
	}

	/// <summary>
	///	SDLMemoryBlock - Owns native memory allocated by native SDL_malloc, SDL_calloc, and SDL_realloc function calls
	/// </summary>
	public sealed unsafe class SDLMemoryBlock : SDLMemoryHandle
	{
		private SDLSize elementCnt;
		private SDLSize elementSize;

		public SDLSize ElementCount
		{
			get
			{
				return elementCnt;
			}
		}

		public SDLSize ElementSize
		{
			get
			{
				return elementSize;
			}
		}

		internal SDLMemoryBlock(VoidPtr mem, SDLSize byteLen) : this(mem, byteLen, byteLen, SDLSize.One) {}

		internal SDLMemoryBlock(VoidPtr mem, SDLSize byteLen, SDLSize elementCnt, SDLSize elementSize) : base(RequireNormalMemory(mem), byteLen)
		{
			RequireElementShape(byteLen, elementCnt, elementSize);
			this.elementCnt = elementCnt;
			this.elementSize = elementSize;
		}

		/// <summary>
		///	Attempts to reallocate a normal SDL memory block to a new sized block
		/// </summary>
		public void Reallocate(SDLSize newByteLen)
		{
			ThrowIfInvalidOrClosed();

			if (!SDL3_MemoryUnsafe.TryRealloc(Pointer, newByteLen, out VoidPtr newMem)) {
				throw new OutOfMemoryException($"SDL_realloc failed to resize the memory block to {(ulong)newByteLen} byte(s).");
			}

			ReplaceHandle(newMem, newByteLen);
			SetElementShape(newByteLen, SDLSize.One);
		}

		public void Reallocate(SDLSize newElementCnt, SDLSize newElementSize)
		{
			SDLSize newByteLen = GetElementByteLength(newElementCnt, newElementSize);
			ThrowIfInvalidOrClosed();

			if (!SDL3_MemoryUnsafe.TryRealloc(Pointer, newByteLen, out VoidPtr newMem)) {
				throw new OutOfMemoryException($"SDL_realloc failed to resize the memory block to {(ulong)newElementCnt} byte(s).");
			}

			ReplaceHandle(newMem, newByteLen);
			SetElementShape(newElementCnt, newElementSize);
		}

		/// <summary>
		///	Returns false if the attempt to reallocate the memory failed, true if the attempt to reallocate
		///	succeeded. If this function returns false, the original block remains unchanged and still owns
		///	the original memory.
		/// </summary>
		public bool TryReallocate(SDLSize newByteLen)
		{
			ThrowIfInvalidOrClosed();

			if (!SDL3_MemoryUnsafe.TryRealloc(Pointer, newByteLen, out VoidPtr newMem)) {
				return false;
			}

			ReplaceHandle(newMem, newByteLen);
			SetElementShape(newByteLen, SDLSize.One);

			return true;
		}

		public bool TryReallocate(SDLSize newElementCnt, SDLSize newElementSize)
		{
			SDLSize newByteLen = GetElementByteLength(newElementCnt, newElementSize);

			ThrowIfInvalidOrClosed();

			if (!SDL3_MemoryUnsafe.TryRealloc(Pointer, newByteLen, out VoidPtr newMem)) {
				return false;
			}

			ReplaceHandle(newMem, newByteLen);
			SetElementShape(newElementCnt, newElementSize);

			return true;
		}

		public void Free()
		{
			Dispose();
		}

		public void ReleaseMemory()
		{
			Free();
		}

		protected override bool ReleaseHandle()
		{
			if (IsInvalid) {
				return true;
			}

			SDL3_MemoryUnsafe.Free(new VoidPtr((vptr)handle, IsAligned));
			MarkHandleReleased();

			return true;
		}

		private static VoidPtr RequireNormalMemory(VoidPtr mem)
		{
			if (mem.IsAligned) {
				throw new ArgumentException("SDLMemoryBlock cannot own memory allocated by SDL_aligned_alloc.", nameof(mem));
			}

			return mem;
		}

		private static SDLSize GetElementByteLength(SDLSize elementCnt, SDLSize elementSize)
		{
			if (!SDLSize.TryMultiply(elementCnt, elementSize, out SDLSize byteLen)) {
				throw new OverflowException("The requested SDL memory array byte length overflowed size_t.");
			}

			return byteLen;
		}

		private static void RequireElementShape(SDLSize byteLen, SDLSize elementCnt, SDLSize elementSize)
		{
			SDLSize expectedByteLen = GetElementByteLength(elementCnt, elementSize);
			if (expectedByteLen != byteLen) {
				throw new ArgumentException("The SDL memory element shape does not match the byte length.");
			}
		}

		private void SetElementShape(SDLSize newElementCnt, SDLSize newElementSize)
		{
			RequireElementShape(ByteLength, newElementCnt, newElementSize);
			elementCnt = newElementCnt;
			elementSize = newElementSize;
		}
	}

	/// <summary>
	///	SDLAlignedMemoryBlock - This owns memory that has been allocated by SDL_aligned_alloc. Unlike the normal
	///	SDLMemoryBlock, SDLAlignedMemoryBlock intentionally does not expose a Reallocate function for this type
	///	because SDL does not provide a SDL_aligned_realloc function.
	/// </summary>
	public sealed unsafe class SDLAlignedMemoryBlock : SDLMemoryHandle
	{
		private readonly SDLSize alignment;

		public SDLSize Alignment
		{
			get
			{
				return alignment;
			}
		}

		public bool IsCorrectlyAligned
		{
			get
			{
				ThrowIfInvalidOrClosed();

				if (alignment.IsZero) {
					return false;
				}

				if (((ptr)Pointer.Address % (size_t)alignment) == 0) {
					return true;
				}

				return false;
			}
		}

		internal SDLAlignedMemoryBlock(VoidPtr mem, SDLSize byteLen, SDLSize alignment) : base(RequireAlignedMemory(mem), byteLen)
		{
			this.alignment = alignment;
		}

		public void Free()
		{
			Dispose();
		}

		public void ReleaseMemory()
		{
			Free();
		}

		protected override bool ReleaseHandle()
		{
			if (IsInvalid) {
				return true;
			}

			SDL3_MemoryUnsafe.AlignedFree(new VoidPtr((vptr)handle, IsAligned));
			MarkHandleReleased();

			return true;
		}

		private static VoidPtr RequireAlignedMemory(VoidPtr mem)
		{
			if (!mem.IsAligned) {
				throw new ArgumentException("SDLAlginedMemoryBlock requires memory allocated by SDL_aligned_alloc.");
			}

			return mem;
		}
	}
}
