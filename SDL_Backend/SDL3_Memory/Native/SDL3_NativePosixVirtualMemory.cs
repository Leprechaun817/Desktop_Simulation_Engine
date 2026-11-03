using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Backend.CustomMarshallers;

namespace SDL3
{
	namespace Native
	{
		/// <summary>
		/// POSIX virtual-memory function imports that are used solely by guarded ABI wrappers.
		/// Wrappers will ensure that these functions don't get called on a Windows host.
		/// Since these are POSIX-based, these will get called on Linux or BSD hosts only.
		/// </summary>
		internal static unsafe partial class SDL3_NativePosixVirtualMemory
		{
			internal const int PROT_NONE = 0x0;
			internal const int PROT_READ = 0x1;
			internal const int PROT_WRITE = 0x2;

			internal const int MAP_PRIVATE = 0x02;
			internal const int MAP_ANONYMOUS_LINUX = 0x20;
			internal const int MAP_ANON_BSD = 0x1000;

			[LibraryImport("libc", EntryPoint = "mmap", SetLastError = true)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr MmapNativeOffset(vptr address, size_t length, int protection, int flags, int fileDescriptor, offset_t offset);

			[LibraryImport("libc", EntryPoint = "mmap", SetLastError = true)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial vptr MmapInt64Offset(vptr address, size_t length, int protection, int flags, int fileDescriptor, offset64_t offset);

			[LibraryImport("libc", EntryPoint = "mprotect", SetLastError = true)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int Mprotect(vptr address, size_t length, int protection);

			[LibraryImport("libc", EntryPoint = "mprotect", SetLastError = true)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int Munmap(vptr address, size_t length);

			internal static bool IsMapFailed(vptr address)
			{
				if ((nint)address == -1) {
					return true;
				}

				return false;
			}
		}
	}
}