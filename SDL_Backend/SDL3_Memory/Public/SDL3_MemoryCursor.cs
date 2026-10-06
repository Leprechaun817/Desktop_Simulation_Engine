using System;
using System.Diagnostics.CodeAnalysis;
using SDL3.Unsafe;

namespace SDL3
{
	/// <summary>
	///	A mutable, owner-backed cursor type that allows for controlled bounded reads and writes
	///	inside SDL memory.
	/// </summary>
	public sealed unsafe class SDLMemoryCursor
	{
		private SDLMemoryHandle? owner;
		private SDLSize byteOffset;

		public bool IsAttached
		{
			get
			{
				if (owner != null) {
					return true;
				}

				return false;
			}
		}

		public SDLMemoryHandle? Owner
		{
			get
			{
				return owner;
			}
		}

		public SDLSize ByteOffset
		{
			get
			{
				return byteOffset;
			}
		}

		public SDLSize RemainingBytes
		{
			get
			{
				SDLMemoryHandle currentOwner = GetValidatedOwner();

				return currentOwner.ByteLength - byteOffset;
			}
		}

		public bool IsAtStart
		{
			get
			{
				GetValidatedOwner();

				if (byteOffset == SDLSize.Zero) {
					return true;
				}

				return false;
			}
		}

		public bool IsAtEnd
		{
			get
			{
				SDLMemoryHandle currentOwner = GetValidatedOwner();

				if (byteOffset == GetLastByteOffset(currentOwner)) {
					return true;
				}

				return false;
			}
		}

		public SDLBorrowedMemoryPointer Pointer
		{

			get
			{
				return GetRawMemoryAddress();
			}
		}

		public SDLConstBorrowedMemoryPointer ConstPointer
		{
			get
			{
				return GetRawConstMemoryAddress();
			}
		}

		public SDLMemoryCursor() {}

		public SDLMemoryCursor(SDLMemoryHandle owner) : this(owner, SDLSize.Zero) {}

		public SDLMemoryCursor(SDLMemoryHandle owner, SDLSize byteOffset)
		{
			Attach(owner, byteOffset);
		}

		public SDLMemoryCursor Attach(SDLMemoryHandle owner)
		{
			return Attach(owner, SDLSize.Zero);
		}

		public SDLMemoryCursor Attach(SDLMemoryHandle owner, SDLSize byteOffset)
		{
			ArgumentNullException.ThrowIfNull(owner);
			owner.ThrowIfInvalidOrClosedForCursor();

			if (owner.ByteLength.IsZero) {
				throw new InvalidOperationException("A cursor cannot attach to a zero-byte SDL memory block.");
			}

			if (byteOffset >= owner.ByteLength) {
				throw new ArgumentOutOfRangeException(nameof(byteOffset), "The cursor offset must point at a byte inside the SDL memory block.");
			}

			this.owner = owner;
			this.byteOffset = byteOffset;

			return this;
		}

		public bool TryAttach(SDLMemoryHandle? owner)
		{
			return TryAttach(owner, SDLSize.Zero);
		}

		public bool TryAttach(SDLMemoryHandle? owner, SDLSize byteOffset)
		{
			if (!CanAttach(owner, byteOffset)) {
				return false;
			}

			this.owner = owner;
			this.byteOffset = byteOffset;

			return true;
		}

		public SDLMemoryCursor Detach()
		{
			owner = null;
			byteOffset = SDLSize.Zero;

			return this;
		}

		public SDLBorrowedMemoryPointer GetRawMemoryAddress()
		{
			SDLMemoryHandle currentOwner = GetValidatedOwner();

			return new SDLBorrowedMemoryPointer(GetPointerAtCurrentOffset(currentOwner), currentOwner.IsAligned, byteOffset);
		}

		public SDLConstBorrowedMemoryPointer GetRawConstMemoryAddress()
		{
			SDLMemoryHandle currentOwner = GetValidatedOwner();

			return new SDLConstBorrowedMemoryPointer(GetPointerAtCurrentOffset(currentOwner), currentOwner.IsAligned, byteOffset);
		}

