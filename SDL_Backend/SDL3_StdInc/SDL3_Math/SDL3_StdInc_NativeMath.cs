using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SDL3
{
	namespace Native
	{
		internal static partial class SDL3_NativeMath
		{
			[LibraryImport(sdllib, EntryPoint = "SDL_abs")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_abs(int x);

			[LibraryImport(sdllib, EntryPoint = "SDL_acos")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_acos(double x);

			[LibraryImport(sdllib, EntryPoint = "SDL_acosf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_acosf(float x);

			[LibraryImport(sdllib, EntryPoint = "SDL_asin")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_asin(double x);

			[LibraryImport(sdllib, EntryPoint = "SDL_asinf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_asinf(float x);

			[LibraryImport(sdllib, EntryPoint = "SDL_atan")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_atan(double x);

			[LibraryImport(sdllib, EntryPoint = "SDL_atanf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_atanf(float x);

			[LibraryImport(sdllib, EntryPoint = "SDL_atan2")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_atan2(double y, double x);

			[LibraryImport(sdllib, EntryPoint = "SDL_atan2f")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_atan2f(float y, float x);

			[LibraryImport(sdllib, EntryPoint = "SDL_ceil")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_ceil(double x);

			[LibraryImport(sdllib, EntryPoint = "SDL_ceilf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_ceilf(float x);

			[LibraryImport(sdllib, EntryPoint = "SDL_copysign")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_copysign(double x, double y);

			[LibraryImport(sdllib, EntryPoint = "SDL_copysignf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_copysignf(float x, float y);

			[LibraryImport(sdllib, EntryPoint = "SDL_cos")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_cos(double x);

			[LibraryImport(sdllib, EntryPoint = "SDL_cosf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_cosf(float x);

			[LibraryImport(sdllib, EntryPoint = "SDL_exp")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_exp(double x);

			[LibraryImport(sdllib, EntryPoint = "SDL_expf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_expf(float x);

			[LibraryImport(sdllib, EntryPoint = "SDL_fabs")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_fabs(double x);

			[LibraryImport(sdllib, EntryPoint = "SDL_fabsf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_fabsf(float x);

			[LibraryImport(sdllib, EntryPoint = "SDL_floor")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_floor(double x);

			[LibraryImport(sdllib, EntryPoint = "SDL_floorf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_floorf(float x);

			[LibraryImport(sdllib, EntryPoint = "SDL_trunc")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_trunc(double x);

			[LibraryImport(sdllib, EntryPoint = "SDL_truncf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_truncf(float x);

			[LibraryImport(sdllib, EntryPoint = "SDL_fmod")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_fmod(double x, double y);

			[LibraryImport(sdllib, EntryPoint = "SDL_fmodf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_fmodf(float x, float y);

			[LibraryImport(sdllib, EntryPoint = "SDL_isinf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_isinf(double x);

			[LibraryImport(sdllib, EntryPoint = "SDL_isinff")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_isinff(float x);

			[LibraryImport(sdllib, EntryPoint = "SDL_isnan")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_isnan(double x);

			[LibraryImport(sdllib, EntryPoint = "SDL_isnanf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial int SDL_isnanf(float x);

			[LibraryImport(sdllib, EntryPoint = "SDL_log")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_log(double x);

			[LibraryImport(sdllib, EntryPoint = "SDL_logf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_logf(float x);

			[LibraryImport(sdllib, EntryPoint = "SDL_log10")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_log10(double x);

			[LibraryImport(sdllib, EntryPoint = "SDL_log10f")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_log10f(float x);

			[LibraryImport(sdllib, EntryPoint = "SDL_modf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_modf(double x, out double y);

			[LibraryImport(sdllib, EntryPoint = "SDL_modff")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_modff(float x, out float y);

			[LibraryImport(sdllib, EntryPoint = "SDL_pow")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_pow(double x, double y);

			[LibraryImport(sdllib, EntryPoint = "SDL_powf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_powf(float x, float y);

			[LibraryImport(sdllib, EntryPoint = "SDL_round")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_round(double x);

			[LibraryImport(sdllib, EntryPoint = "SDL_roundf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_roundf(float x);

			[LibraryImport(sdllib, EntryPoint = "SDL_lround")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial CLong SDL_lround(double x);

			[LibraryImport(sdllib, EntryPoint = "SDL_lroundf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial CLong SDL_lroundf(float x);

			[LibraryImport(sdllib, EntryPoint = "SDL_scalbn")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_scalbn(double x, int n);

			[LibraryImport(sdllib, EntryPoint = "SDL_scalbnf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_scalbnf(float x, int n);

			[LibraryImport(sdllib, EntryPoint = "SDL_sin")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_sin(double x);

			[LibraryImport(sdllib, EntryPoint = "SDL_sinf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_sinf(float x);

			[LibraryImport(sdllib, EntryPoint = "SDL_sqrt")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_sqrt(double x);

			[LibraryImport(sdllib, EntryPoint = "SDL_sqrtf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_sqrtf(float x);

			[LibraryImport(sdllib, EntryPoint = "SDL_tan")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial double SDL_tan(double x);

			[LibraryImport(sdllib, EntryPoint = "SDL_tanf")]
			[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
			internal static partial float SDL_tanf(float x);
		}
	}
}