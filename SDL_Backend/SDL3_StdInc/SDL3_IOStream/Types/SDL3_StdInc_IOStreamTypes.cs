using SDL3.Unsafe;
using System;
using System.Runtime.InteropServices;

namespace SDL3
{
	public enum SDLIOStatus
	{
		Ready = 0,
		Error = 1,
		EOF = 2,
		NotReady = 3,
		ReadOnly = 4,
		WriteOnly = 5
	}

	public enum SDLIOSeekStartPos
	{
		Set = 0,
		Current = 1,
		End = 2
	}

	public static class SDLIOStreamPropertyNames
	{
		public const string WindowsHandlePointer = "SDL.iostream.windows.handle";
		public const string StdioFilePointer = "SDL.iostream.stdio.file";
		public const string FileDescriptorNumber = "SDL.iostream.file_descriptor";
		public const string AndroidAssetPointer = "SDL.iostream.android.asset";
		public const string MemoryPointer = "SDL.iostream.memory.base";
		public const string MemorySizeNumber = "SDL.iostream.memory.size";
		public const string MemoryFreeFunctionPointer = "SDL.iostream.memory.free";
		public const string DynamicMemoryPointer = "SDL.iostream.dynamic.memory";
		public const string DynamicChunkSizeNumber = "SDL.iostream.dynamic.chunksize";
	}

	public delegate long SDLIOStreamSizeCallback(object? userdata);
	public delegate long SDLIOStreamSeekCallback(object? userdata, long offset, SDLIOSeekStartPos seekPos);
	public delegate SDLSize SDLIOStreamReadCallback(object? userdata, Span<byte> dest, out SDLIOStatus status);
	public delegate SDLSize SDLIOStreamWriteCallback(object? userdata, Span<byte> src, out SDLIOStatus status);
	public delegate bool SDLIOStreamFlushCallback(object? userdata, out SDLIOStatus status);
	public delegate bool SDLIOStreamCloseCallback(object? userdata);

	public sealed class SDLIOStreamCallbacks
	{
		private SDLIOStreamSizeCallback? size;
		private SDLIOStreamSeekCallback? seek;
		private SDLIOStreamReadCallback? read;
		private SDLIOStreamWriteCallback? write;
		private SDLIOStreamFlushCallback? flush;
		private SDLIOStreamCloseCallback? close;

		public SDLIOStreamSizeCallback? Size
		{
			get
			{
				return size;
			}

			init
			{
				size = value;
			}
		}

		public SDLIOStreamSeekCallback? Seek
		{
			get
			{
				return seek;
			}

			init
			{
				seek = value;
			}
		}

		public SDLIOStreamReadCallback? Read
		{
			get
			{
				return read;
			}

			init
			{
				read = value;
			}
		}

		public SDLIOStreamWriteCallback? Write
		{
			get
			{
				return write;
			}

			init
			{
				write = value;
			}
		}

		public SDLIOStreamFlushCallback? Flush
		{
			get
			{
				return flush;
			}

			init
			{
				flush = value;
			}
		}

		public SDLIOStreamCloseCallback? Close
		{
			get
			{
				return close;
			}

			init
			{
				close = value;
			}
		}
	}
}