		public bool CanAdvance(SDLSize byteCnt)
		{
			return CanMoveForward(byteCnt);
		}

		public bool TryAdvance(SDLSize byteCnt)
		{
			if (!CanAdvance(byteCnt)) {
				return false;
			}

			byteOffset += byteCnt;

			return true;
		}

		public SDLMemoryCursor Advance(SDLSize byteCnt)
		{
			GetValidatedOwner();

			if (!TryAdvance(byteCnt)) {
				throw new ArgumentOutOfRangeException(nameof(byteCnt), "The cursor cannot advance past the SDL memory block length.");
			}

			return this;
		}

		public bool CanRewind(SDLSize byteCnt)
		{
			return CanMoveBackward(byteCnt);
		}

		public bool TryRewind(SDLSize byteCnt)
		{
			if (!CanRewind(byteCnt)) {
				return false;
			}

			byteOffset -= byteCnt;

			return true;
		}

		public SDLMemoryCursor Rewind(SDLSize byteCnt)
		{
			GetValidatedOwner();

			if (!TryRewind(byteCnt)) {
				throw new ArgumentOutOfRangeException(nameof(byteCnt), "The cursor cannot rewind before the start of the SDL memory block.");
			}

			return this;
		}

		public bool TrySeek(SDLSize newByteOffset)
		{
			if (!TryGetUsableOwner(out SDLMemoryHandle? currentOwner)) {
				return false;
			}

			if (!IsValidOffset(currentOwner, newByteOffset)) {
				return false;
			}

			byteOffset = newByteOffset;

			return true;
		}

		public SDLMemoryCursor Seek(SDLSize newByteOffset)
		{
			GetAttachedUsableOwner();

			if (!TrySeek(newByteOffset)) {
				throw new ArgumentOutOfRangeException(nameof(newByteOffset), "The cursor offset must point at a byte inside the SDL memory block.");
			}

			return this;
		}

		public SDLMemoryCursor Reset()
		{
			GetAttachedUsableOwner();
			byteOffset = SDLSize.Zero;

			return this;
		}

		public SDLMemoryCursor MoveToEnd()
		{
			SDLMemoryHandle currentOwner = GetAttachedUsableOwner();
			byteOffset = GetLastByteOffset(currentOwner);

			return this;
		}

		public T Read<T>() where T : unmanaged
		{
			if (!TryRead(out T value)) {
				throw new InvalidOperationException($"The cursor cannot read {RuntimeUnsafe.SizeOf<T>()} byte(s) at offset {(ulong)byteOffset}.");
			}

			return value;
		}

		public bool TryRead<T>(out T value) where T : unmanaged
		{
			SDLSize byteCnt = SizeOf<T>();
			if (!CanAccess(byteCnt)) {
				value = default;

				return false;
			}

			value = RuntimeUnsafe.ReadUnaligned<T>(GetPointerAtCurrentOffset(GetValidatedOwner()));

			return true;
		}

		public void Write<T>(T value) where T : unmanaged
		{
			if (!TryWrite(value)) {
				throw new InvalidOperationException($"The cursor cannot write {RuntimeUnsafe.SizeOf<T>()} byte(s) at offset {(ulong)byteOffset}.");
			}
		}

		public bool TryWrite<T>(T value) where T : unmanaged
		{
			SDLSize byteCnt = SizeOf<T>();
			if (!CanAccess(byteCnt)) {
				return false;
			}

			RuntimeUnsafe.WriteUnaligned(GetPointerAtCurrentOffset(GetValidatedOwner()), value);

			return true;
		}

		public T ReadAndAdvance<T>() where T : unmanaged
		{
			if (!TryReadAndAdvance(out T value)) {
				throw new InvalidOperationException($"The cursor cannot read {RuntimeUnsafe.SizeOf<T>()} byte(s) at offset {(ulong)byteOffset}.");
			}

			return value;
		}

