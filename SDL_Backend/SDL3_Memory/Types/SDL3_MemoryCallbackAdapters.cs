using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace SDL3
{
	namespace Unsafe
	{
		/// <summary>
		/// Delegate shape for SDL_malloc_func:
		/// void* (SDLCALL* SDL_malloc_func)(size_t size).
		/// </summary>
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate vptr SDLMallocCallback(size_t size);

		///<summary>
		/// Delegate shape for SDL_calloc_func:
		/// void* (SDLCALL* SDL_calloc_func)(size_t size).
		/// </summary>
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate vptr SDLCallocCallback(size_t nmemb, size_t size);

		/// <summary>
		/// Delegate shape for SDL_realloc_func:
		/// void* (SDLCALL* SDL_realloc_func)(void* mem, size_t size).
		/// </summary>
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate vptr SDLReallocCallback(vptr mem, size_t size);

		/// <summary>
		/// Delegate shape for SDL_free_func:
		/// void (SDLCALL* SDL_free_func)(void* mem).
		/// </summary>
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate void SDLFreeCallback(vptr mem);

		/// <summary>
		///	Core storage for unmanaged SDL memory callback pointers.
		///	This keeps the primary binding shape as delegate* unmanaged[Cdecl], while
		///	still allowing delegate-based adapters to produce the same function pointer
		///	table.
		/// </summary>
		public readonly unsafe struct SDLMemoryFunctionPointerTable : IEquatable<SDLMemoryFunctionPointerTable>
		{
			private readonly SDLMallocFunctionPtr MallocFuncPtr;
			private readonly SDLCallocFunctionPtr CallocFuncPtr;
			private readonly SDLReallocFunctionPtr ReallocFuncPtr;
			private readonly SDLFreeFunctionPtr FreeFuncPtr;

			public SDLMallocFunctionPtr Malloc
			{
				get
				{
					return MallocFuncPtr;
				}
			}

			public SDLCallocFunctionPtr Calloc
			{
				get
				{
					return CallocFuncPtr;
				}
			}

			public SDLReallocFunctionPtr Realloc
			{
				get
				{
					return ReallocFuncPtr;
				}
			}

			public SDLFreeFunctionPtr Free
			{
				get
				{
					return FreeFuncPtr;
				}
			}

			public SDLMemoryFunctionPointerTable(SDLMallocFunctionPtr malloc, SDLCallocFunctionPtr calloc, SDLReallocFunctionPtr realloc, SDLFreeFunctionPtr free)
			{
				MallocFuncPtr = malloc;
				CallocFuncPtr = calloc;
				ReallocFuncPtr = realloc;
				FreeFuncPtr = free;
			}

			public bool IsComplete
			{
				get 
				{
					if (MallocFuncPtr != null && CallocFuncPtr != null && ReallocFuncPtr != null && FreeFuncPtr != null) {
						return true;
					}

					return false;
				}
			}

			public void ThrowIfIncomplete()
			{
				if (IsComplete) {
					return;
				}

				throw new InvalidOperationException("SDL memory callback pointers must include a non-null pointer to a complete set of malloc, calloc, realloc and free functions.");
			}

			public bool Equals(SDLMemoryFunctionPointerTable other)
			{
				if (MallocFuncPtr == other.MallocFuncPtr && CallocFuncPtr == other.CallocFuncPtr && ReallocFuncPtr == other.ReallocFuncPtr && FreeFuncPtr == other.FreeFuncPtr) {
					return true;
				}

				return false;
			}

			public override bool Equals([NotNullWhen(true)] object? obj)
			{
				if (obj == null || obj.GetType() != typeof(SDLMemoryFunctionPointerTable)) {
					return false;
				}

				return Equals((SDLMemoryFunctionPointerTable)obj);
			}

			public override int GetHashCode()
			{
				return HashCode.Combine((nint)(vptr)Malloc, (nint)(vptr)Calloc, (nint)(vptr)Realloc, (nint)(vptr)Free);
			}

			public static bool operator ==(SDLMemoryFunctionPointerTable leftOpr, SDLMemoryFunctionPointerTable rightOpr)
			{
				return leftOpr.Equals(rightOpr);
			}

			public static bool operator !=(SDLMemoryFunctionPointerTable leftOpr, SDLMemoryFunctionPointerTable rightOpr)
			{
				return !leftOpr.Equals(rightOpr);
			}
		}

		/// <summary>
		///	Delegate-backed SDL memory callback table
		///	
		/// An instance of this type should be kept alive for as long as SDL might call any
		/// of the exposed function pointers. The managed delegates are rooted in this object
		/// which means that if this object is GC'd while SDL still holds the function pointers.
		/// SDL may possibly call invalid memory or may cause undefined behavior.
		/// </summary>
		public sealed unsafe class SDLMemoryCallbackAdapterTable
		{
			private readonly SDLMallocCallback mallocClbk;
			private readonly SDLCallocCallback callocClbk;
			private readonly SDLReallocCallback reallocClbk;
			private readonly SDLFreeCallback freeClbk;

			private readonly SDLMallocFunctionPtr mallocFuncPtr;
			private readonly SDLCallocFunctionPtr callocFuncPtr;
			private readonly SDLReallocFunctionPtr reallocFuncPtr;
			private readonly SDLFreeFunctionPtr freeFuncPtr;

			public SDLMallocCallback MallocDelegate
			{
				get
				{
					return mallocClbk;
				}
			}

			public SDLCallocCallback CallocDelegate
			{
				get
				{
					return callocClbk;
				}
			}

			public SDLReallocCallback ReallocDelegate
			{
				get
				{
					return reallocClbk;
				}
			}

			public SDLFreeCallback FreeDelegate
			{
				get
				{
					return freeClbk;
				}
			}

			public SDLMallocFunctionPtr MallocPointer
			{
				get
				{
					return mallocFuncPtr;
				}
			}

			public SDLCallocFunctionPtr CallocPointer
			{
				get
				{
					return callocFuncPtr;
				}
			}

			public SDLReallocFunctionPtr ReallocPointer
			{
				get
				{
					return reallocFuncPtr;
				}
			}

			public SDLFreeFunctionPtr FreePointer
			{
				get
				{
					return freeFuncPtr;
				}
			}

			public SDLMemoryFunctionPointerTable FunctionPointers
			{
				get
				{
					return new SDLMemoryFunctionPointerTable(mallocFuncPtr, callocFuncPtr, reallocFuncPtr, freeFuncPtr);
				}
			}

			public SDLMemoryCallbackAdapterTable(SDLMallocCallback malloc, SDLCallocCallback calloc, SDLReallocCallback realloc, SDLFreeCallback free)
			{
				if (malloc == null) {
					throw new ArgumentNullException(nameof(malloc));
				}
				if (calloc == null) {
					throw new ArgumentNullException(nameof(calloc));
				}
				if (realloc == null) {
					throw new ArgumentNullException(nameof(realloc));
				}
				if (free == null) {
					throw new ArgumentNullException(nameof(free));
				}

				this.mallocClbk = malloc;
				this.callocClbk = calloc;
				this.reallocClbk = realloc;
				this.freeClbk = free;

				mallocFuncPtr = (SDLMallocFunctionPtr)(vptr)Marshal.GetFunctionPointerForDelegate(this.mallocClbk);
				callocFuncPtr = (SDLCallocFunctionPtr)(vptr)Marshal.GetFunctionPointerForDelegate(this.callocClbk);
				reallocFuncPtr = (SDLReallocFunctionPtr)(vptr)Marshal.GetFunctionPointerForDelegate(this.reallocClbk);
				freeFuncPtr = (SDLFreeFunctionPtr)(vptr)Marshal.GetFunctionPointerForDelegate(this.freeClbk);
			}
		}

		/// <summary>
		///	Helpers for converting delegate/unmanaged function pointers from one type to the other.
		/// </summary>
		public static unsafe class SDLMemoryCallbackAdapters
		{
			public static SDLMemoryCallbackAdapterTable Create(SDLMallocCallback mallocClbk, SDLCallocCallback callocClbk, SDLReallocCallback reallocClbk, SDLFreeCallback freeClbk)
			{
				return new SDLMemoryCallbackAdapterTable(mallocClbk, callocClbk, reallocClbk, freeClbk);
			}

			public static SDLMallocCallback GetMallocDelegate(SDLMallocFunctionPtr mallocFuncPtr)
			{
				if (mallocFuncPtr == null) {
					throw new ArgumentNullException(nameof(mallocFuncPtr));
				}

				return Marshal.GetDelegateForFunctionPointer<SDLMallocCallback>((nint)(vptr)mallocFuncPtr);
			}

			public static SDLCallocCallback GetCallocDelegate(SDLCallocFunctionPtr callocFuncPtr)
			{
				if (callocFuncPtr == null) {
					throw new ArgumentNullException(nameof(callocFuncPtr));
				}

				return Marshal.GetDelegateForFunctionPointer<SDLCallocCallback>((nint)(vptr)callocFuncPtr);
			}

			public static SDLReallocCallback GetReallocDelegate(SDLReallocFunctionPtr reallocFuncPtr)
			{
				if (reallocFuncPtr == null) {
					throw new ArgumentNullException(nameof(reallocFuncPtr));
				}

				return Marshal.GetDelegateForFunctionPointer<SDLReallocCallback>((nint)(vptr)reallocFuncPtr);
			}

			public static SDLFreeCallback GetFreeDelegate(SDLFreeFunctionPtr freeFuncPtr)
			{
				if (freeFuncPtr == null) {
					throw new ArgumentNullException(nameof(freeFuncPtr));
				}

				return Marshal.GetDelegateForFunctionPointer<SDLFreeCallback>((nint)(vptr)freeFuncPtr);
			}
		}
	}
}
