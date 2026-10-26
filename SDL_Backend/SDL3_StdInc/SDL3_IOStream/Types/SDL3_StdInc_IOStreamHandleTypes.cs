using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace SDL3
{
	namespace Unsafe
	{
		public readonly unsafe struct SDLIOStreamHandle : IEquatable<SDLIOStreamHandle>
		{
			private readonly vptr handle;

			public bool IsNull
			{
				get
				{
					if (handle == null) {
						return true;
					}

					return false;
				}
			}

			public nint Address
			{
				get
				{
					return (nint)handle;
				}
			}

			public static SDLIOStreamHandle Null
			{
				get 
				{
					return new SDLIOStreamHandle(null);
				}
			}

			public SDLIOStreamHandle(vptr handle)
			{
				this.handle = handle;
			}

			public bool Equals(SDLIOStreamHandle other)
			{
				if (handle == other.handle) {
					return true;
				}

				return false;
			}

			public override bool Equals([NotNullWhen(true)] object? obj)
			{
				if (obj == null || obj.GetType() != typeof(SDLIOStreamHandle)) {
					return false;
				}

				return Equals((SDLIOStreamHandle)obj);
			}

			public override int GetHashCode()
			{
				return Address.GetHashCode();
			}

			public override string ToString()
			{
				return $"0x{Address:x}";
			}

			public static bool operator ==(SDLIOStreamHandle leftOpr, SDLIOStreamHandle rightOpr)
			{
				return leftOpr.Equals(rightOpr);
			}

			public static bool operator !=(SDLIOStreamHandle leftOpr, SDLIOStreamHandle rightOpr)
			{
				return !leftOpr.Equals(rightOpr);
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public unsafe struct SDLIOStreamInterface
		{
			public uint Version;

			public delegate* unmanaged[Cdecl]<vptr, long> Size;
			public delegate* unmanaged[Cdecl]<vptr, long, SDLIOSeekStartPos, long> Seek;
			public delegate* unmanaged[Cdecl]<vptr, vptr, size_t, SDLIOStatus*, size_t> Read;
			public delegate* unmanaged[Cdecl]<vptr, vptr, size_t, SDLIOStatus*, size_t> Write;
			public delegate* unmanaged[Cdecl]<vptr, SDLIOStatus*, byte> Flush;
			public delegate* unmanaged[Cdecl]<vptr, byte> Close;

			public static SDLIOStreamInterface CreateInitialized()
			{
				return new SDLIOStreamInterface 
				{
					Version = checked((uint)sizeof(SDLIOStreamInterface))
				};
			}
		}
	}
}