		public bool TryReadAndAdvance<T>(out T value) where T : unmanaged
		{
			SDLSize byteCnt = SizeOf<T>();
			if (!CanAccess(byteCnt) || !TryAdvanceAfterAccess(byteCnt)) {
				value = default;

				return false;
			}

			value = RuntimeUnsafe.ReadUnaligned<T>(GetPointerAtCurrentOffset(GetValidatedOwner()));
			byteOffset += byteCnt;

			return true;
		}

		public void WriteAndAdvance<T>(T value) where T : unmanaged
		{
			if (!TryWriteAndAdvance(value)) {
				throw new InvalidOperationException($"The cursor cannot write {RuntimeUnsafe.SizeOf<T>()} byte(s) at offset {(ulong)byteOffset}.");
			}
		}

		public bool TryWriteAndAdvance<T>(T value) where T : unmanaged
		{
			SDLSize byteCnt = SizeOf<T>();
			if (!CanAccess(byteCnt) || !TryAdvanceAfterAccess(byteCnt)) {
				return false;
			}

			RuntimeUnsafe.WriteUnaligned(GetPointerAtCurrentOffset(GetValidatedOwner()), value);
			byteOffset += byteCnt;

			return true;
		}

		public void Read(Span<byte> dest)
		{
			if (!TryRead(dest)) {
				throw new InvalidOperationException($"The cursor cannot read {dest.Length} byte(s) at offset {(ulong)byteOffset}.");
			}
		}

		public bool TryRead(Span<byte> dest)
		{
			SDLSize byteCnt = new((size_t)dest.Length);
			if (!CanAccess(byteCnt)) {
				return false;
			}

			if (dest.Length == 0) {
				return true;
			}

			new ReadOnlySpan<byte>(GetPointerAtCurrentOffset(GetValidatedOwner()), dest.Length).CopyTo(dest);

			return true;
		}

		public void Write(ReadOnlySpan<byte> src)
		{
			if (!TryWrite(src)) {
				throw new InvalidOperationException($"The cursor cannot write {src.Length} byte(s) at offset {(ulong)byteOffset}");
			}
		}

		public bool TryWrite(ReadOnlySpan<byte> src)
		{
			SDLSize byteCnt = new((size_t)src.Length);
			if (!CanAccess(byteCnt)) {
				return false;
			}

			if (src.Length == 0) {
				return true;
			}

			src.CopyTo(new Span<byte>(GetPointerAtCurrentOffset(GetValidatedOwner()), src.Length));

			return true;
		}

		public void ReadAndAdvance(Span<byte> dest)
		{
			if (!TryReadAndAdvance(dest)) {
				throw new InvalidOperationException($"The cursor cannot read {dest.Length} byte(s) at offset {(ulong)byteOffset}.");
			}
		}

		public bool TryReadAndAdvance(Span<byte> dest)
		{
			SDLSize byteCnt = new SDLSize((size_t)dest.Length);
			if (!CanAccess(byteCnt) || !TryAdvanceAfterAccess(byteCnt)) {
				return false;
			}

			if (dest.Length != 0) {
				new ReadOnlySpan<byte>(GetPointerAtCurrentOffset(GetValidatedOwner()), dest.Length).CopyTo(dest);
			}

			byteOffset += byteCnt;

			return true;
		}

		public void WriteAndAdvance(ReadOnlySpan<byte> src)
		{
			if (!TryWriteAndAdvance(src)) {
				throw new InvalidOperationException($"The cursor cannot write {src.Length} byte(s) at offset {(ulong)byteOffset}");
			}
		}

		public bool TryWriteAndAdvance(ReadOnlySpan<byte> src)
		{
			SDLSize byteCnt = new SDLSize((size_t)src.Length);
			if (!CanAccess(byteCnt) || !TryAdvanceAfterAccess(byteCnt)) {
				return false;
			}

			if (src.Length != 0) {
				src.CopyTo(new Span<byte>(GetPointerAtCurrentOffset(GetValidatedOwner()), src.Length));
			}

			byteOffset += byteCnt;

			return true;
		}

