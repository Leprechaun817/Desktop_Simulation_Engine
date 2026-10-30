using System.Runtime.InteropServices;
using SDL3.Native;

namespace SDL3
{
	namespace Unsafe
	{
		/// <summary>
		/// Intermediate, unsafe narrow/UTF-8/Wide String function helpers
		/// </summary>
		public static unsafe class SDL3_StringUnsafe
		{
			public static SDLSize Strlen(bptr str)
			{
				return new SDLSize(SDL3_NativeString.SDL_strlen(str));
			}

			public static SDLSize Strnlen(bptr str, SDLSize maxlen)
			{
				return new SDLSize(SDL3_NativeString.SDL_strnlen(str, maxlen.Value));
			}

			public static SDLSize Strlcpy(bptr dest, bptr src, SDLSize maxlen)
			{
				return new SDLSize(SDL3_NativeString.SDL_strlcpy(dest, src, maxlen.Value));
			}

			public static SDLSize UTF8Strlcpy(bptr dest, bptr src, SDLSize destBytes)
			{
				return new SDLSize(SDL3_NativeString.SDL_utf8strlcpy(dest, src, destBytes.Value));
			}

			public static SDLSize Strlcat(bptr dest, bptr src, SDLSize maxlen)
			{
				return new SDLSize(SDL3_NativeString.SDL_strlcat(dest, src, maxlen.Value));
			}

			public static bptr Strdup(bptr str)
			{
				return SDL3_NativeString.SDL_strdup(str);
			}

			public static bptr Strndup(bptr str, SDLSize maxlen)
			{
				return SDL3_NativeString.SDL_strndup(str, maxlen.Value);
			}

			public static bptr Strrev(bptr str)
			{
				return SDL3_NativeString.SDL_strrev(str);
			}

			public static bptr Strupr(bptr str)
			{
				return SDL3_NativeString.SDL_strupr(str);
			}

			public static bptr Strlwr(bptr str)
			{
				return SDL3_NativeString.SDL_strlwr(str);
			}

			public static bptr Strchr(bptr str, int c)
			{
				return SDL3_NativeString.SDL_strchr(str, c);
			}

			public static bptr Strrchr(bptr str, int c)
			{
				return SDL3_NativeString.SDL_strrchr(str, c);
			}

			public static bptr Strstr(bptr haystack, bptr needle)
			{
				return SDL3_NativeString.SDL_strstr(haystack, needle);
			}

			public static bptr Strnstr(bptr haystack, bptr needle, SDLSize maxlen)
			{
				return SDL3_NativeString.SDL_strnstr(haystack, needle, maxlen.Value);
			}

			public static bptr Strcasestr(bptr haystack, bptr needle)
			{
				return SDL3_NativeString.SDL_strcasestr(haystack, needle);
			}

			public static bptr StrtokR(bptr str, bptr delim, bdptr saveptr)
			{
				return SDL3_NativeString.SDL_strtok_r(str, delim, saveptr);
			}

			public static bptr Itoa(int value, bptr str, int radix)
			{
				return SDL3_NativeString.SDL_itoa(value, str, radix);
			}

			public static bptr Uitoa(uint value, bptr str, int radix)
			{
				return SDL3_NativeString.SDL_uitoa(value, str, radix);
			}

			public static bptr Ltoa(CLong value, bptr str, int radix)
			{
				return SDL3_NativeString.SDL_ltoa(value, str, radix);
			}

			public static bptr Ultoa(CULong value, bptr str, int radix)
			{
				return SDL3_NativeString.SDL_ultoa(value, str, radix);
			}

			public static bptr Lltoa(long value, bptr str, int radix)
			{
				return SDL3_NativeString.SDL_lltoa(value, str, radix);
			}

			public static bptr Ulltoa(ulong value, bptr str, int radix)
			{
				return SDL3_NativeString.SDL_ulltoa(value, str, radix);
			}

			public static int Atoi(bptr str)
			{
				return SDL3_NativeString.SDL_atoi(str);
			}

			public static double Atof(bptr str)
			{
				return SDL3_NativeString.SDL_atof(str);
			}

			public static CLong Strtol(bptr str, out ConstVoidPtr end, int baseRadix)
			{
				bptr endPtr;
				CLong result = SDL3_NativeString.SDL_strtol(str, &endPtr, baseRadix);
				end = new ConstVoidPtr(endPtr);

				return result;
			}

