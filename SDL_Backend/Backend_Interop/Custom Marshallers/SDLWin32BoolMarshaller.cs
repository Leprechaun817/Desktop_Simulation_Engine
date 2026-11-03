using System.Runtime.InteropServices.Marshalling;

namespace Backend
{
	namespace CustomMarshallers
	{
		/// <summary>
		/// Marshaller converts managed Boolean values to and from four-byte Win32 BOOL values
		/// </summary>
		[CustomMarshaller(typeof(bool), MarshalMode.Default, typeof(SDLWin32BoolMarshaller))]
		internal static class SDLWin32BoolMarshaller
		{
			public static int ConvertToUnmanaged(bool value)
			{
				if (value) {
					return 1;
				}
				else {
					return 0;
				}
			}

			public static bool ConvertToManaged(int value)
			{
				if (value != 0) {
					return true;
				}
				else {
					return false;
				}
			}
		}
	}
}