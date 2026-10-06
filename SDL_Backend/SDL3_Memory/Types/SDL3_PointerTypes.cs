using System;
using System.Diagnostics.CodeAnalysis;

namespace SDL3
{
	namespace Unsafe
	{
		/// <summary>
		///	A Mutable void pointer wrapper type that represents
		///	the C-style void*/void pointer
		/// </summary>
		public readonly unsafe struct VoidPtr : IEquatable<VoidPtr>
		{
			private readonly vptr pointer;
			private readonly bool isAligned;

			public bool IsNull
			{
				get
				{
					if (pointer == null) {
						return true;
					}

					return false;
				}
			}

			/// <summary>
			/// This is true when the pointer is known to own memory from a SDL_aligned_alloc
			/// function call. This only serves as provenance metadata; This should not be used
			/// in an address-divisibility test.
			/// </summary>
			public bool IsAligned
			{
				get
				{
					return isAligned;
				}
			}

			public mptr Address
			{
				get
				{
					return (mptr)pointer;
				}
			}

			public static VoidPtr Null
			{
				get
				{
					return new VoidPtr(null);
				}
			}

			public VoidPtr(vptr pointer) : this(pointer, false) {}

			public VoidPtr(vptr pointer, bool isAligned)
			{
				this.pointer = pointer;
				this.isAligned = isAligned;
			}

			public static VoidPtr FromAligned(vptr pointer)
			{
				return new VoidPtr(pointer, true);
			}

			public VoidPtr WithAlignmentFlag(bool aligned)
			{
				return new VoidPtr(pointer, aligned);
			}

			public static explicit operator vptr(VoidPtr Pointer)
			{
				return Pointer.pointer;
			}

			public static explicit operator mptr(VoidPtr Pointer)
			{
				return Pointer.Address;
			}

			public static explicit operator VoidPtr(mptr Pointer)
			{
				return new VoidPtr((vptr)Pointer);
			}

			public bool Equals(VoidPtr other)
			{
				if (pointer == other.pointer) {
					return true;
				}

				return false;
			}

			public override bool Equals([NotNullWhen(true)] object? obj)
			{
				if (obj == null || obj.GetType() != typeof(VoidPtr)) {
					return false;
				}

				return Equals((VoidPtr)obj);
			}

			public override int GetHashCode()
			{
				return Address.GetHashCode();
			}

			public override string ToString()
			{
				return $"0x{Address:x}";
			}

			public static bool operator ==(VoidPtr leftOpr, VoidPtr rightOpr)
			{
				return leftOpr.Equals(rightOpr);
			}

			public static bool operator !=(VoidPtr leftOpr, VoidPtr rightOpr)
			{
				return !leftOpr.Equals(rightOpr);
			}
		}

		/// <summary>
		/// Read-Only void pointer wrapper that is meant to represent the C const void*.
		/// Please note that this does not actually make the pointed to memory immutable
		/// </summary>
		public readonly unsafe struct ConstVoidPtr : IEquatable<ConstVoidPtr>
		{
			private readonly vptr pointer;
			private readonly bool isAligned;

			public bool IsNull
			{
				get
				{
					if (pointer == null) {
						return true;
					}

					return false;
				}
			}

			/// <summary>
			/// This is true when the pointer is known to own memory from a SDL_aligned_alloc
			/// function call. This only serves as provenance metadata; This should not be used
			/// in an address-divisibility test.
			/// </summary>
			public bool IsAligned
			{
				get
				{
					return isAligned;
				}
			}

			public mptr Address
			{
				get
				{
					return (mptr)pointer;
				}
			}

			public static ConstVoidPtr Null
			{
				get
				{
					return new ConstVoidPtr(null);
				}
			}

			public ConstVoidPtr(vptr pointer) : this(pointer, false) {}

			public ConstVoidPtr(vptr pointer, bool isAligned)
			{
				this.pointer = pointer;
				this.isAligned = isAligned;
			}

			public static ConstVoidPtr FromAligned(vptr pointer)
			{
				return new ConstVoidPtr(pointer, true);
			}

			public ConstVoidPtr WithAlignmentFlag(bool aligned)
			{
				return new ConstVoidPtr(pointer, aligned);
			}

			public static explicit operator vptr(ConstVoidPtr Pointer)
			{
				return Pointer.pointer;
			}

			public static explicit operator mptr(ConstVoidPtr Pointer)
			{
				return Pointer.Address;
			}

			public static ConstVoidPtr From(VoidPtr pointer)
			{
				return new ConstVoidPtr((vptr)pointer, pointer.IsAligned);
			}

			public static ConstVoidPtr From<T>(T* pointer) where T : unmanaged
			{
				return new ConstVoidPtr(pointer);
			}

			public bool Equals(ConstVoidPtr other)
			{
				if (pointer == other.pointer) {
					return true;
				}

				return false;
			}

			public override bool Equals(object? obj)
			{
				if (obj == null || obj.GetType() != typeof(ConstVoidPtr)) {
					return false;
				}

				return Equals((ConstVoidPtr)obj);
			}

			public override int GetHashCode()
			{
				return Address.GetHashCode();
			}

			public override string ToString()
			{
				return $"0x{Address:x}";
			}

			public static bool operator ==(ConstVoidPtr leftOpr, ConstVoidPtr rightOpr)
			{
				return leftOpr.Equals(rightOpr);
			}

			public static bool operator !=(ConstVoidPtr leftOpr, ConstVoidPtr rightOpr)
			{
				return !leftOpr.Equals(rightOpr);
			}
		}