			public static CLong Strtol(bptr str, int baseRadix)
			{
				return SDL3_NativeString.SDL_strtol(str, null, baseRadix);
			}

			public static CULong Strtoul(bptr str, out ConstVoidPtr end, int baseRadix)
			{
				bptr endPtr;
				CULong result = SDL3_NativeString.SDL_strtoul(str, &endPtr, baseRadix);
				end = new ConstVoidPtr(endPtr);

				return result;
			}

			public static CULong Strtoul(bptr str, int baseRadix)
			{
				return SDL3_NativeString.SDL_strtoul(str, null, baseRadix);
			}

			public static long Strtoll(bptr str, out ConstVoidPtr end, int baseRadix)
			{
				bptr endPtr;
				long result = SDL3_NativeString.SDL_strtoll(str, &endPtr, baseRadix);
				end = new ConstVoidPtr(endPtr);

				return result;
			}

			public static long Strtoll(bptr str, int baseRadix)
			{
				return SDL3_NativeString.SDL_strtoll(str, null, baseRadix);
			}

			public static ulong Strtoull(bptr str, out ConstVoidPtr end, int baseRadix)
			{
				bptr endPtr;
				ulong result = SDL3_NativeString.SDL_strtoull(str, &endPtr, baseRadix);
				end = new ConstVoidPtr(endPtr);

				return result;
			}

			public static ulong Strtoull(bptr str, int baseRadix)
			{
				return SDL3_NativeString.SDL_strtoull(str, null, baseRadix);
			}

			public static double Strtod(bptr str, out ConstVoidPtr end)
			{
				bptr endPtr;
				double result = SDL3_NativeString.SDL_strtod(str, &endPtr);
				end = new ConstVoidPtr(endPtr);

				return result;
			}

			public static double Strtod(bptr str)
			{
				return SDL3_NativeString.SDL_strtod(str, null);
			}

			public static SDLSize UTF8Strlen(bptr str)
			{
				return new SDLSize(SDL3_NativeString.SDL_utf8strlen(str));
			}

			public static SDLSize UTF8Strnlen(bptr str, SDLSize bytes)
			{
				return new SDLSize(SDL3_NativeString.SDL_utf8strnlen(str, bytes.Value));
			}

			public static uint StepUTF8(bdptr text, size_t* byteLen)
			{
				return SDL3_NativeString.SDL_StepUTF8(text, byteLen);
			}

			public static uint StepUTF8(bdptr text)
			{
				return SDL3_NativeString.SDL_StepUTF8(text, null);
			}

			public static uint StepBackUTF8(bptr start, bdptr text)
			{
				return SDL3_NativeString.SDL_StepBackUTF8(start, text);
			}

			public static bptr UCS4ToUTF8(uint codepoint, bptr dest)
			{
				return SDL3_NativeString.SDL_UCS4ToUTF8(codepoint, dest);
			}

			public static int Strcmp(bptr str1, bptr str2)
			{
				return SDL3_NativeString.SDL_strcmp(str1, str2);
			}

			public static int Strncmp(bptr str1, bptr str2, SDLSize maxlen)
			{
				return SDL3_NativeString.SDL_strncmp(str1, str2, maxlen.Value);
			}

			public static int Strcasecmp(bptr str1, bptr str2)
			{
				return SDL3_NativeString.SDL_strcasecmp(str1, str2);
			}

			public static int Strncasecmp(bptr str1, bptr str2, SDLSize maxlen)
			{
				return SDL3_NativeString.SDL_strncasecmp(str1, str2, maxlen.Value);
			}

			public static bptr Strpbrk(bptr str, bptr breakset)
			{
				return SDL3_NativeString.SDL_strpbrk(str, breakset);
			}

			public static SDLSize Wcslen(ConstVoidPtr str)
			{
				return new SDLSize(SDL3_NativeString.SDL_wcslen((vptr)str));
			}

			public static SDLSize Wcsnlen(ConstVoidPtr str, SDLSize maxlen)
			{
				return new SDLSize(SDL3_NativeString.SDL_wcsnlen((vptr)str, maxlen.Value));
			}

