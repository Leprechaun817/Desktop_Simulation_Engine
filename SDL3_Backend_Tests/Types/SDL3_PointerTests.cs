using System;
using System.Reflection;
using SDL3.Unsafe;
using Xunit;

namespace SDL3_Backend_Tests
{
	namespace Types
	{
		public sealed unsafe class SDLPointerTests
		{
			[Fact]
			public void OwningPointers_ShouldExposeNullAndRoundTripAddresses()
			{
				int value = 42;
				VoidPtr pointer = new VoidPtr(&value);
				Assert.True(VoidPtr.Null.IsNull);
				Assert.True(ConstVoidPtr.Null.IsNull);
				Assert.Equal(default, VoidPtr.Null);
				Assert.Equal(default, ConstVoidPtr.Null);
				Assert.False(pointer.IsNull);

				Assert.Equal((nint)(&value), pointer.Address);
				Assert.Equal(pointer, (VoidPtr)(nint)pointer);
				Assert.True((void*)pointer == &value);
				Assert.Equal($"0x{pointer.Address:x}", pointer.ToString());

				ConstVoidPtr constantPointer = ConstVoidPtr.From(&value);
				Assert.False(constantPointer.IsNull);
				Assert.Equal(pointer.Address, constantPointer.Address);
				Assert.Equal(constantPointer.Address, (nint)constantPointer);
				Assert.True((void*)constantPointer == &value);
				Assert.Equal(pointer.ToString(), constantPointer.ToString());
			}

			[Fact]
			public void AlignmentFlags_ShouldBeProvenanceAndNotChangeOwningPointerEquality()
			{
				int value = 42;
				VoidPtr normal = new VoidPtr(&value);
				VoidPtr aligned = VoidPtr.FromAligned(&value);
				Assert.False(normal.IsAligned);
				Assert.True(aligned.IsAligned);
				Assert.True(normal.WithAlignmentFlag(true).IsAligned);
				Assert.False(aligned.WithAlignmentFlag(false).IsAligned);
				Assert.Equal(normal, aligned);
				Assert.True(normal == aligned);
				Assert.False(normal != aligned);
				Assert.True(normal.Equals((object)aligned));
				Assert.Equal(normal.GetHashCode(), aligned.GetHashCode());
				Assert.NotEqual(normal, VoidPtr.Null);
				Assert.False(normal.Equals(null));
				Assert.False(normal.Equals((object)normal.Address));

				ConstVoidPtr constNormal = ConstVoidPtr.From(normal);
				ConstVoidPtr constAligned = ConstVoidPtr.From(aligned);
				Assert.False(constNormal.IsAligned);
				Assert.True(constAligned.IsAligned);
				Assert.True(ConstVoidPtr.FromAligned(&value).IsAligned);
				Assert.True(constNormal.WithAlignmentFlag(true).IsAligned);
				Assert.False(constAligned.WithAlignmentFlag(false).IsAligned);
				Assert.True(constNormal == constAligned);
				Assert.False(constNormal != constAligned);
				Assert.True(constNormal.Equals((object)constAligned));
				Assert.Equal(constNormal.GetHashCode(), constAligned.GetHashCode());
				Assert.NotEqual(constNormal, ConstVoidPtr.Null);
				Assert.False(constNormal.Equals(null));
				Assert.False(constNormal.Equals(normal));
			}

			[Fact]
			public void BorrowedPointers_ShouldIncludeProvenanceAndOffsetInEquality()
			{
				byte* memory = stackalloc byte[8];
				SDLBorrowedMemoryPointer pointer = new SDLBorrowedMemoryPointer(memory + 2, true, new SDLSize(2));
				SDLBorrowedMemoryPointer pointerCopy = new SDLBorrowedMemoryPointer(memory + 2, true, new SDLSize(2));
				Assert.False(pointer.IsNull);
				Assert.True(pointer.IsAligned);
				Assert.Equal(new SDLSize(2), pointer.ByteOffset);
				Assert.Equal((nint)(memory + 2), pointer.Address);
				Assert.True(pointer.DangerousGetPointer() == memory + 2);
				Assert.True(pointer == pointerCopy);
				Assert.False(pointer != pointerCopy);
				Assert.True(pointer.Equals((object)pointerCopy));
				Assert.Equal(pointer.GetHashCode(), pointerCopy.GetHashCode());

				Assert.NotEqual(pointer, new SDLBorrowedMemoryPointer(memory + 2, false, new SDLSize(2)));
				Assert.NotEqual(pointer, new SDLBorrowedMemoryPointer(memory + 2, true, new SDLSize(1)));
				Assert.NotEqual(pointer, new SDLBorrowedMemoryPointer(memory + 3, true, new SDLSize(2)));

				Assert.False(pointer.Equals(null));
				Assert.False(pointer.Equals((object)pointer.Address));
				Assert.True(default(SDLBorrowedMemoryPointer).IsNull);
				Assert.Equal($"0x{pointer.Address:x}", pointer.ToString());

				SDLConstBorrowedMemoryPointer constant = new SDLConstBorrowedMemoryPointer(memory + 2, true, new SDLSize(2));
				SDLConstBorrowedMemoryPointer constCopy = new SDLConstBorrowedMemoryPointer(memory + 2, true, new SDLSize(2));
				Assert.False(constant.IsNull);
				Assert.True(constant.IsAligned);
				Assert.Equal(pointer.ByteOffset, constant.ByteOffset);
				Assert.True(constant.DangerousGetPointer() == memory + 2);
				Assert.True(constant == constCopy);
				Assert.False(constant != constCopy);
				Assert.True(constant.Equals((object)constCopy));
				Assert.Equal(constant.GetHashCode(), constCopy.GetHashCode());
				
				Assert.NotEqual(constant, new SDLConstBorrowedMemoryPointer(memory + 2, false, new SDLSize(2)));
				Assert.NotEqual(constant, new SDLConstBorrowedMemoryPointer(memory + 2, true, new SDLSize(1)));
				Assert.NotEqual(constant, new SDLConstBorrowedMemoryPointer(memory + 3, true, new SDLSize(2)));

				Assert.False(constant.Equals(null));
				Assert.True(default(SDLConstBorrowedMemoryPointer).IsNull);
				Assert.Equal(pointer.ToString(), constant.ToString());
			}

			[Theory]
			[InlineData(typeof(SDLBorrowedMemoryPointer))]
			[InlineData(typeof(SDLConstBorrowedMemoryPointer))]
			public void BorrowedPointers_ShouldNotExposeAllocatorCompatibleConversions(Type pointerType)
			{
				foreach (MethodInfo method in pointerType.GetMethods(BindingFlags.Public | BindingFlags.Static)) {
					if (method.Name != "op_Implicit" || method.Name != "op_Explicit") {
						continue;
					}

					Assert.NotEqual(typeof(VoidPtr), method.ReturnType);
					Assert.NotEqual(typeof(ConstVoidPtr), method.ReturnType);
					foreach (ParameterInfo parameter in method.GetParameters()) {
						Assert.NotEqual(typeof(VoidPtr), parameter.ParameterType);
						Assert.NotEqual(typeof(ConstVoidPtr), parameter.ParameterType);
					}
				}
			}
		}
	}
}