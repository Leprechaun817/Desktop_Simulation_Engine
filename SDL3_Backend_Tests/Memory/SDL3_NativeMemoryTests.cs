using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SDL3.Native;
using SDL3.Unsafe;
using Xunit;

namespace SDL3_Backend_Tests
{
	namespace Memory
	{
		[Collection(SDLRuntimeTestCollection.Name)]
		public sealed unsafe class SDLNativeMemoryTests
		{
			[Theory]
			[InlineData("SDL_malloc")]
			[InlineData("SDL_calloc")]
			[InlineData("SDL_realloc")]
			[InlineData("SDL_free")]
			[InlineData("SDL_aligned_alloc")]
			[InlineData("SDL_aligned_free")]
			[InlineData("SDL_GetMemoryFunctions")]
			[InlineData("SDL_GetOriginalMemoryFunctions")]
			[InlineData("SDL_SetMemoryFunctions")]
			[InlineData("SDL_GetNumAllocations")]
			[InlineData("SDL_memcpy")]
			[InlineData("SDL_memmove")]
			[InlineData("SDL_memset")]
			[InlineData("SDL_memset4")]
			[InlineData("SDL_memcmp")]
			public void NativeImports_ShouldUseCdeclAndResolveFromTheConfiguredRuntime(string entryPoint)
			{
				Assert.Equal(Architecture.X64, RuntimeInformation.ProcessArchitecture);

				MethodInfo method = GetImport(entryPoint);
				LibraryImportAttribute import = Assert.IsType<LibraryImportAttribute>(method.GetCustomAttribute<LibraryImportAttribute>());
				Assert.Equal("SDL3.dll", import.LibraryName);
				Assert.Equal(entryPoint, import.EntryPoint);

				UnmanagedCallConvAttribute callConvention = Assert.IsType<UnmanagedCallConvAttribute>(method.GetCustomAttribute<UnmanagedCallConvAttribute>());
				Assert.Equal(new[] { typeof(CallConvCdecl) }, callConvention.CallConvs);

				//Load the DLL copied by the existing project target such that a missing export fails the test.
				nint library = NativeLibrary.Load(Path.Combine(AppContext.BaseDirectory, "SDL3.dll"));
				try {
					Assert.True(NativeLibrary.TryGetExport(library, entryPoint, out nint address), $"SDL3.dll is missing {entryPoint}.");
					Assert.NotEqual(nint.Zero, address);
				}
				finally{
					NativeLibrary.Free(library);
				}
			}

			[Fact]
			public void AllocationImports_ShouldUseSizeTAndVoidPointerABITypes()
			{
				AssertSignature("SDL_malloc", typeof(void*), typeof(nuint));
				AssertSignature("SDL_calloc", typeof(void*), typeof(nuint), typeof(nuint));
				AssertSignature("SDL_realloc", typeof(void*), typeof(void*), typeof(nuint));
				AssertSignature("SDL_aligned_alloc", typeof(void*), typeof(nuint), typeof(nuint));
				AssertSignature("SDL_GetNumAllocations", typeof(int));

				Assert.Equal(typeof(byte), GetImport("SDL_SetMemoryFunctions").ReturnType);
			}

			[Fact]
			public void FreeImportsAndCallback_ShouldReturnVoidAsDeclaredBySDL()
			{
				//SDL_stdinc.h: void SDL_free(void*), void SDL_aligned_free(void*),
				// and typedef void(SDLCALL* SDL_free_func)(void*)
				AssertSignature("SDL_free", typeof(void), typeof(void*));
				AssertSignature("SDL_aligned_free", typeof(void), typeof(void*));

				MethodInfo invoke = typeof(SDLFreeCallback).GetMethod("Invoke")!;
				Assert.Equal(typeof(void), invoke.ReturnType);
				Assert.Equal(typeof(void*), Assert.Single(invoke.GetParameters()).ParameterType);
			}

			[Fact]
			public void MemoryOperationImports_ShouldUseByteLengthsAndDwordValues()
			{
				AssertSignature("SDL_memcpy", typeof(void*), typeof(void*), typeof(void*), typeof(nuint));
				AssertSignature("SDL_memmove", typeof(void*), typeof(void*), typeof(void*), typeof(nuint));
				AssertSignature("SDL_memset", typeof(void*), typeof(void*), typeof(int), typeof(nuint));
				AssertSignature("SDL_memset4", typeof(void*), typeof(void*), typeof(uint), typeof(nuint));
				AssertSignature("SDL_memcmp", typeof(int), typeof(void*), typeof(void*), typeof(nuint));
			}

			private static MethodInfo GetImport(string name)
			{
				return typeof(SDL3_NativeMemory).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic) ?? throw new InvalidOperationException($"Missing native import {name}.");
			}

			private static void AssertSignature(string name, Type retType, params Type[] paramTypes)
			{
				MethodInfo method = GetImport(name);
				Assert.Equal(retType, method.ReturnType);

				ParameterInfo[] parameters = method.GetParameters();
				Assert.Equal(paramTypes.Length, parameters.Length);

				for (int i = 0; i < parameters.Length; i++) {
					Assert.Equal(paramTypes[i], parameters[i].ParameterType);
				}
			}
		}
	}
}