			public static SDLSize Wcslcpy(VoidPtr dest, ConstVoidPtr src, SDLSize maxlen)
			{
				return new SDLSize(SDL3_NativeString.SDL_wcslcpy((vptr)dest, (vptr)src, maxlen.Value));
			}

			public static SDLSize Wcslcat(VoidPtr dest, ConstVoidPtr src, SDLSize maxlen)
			{
				return new SDLSize(SDL3_NativeString.SDL_wcslcat((vptr)dest, (vptr)src, maxlen.Value));
			}

			public static VoidPtr Wcsdup(ConstVoidPtr str)
			{
				return new VoidPtr(SDL3_NativeString.SDL_wcsdup((vptr)str));
			}

			public static ConstVoidPtr Wcsstr(ConstVoidPtr haystack, ConstVoidPtr needle)
			{
				return new ConstVoidPtr(SDL3_NativeString.SDL_wcsstr((vptr)haystack, (vptr)needle));
			}

			public static ConstVoidPtr Wcsnstr(ConstVoidPtr haystack, ConstVoidPtr needle, SDLSize maxlen)
			{
				return new ConstVoidPtr(SDL3_NativeString.SDL_wcsnstr((vptr)haystack, (vptr)needle, maxlen.Value));
			}

			public static int Wcscmp(ConstVoidPtr str1, ConstVoidPtr str2)
			{
				return SDL3_NativeString.SDL_wcscmp((vptr)str1, (vptr)str2);
			}

			public static int Wcsncmp(ConstVoidPtr str1, ConstVoidPtr str2, SDLSize maxlen)
			{
				return SDL3_NativeString.SDL_wcsncmp((vptr)str1, (vptr)str2, maxlen.Value);
			}

			public static int Wcscasecmp(ConstVoidPtr str1, ConstVoidPtr str2)
			{
				return SDL3_NativeString.SDL_wcscasecmp((vptr)str1, (vptr)str2);
			}

			public static int Wcsncasecmp(ConstVoidPtr str1, ConstVoidPtr str2, SDLSize maxlen)
			{
				return SDL3_NativeString.SDL_wcsncasecmp((vptr)str1, (vptr)str2, maxlen.Value);
			}

			public static CLong Wcstol(ConstVoidPtr str, out ConstVoidPtr end, int baseRadix)
			{
				vptr endPtr;
				CLong result = SDL3_NativeString.SDL_wcstol((vptr)str, &endPtr, baseRadix);
				end = new ConstVoidPtr(endPtr);

				return result;
			}

			public static CLong Wcstol(ConstVoidPtr str, int baseRadix)
			{
				return SDL3_NativeString.SDL_wcstol((vptr)str, null, baseRadix);
			}

			public static CULong Wcstoul(ConstVoidPtr str, out ConstVoidPtr end, int baseRadix)
			{
				vptr endPtr;
				CULong result = SDL3_NativeString.SDL_wcstoul((vptr)str, &endPtr, baseRadix);
				end = new ConstVoidPtr(endPtr);

				return result;
			}

			public static CULong Wcstoul(ConstVoidPtr str, int baseRadix)
			{
				return SDL3_NativeString.SDL_wcstoul((vptr)str, null, baseRadix);
			}

			public static long Wcstoll(ConstVoidPtr str, out ConstVoidPtr end, int baseRadix)
			{
				vptr endPtr;
				long result = SDL3_NativeString.SDL_wcstoll((vptr)str, &endPtr, baseRadix);
				end = new ConstVoidPtr(endPtr);

				return result;
			}

			public static long WcsToll(ConstVoidPtr str, int baseRadix)
			{
				return SDL3_NativeString.SDL_wcstoll((vptr)str, null, baseRadix);
			}

			public static ulong Wcstoull(ConstVoidPtr str, out ConstVoidPtr end, int baseRadix)
			{
				vptr endPtr;
				ulong result = SDL3_NativeString.SDL_wcstoull((vptr)str, &endPtr, baseRadix);
				end = new ConstVoidPtr(endPtr);

				return result;
			}

			public static ulong Wcstoull(ConstVoidPtr str, int baseRadix)
			{
				return SDL3_NativeString.SDL_wcstoull((vptr)str, null, baseRadix);
			}
		}
	}
}