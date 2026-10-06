using System;
using SDL3.Native;

namespace SDL3
{
	namespace Unsafe
	{
		/// <summary>
		/// Typed SDL Memory API - Still considered unsafe
		/// This portion of the API layer remains pointer-based, but handles
		/// naming and wrapper types such that intent is communicated more clearly
		/// when compared to the raw-native layer
		/// </summary>
		public static unsafe class SDL3_MemoryUnsafe
		{
			public static VoidPtr Malloc(SDLSize size)
			{
				return new VoidPtr(SDL3_NativeMemory.SDL_malloc((size_t)size));
			}

			public static bool TryMalloc(SDLSize size, out VoidPtr mem)
			{
				mem = Malloc(size);
				return !mem.IsNull;
			}

			public static VoidPtr Calloc(SDLSize elemCnt, SDLSize elemSize)
			{
				return new VoidPtr(SDL3_NativeMemory.SDL_calloc((size_t)elemCnt, (size_t)elemSize));
			}

			public static bool TryCalloc(SDLSize elemCnt, SDLSize elemSize, out VoidPtr mem)
			{
				mem = Calloc(elemCnt, elemSize);
				return !mem.IsNull;
			}

			/// <summary>
			/// Only reallocates memory allocated normally. Using reallocate on memory allocated with
			/// SDL_aligned_alloc function will result in thrown exception.
			/// </summary>
			public static VoidPtr Realloc(VoidPtr mem, SDLSize newSize)
			{
				ThrowIfAlignedForNormalAllocator(mem, nameof(Realloc));

				return new VoidPtr(SDL3_NativeMemory.SDL_realloc((vptr)mem, (size_t)newSize));
			}

			/// <summary>
			/// Reallocates normally allocated memory by SDL_malloc function. If this function
			/// return false, the original memory is still valid and must be freed by the caller.
			/// </summary>
			public static bool TryRealloc(VoidPtr mem, SDLSize newSize, out VoidPtr reallocatedMem)
			{
				reallocatedMem = Realloc(mem, newSize);

				return !reallocatedMem.IsNull;
			}

			public static void Free(VoidPtr mem)
			{
				ThrowIfAlignedForNormalAllocator(mem, nameof(Free));

				SDL3_NativeMemory.SDL_free((vptr)mem);
			}

			public static VoidPtr AlignedAlloc(SDLSize alignment, SDLSize size)
			{
				return VoidPtr.FromAligned(SDL3_NativeMemory.SDL_aligned_alloc((size_t)alignment, (size_t)size));
			}

			public static bool TryAlignedAlloc(SDLSize alignment, SDLSize size, out VoidPtr mem)
			{
				mem = AlignedAlloc(alignment, size);

				return !mem.IsNull;
			}

			public static void AlignedFree(VoidPtr mem)
			{
				ThrowIfNotAlignedForAlignedAllocator(mem, nameof(AlignedFree));

				SDL3_NativeMemory.SDL_aligned_free((vptr)mem);
			}

			public static SDLMemoryFunctionPointerTable GetMemoryFunctions()
			{
				SDL3_NativeMemory.SDL_GetMemoryFunctions(out SDLMallocFunctionPtr mallocFuncPtr, out SDLCallocFunctionPtr callocFuncPtr, out SDLReallocFunctionPtr reallocFuncPtr,
														 out SDLFreeFunctionPtr freeFuncPtr);

				return new SDLMemoryFunctionPointerTable(mallocFuncPtr, callocFuncPtr, reallocFuncPtr, freeFuncPtr);
			}

			public static SDLMemoryFunctionPointerTable GetOriginalMemoryFunctions()
			{
				SDL3_NativeMemory.SDL_GetOriginalMemoryFunctions(out SDLMallocFunctionPtr orgMallocFuncPtr, out SDLCallocFunctionPtr orgCallocFuncPtr,
																 out SDLReallocFunctionPtr orgReallocFuncPtr, out SDLFreeFunctionPtr orgFreeFuncPtr);

				return new SDLMemoryFunctionPointerTable(orgMallocFuncPtr, orgCallocFuncPtr, orgReallocFuncPtr, orgFreeFuncPtr);
			}

			public static bool SetMemoryFunctions(SDLMemoryFunctionPointerTable functionTable)
			{
				functionTable.ThrowIfIncomplete();

				if (SDL3_NativeMemory.SDL_SetMemoryFunctions(functionTable.Malloc, functionTable.Calloc, functionTable.Realloc, functionTable.Free) != 0) {
					return true;
				}

				return false;
			}

			public static bool SetMemoryFunctions(SDLMemoryCallbackAdapterTable adapterTable)
			{
				ArgumentNullException.ThrowIfNull(adapterTable);

				return SetMemoryFunctions(adapterTable.FunctionPointers);
			}

			public static int GetNumAllocations()
			{
				return SDL3_NativeMemory.SDL_GetNumAllocations();
			}

			public static VoidPtr Memcpy(VoidPtr dest, ConstVoidPtr src, SDLSize byteCnt)
			{
				SDL3_NativeMemory.SDL_memcpy((vptr)dest, (vptr)src, (size_t)byteCnt);

				return dest;
			}

			public static VoidPtr Memmove(VoidPtr dest, ConstVoidPtr src, SDLSize byteCnt)
			{
				SDL3_NativeMemory.SDL_memmove((vptr)dest, (vptr)src, (size_t)byteCnt);

				return dest;
			}

			public static VoidPtr Memset(VoidPtr dest, byte value, SDLSize byteCnt)
			{
				SDL3_NativeMemory.SDL_memset((vptr)dest, value, (size_t)byteCnt);

				return dest;
			}

			public static VoidPtr Memset(VoidPtr dest, int value, SDLSize byteCnt)
			{
				SDL3_NativeMemory.SDL_memset((vptr)dest, value, (size_t)byteCnt);

				return dest;
			}

			/// <summary>
			/// Sets dwordCnt 32-bit values, not byteCnt bytes.
			/// </summary>
			public static VoidPtr Memset4(VoidPtr dest, uint value, SDLSize dwordCnt)
			{
				SDL3_NativeMemory.SDL_memset4((vptr)dest, value, (size_t)dwordCnt);

				return dest;
			}

			public static int Memcmp(ConstVoidPtr left, ConstVoidPtr right, SDLSize byteCnt)
			{
				return SDL3_NativeMemory.SDL_memcmp((vptr)left, (vptr)right, (size_t)byteCnt);
			}

			private static void ThrowIfAlignedForNormalAllocator(VoidPtr mem, string operation)
			{
				if (!mem.IsAligned) {
					return;
				}

				throw new InvalidOperationException($"{operation} cannot use SDL_malloc-family APIs for memory allocated by SDL_aligned_alloc.");
			}

			private static void ThrowIfNotAlignedForAlignedAllocator(VoidPtr mem, string operation)
			{
				if (mem.IsNull || mem.IsAligned) {
					return;
				}

				throw new InvalidOperationException($"{operation} requires memory to be allocated by SDL_aligned_alloc.");
			}
		}
	}
}
