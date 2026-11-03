using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;

namespace SDL3
{
	namespace Native
	{
		/// <summary>
		/// Windows-specific virtual-memory function imports that are used solely by
		/// guarded ABI wrappers. Wrappers will ensure that they don't get called on
		/// a POSIX-based host. The specific Windows-hosts supported will likely depend
		/// at least in part on what the running dotnet version supports.
		/// </summary>
		internal static unsafe partial class SDL3_NativeWindowsVirtualMemory
		{
			internal const uint MEM_COMMIT = 0x1000;
			internal const uint MEM_RESERVE = 0x2000;
			internal const uint MEM_RELEASE = 0x8000;

			internal const uint PAGE_NOACCESS = 0x01;
			internal const uint PAGE_READWRITE = 0x04;

			[LibraryImport("kernel32.dll", EntryPoint = "VirtualAlloc", SetLastError = true)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
			internal static partial vptr VirtualAlloc(vptr lpAddress, size_t dwSize, uint flAllocationType, uint flProtect);

			[LibraryImport("kernel32.dll", EntryPoint = "VirtualAlloc", SetLastError = true)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static partial bool VirtualProtect(vptr lpAddress, size_t dwSize, uint flNewProtect, out uint lpflOldProtect);

			[LibraryImport("kernel32.dll", EntryPoint = "VirtualFree", SetLastError = true)]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
			[return: MarshalAs(UnmanagedType.Bool)]
		}
	}
}