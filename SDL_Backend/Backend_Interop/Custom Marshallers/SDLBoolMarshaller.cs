using System.Runtime.InteropServices.Marshalling;

namespace Backend
{
	namespace CustomMarshallers
	{
		///<summary>
		/// Marshaler converts managed Boolean values to and from SDL's one-byte-based C-bool values
		/// </summary>
		[CustomMarshaller(typeof(bool), MarshalMode.Default, typeof(SDLBoolMarshaller))]
		internal static class SDLBoolMarshaller
		{
			public static sbyte ConvertToUnmanaged(bool value)
			{
				if (value) {
					return (sbyte)1;
				}
				else {
					return (sbyte)0;
				}
			}

			public static bool ConvertToManaged(sbyte value)
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