		/// <summary>
		/// A borrowed writable pointer into an SDL memory block. However, this is not
		/// an owning pointer and cannot resolve into a VoidPtr.
		/// </summary>
		public readonly unsafe struct SDLBorrowedMemoryPointer : IEquatable<SDLBorrowedMemoryPointer>
		{
			private readonly void* pointer;
			private readonly bool isAligned;
			private readonly SDLSize byteOffset;

			public mptr Address
			{
				get
				{
					return (mptr)pointer;
				}
			}

			public bool IsNull
			{
				get
				{
					if (pointer == null) {
						return true;
					}

					return false;
				}
			}

			/// <summary>
			/// This is true when the pointer is known to own memory from a SDL_aligned_alloc
			/// function call. This only serves as provenance metadata; This should not be used
			/// in an address-divisibility test.
			/// </summary>
			public bool IsAligned
			{
				get
				{
					return isAligned;
				}
			}

			public SDLSize ByteOffset
			{
				get
				{
					return byteOffset;
				}
			}

			public SDLBorrowedMemoryPointer(vptr pointer, bool isAligned, SDLSize byteOffset)
			{
				this.pointer = pointer;
				this.isAligned = isAligned;
				this.byteOffset = byteOffset;
			}

			public vptr DangerousGetPointer()
			{
				return pointer;
			}

			public bool Equals(SDLBorrowedMemoryPointer other)
			{
				if (pointer == other.pointer && isAligned == other.isAligned && byteOffset == other.byteOffset) {
					return true;
				}

				return false;
			}

			public override bool Equals(object? obj)
			{
				if (obj == null || obj.GetType() != typeof(SDLBorrowedMemoryPointer)) {
					return false;
				}

				return Equals((SDLBorrowedMemoryPointer)obj);
			}

			public override int GetHashCode()
			{
				return HashCode.Combine(Address, isAligned, byteOffset);
			}

			public override string ToString()
			{
				return $"0x{Address:x}";
			}

			public static bool operator ==(SDLBorrowedMemoryPointer leftOpr, SDLBorrowedMemoryPointer rightOpr)
			{
				return leftOpr.Equals(rightOpr);
			}

			public static bool operator !=(SDLBorrowedMemoryPointer leftOpr, SDLBorrowedMemoryPointer rightOpr)
			{
				return !leftOpr.Equals(rightOpr);
			}
		}

		/// <summary>
		/// A borrowed writable pointer into an SDL memory block. However, this is not
		/// an owning pointer and cannot resolve into a ConstVoidPtr.
		/// </summary>
		public readonly unsafe struct SDLConstBorrowedMemoryPointer : IEquatable<SDLConstBorrowedMemoryPointer>
		{
			private readonly vptr pointer;
			private readonly bool isAligned;
			private readonly SDLSize byteOffset;

			public mptr Address
			{
				get
				{
					return (mptr)pointer;
				}
			}

			public bool IsNull
			{
				get
				{
					if (pointer == null) {
						return true;
					}

					return false;
				}
			}

			/// <summary>
			/// This is true when the pointer is known to own memory from a SDL_aligned_alloc
			/// function call. This only serves as provenance metadata; This should not be used
			/// in an address-divisibility test.
			/// </summary>
			public bool IsAligned
			{
				get
				{
					return isAligned;
				}
			}

			public SDLSize ByteOffset
			{
				get
				{
					return byteOffset;
				}
			}

			public SDLConstBorrowedMemoryPointer(vptr pointer, bool isAligned, SDLSize byteOffset)
			{
				this.pointer = pointer;
				this.isAligned = isAligned;
				this.byteOffset = byteOffset;
			}

			public vptr DangerousGetPointer()
			{
				return pointer;
			}

			public bool Equals(SDLConstBorrowedMemoryPointer other)
			{
				if (pointer == other.pointer && isAligned == other.isAligned && byteOffset == other.byteOffset) {
					return true;
				}

				return false;
			}

			public override bool Equals([NotNullWhen(true)] object? obj)
			{
				if (obj == null || obj.GetType() != typeof(SDLConstBorrowedMemoryPointer)) {
					return false;
				}

				return Equals((SDLConstBorrowedMemoryPointer)obj);
			}

			public override int GetHashCode()
			{
				return HashCode.Combine(Address, isAligned, byteOffset);
			}

			public override string ToString()
			{
				return $"0x{Address:x}";
			}

			public static bool operator ==(SDLConstBorrowedMemoryPointer leftOpr, SDLConstBorrowedMemoryPointer rightOpr)
			{
				return leftOpr.Equals(rightOpr);
			}

			public static bool operator !=(SDLConstBorrowedMemoryPointer leftOpr, SDLConstBorrowedMemoryPointer rightOpr)
			{
				return !leftOpr.Equals(rightOpr);
			}
		}
	}
}