		public static SDLMemoryCursor operator ++(SDLMemoryCursor cursor)
		{
			ArgumentNullException.ThrowIfNull(cursor);

			return cursor.Advance(SDLSize.One);
		}

		public static SDLMemoryCursor operator --(SDLMemoryCursor cursor)
		{
			ArgumentNullException.ThrowIfNull(cursor);

			return cursor.Rewind(SDLSize.One);
		}

		private bool CanAccess(SDLSize byteCnt)
		{
			if (!TryGetUsableOwner(out SDLMemoryHandle? currentOwner)) {
				return false;
			}

			if (IsValidOffset(currentOwner, byteOffset) && byteCnt <= (currentOwner.ByteLength - byteOffset)) {
				return true;
			}

			return false;
		}

		private bool CanMoveForward(SDLSize byteCnt)
		{
			if (!TryGetUsableOwner(out SDLMemoryHandle? currentOwner)) {
				return false;
			}

			if (IsValidOffset(currentOwner, byteOffset) && (byteCnt <= GetLastByteOffset(currentOwner) - byteOffset)) {
				return true;
			}

			return false;
		}

		private bool CanMoveBackward(SDLSize byteCnt)
		{
			if (!TryGetUsableOwner(out SDLMemoryHandle? currentOwner)) {
				return false;
			}

			if (IsValidOffset(currentOwner, byteOffset) && byteCnt <= byteOffset) {
				return true;
			}

			return false;
		}

		private bool TryAdvanceAfterAccess(SDLSize byteCnt)
		{
			if (byteCnt.IsZero) {
				return true;
			}

			return CanMoveForward(byteCnt);
		}

		private SDLMemoryHandle GetValidatedOwner()
		{
			SDLMemoryHandle currentOwner = GetAttachedUsableOwner();

			if (!IsValidOffset(currentOwner, byteOffset)) {
				throw new InvalidOperationException("The cursor offset is outside the current SDL memory block.");
			}

			return currentOwner;
		}

		private SDLMemoryHandle GetAttachedUsableOwner()
		{
			if (owner == null) {
				throw new InvalidOperationException("The SDL memory cursor is not attached to a memory block");
			}

			owner.ThrowIfInvalidOrClosedForCursor();

			if (owner.ByteLength.IsZero) {
				throw new InvalidOperationException("The SDL memory cursor is attached to a zero-byte memory block.");
			}

			return owner;
		}

		private bool TryGetUsableOwner([NotNullWhen(true)] out SDLMemoryHandle? currentOwner) {
			currentOwner = owner;
			if (currentOwner == null || currentOwner.IsReleased || currentOwner.ByteLength.IsZero) {
				return false;
			}

			return true;
		}

		private static bool CanAttach(SDLMemoryHandle? owner, SDLSize byteOffset) {
			if (owner != null && !owner.IsReleased && !owner.ByteLength.IsZero && IsValidOffset(owner, byteOffset)) {
				return true;
			}

			return false;
		}

		private bptr GetPointerAtCurrentOffset(SDLMemoryHandle currentOwner) {
			bptr basePointer = (bptr)currentOwner.Pointer;

			return (bptr)(basePointer + (size_t)byteOffset);
		}

		private static bool IsValidOffset(SDLMemoryHandle owner, SDLSize offset)
		{
			if (!owner.ByteLength.IsZero && offset < owner.ByteLength) {
				return true;
			}

			return false;
		}

		private static SDLSize GetLastByteOffset(SDLMemoryHandle owner)
		{
			return owner.ByteLength - SDLSize.One;
		}

		private static SDLSize SizeOf<T>() where T : unmanaged 
		{
			return new SDLSize((size_t)RuntimeUnsafe.SizeOf<T>());	
		}
	}
}