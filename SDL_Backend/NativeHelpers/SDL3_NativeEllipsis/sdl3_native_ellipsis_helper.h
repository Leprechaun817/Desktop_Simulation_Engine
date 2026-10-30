#ifndef SDL3_NATIVE_ELLIPSIS_HELPER_H
#define SDL3_NATIVE_ELLIPSIS_HELPER_H

#include <stddef.h>
#include <stdint.h>

#if defined(_WIN32)
#define SDL3_NATIVE_ELLIPSIS_EXPORT __declspec(dllexport)
#define SDL3_NATIVE_ELLIPSIS_CDECL __cdecl
#else
#define SDL3_NATIVE_ELLIPSIS_EXPORT __attribute__((visibility("default")))
#define SDL3_NATIVE_ELLIPSIS_CDECL
#endif

#ifdef __cplusplus
extern "C" {
#endif

	enum sdl_native_allipsis_printf_slot_kind 
	{
		SDL_NATIVE_ELLIPSIS_PRINTF_SIGNED_INTEGER = 1,
		SDL_NATIVE_ELLIPSIS_PRINTF_UNSIGNED_INTEGER = 2,
		SDL_NATIVE_ELLIPSIS_PRINTF_FLOATING_POINT = 3,
		SDL_NATIVE_ELLIPSIS_PRINTF_UTF8_STRING = 4,
		SDL_NATIVE_ELLIPSIS_PRINTF_WIDE_STRING = 5,
		SDL_NATIVE_ELLIPSIS_PRINTF_POINTER = 6
	}

	typedef struct sdl_native_ellipsis_printf_slot 
	{
		int kind;
		int native_byte_count;
		int64_t signed_value;
		uint64_t unsigned_value;
		double floating_value;
		uintptr_t pointer_value;
	} sdl_native_ellipsis_printf_slot;

	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_set_sdl_path(const char* path);
	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_log_message(int category, int priority, const char* fmt, const sdl_native_ellipsis_printf_slot* slots,
																							   size_t slot_count);

	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_get_last_error(char* buffer, size_t buffer_size);
	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_set_error(const char* fmt, const sdl_native_ellipsis_printf_slot* slots, size_t slot_count);

	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_render_debug_text_format(void* renderer, float x, float y, const char* fmt, 
																											const sdl_native_ellipsis_printf_slot* slots, size_t slot_count);

	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_snprintf(char* text, size_t maxlen, const char* fmt, const sdl_native_ellipsis_printf_slot* slots,
																						    size_t slot_count);
	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_asprintf(char** text, const char* fmt, const sdl_native_ellipsis_printf_slot* slots, size_t slot_count);
	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_swprintf(void* text, size_t maxlen, const void* fmt, const sdl_native_ellipsis_prinf_slot* slots, 
																							size_t slot_count, int wchar_size);
	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_ioprintf(void* context, const char* fmt, const sdl_native_ellipsis_printf_slot* slots, size_t slot_count);

	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_sscanf_0(const char* text, const char* fmt);
	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_sscanf_1(const char* text, const char* fmt, void* a0);
	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_sscanf_2(const char* text, const char* fmt, void* a0, void* a1);
	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_sscanf_3(const char* text, const char* fmt, void* a0, void* a1, void* a2);
	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_sscanf_4(const char* text, const char* fmt, void* a0, void* a1, void* a2, void* a3);
	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_sscanf_5(const char* text, const char* fmt, void* a0, void* a1, void* a2, void* a3, void* a4);
	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_sscanf_6(const char* text, const char* fmt, void* a0, void* a1, void* a2, void* a3, void* a4, void* a5);
	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_sscanf_7(const char* text, const char* fmt, void* a0, void* a1, void* a2, void* a3, void* a4, void* a5,
																							void* a6);
	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_sscanf_8(const char* text, const char* fmt, void* a0, void* a1, void* a2, void* a3, void* a4, void* a5,
																							void* a6, void* a7);
	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_sscanf_9(const char* text, const char* fmt, void* a0, void* a1, void* a2, void* a3, void* a4, void* a5,
																							void* a6, void* a7, void* a8);
	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_sscanf_10(const char* text, const char* fmt, void* a0, void* a1, void* a2, void* a3, void* a4, void* a5,
																							 void* a6, void* a7, void* a8, void* a9);
	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_sscanf_11(const char* text, const char* fmt, void* a0, void* a1, void* a2, void* a3, void* a4, void* a5,
																							 void* a6, void* a7, void* a8, void* a9, void* a10);
	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_sscanf_12(const char* text, const char* fmt, void* a0, void* a1, void* a2, void* a3, void* a4, void* a5,
																							 void* a6, void* a7, void* a8, void* a9, void* a10, void* a11);
	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_sscanf_13(const char* text, const char* fmt, void* a0, void* a1, void* a2, void* a3, void* a4, void* a5,
																							 void* a6, void* a7, void* a8, void* a9, void* a10, void* a11, void* a12);
	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_sscanf_14(const char* text, const char* fmt, void* a0, void* a1, void* a2, void* a3, void* a4, void* a5,
																							 void* a6, void* a7, void* a8, void* a9, void* a10, void* a11, void* a12, void* a13);
	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_sscanf_15(const char* text, const char* fmt, void* a0, void* a1, void* a2, void* a3, void* a4, void* a5,
																							 void* a6, void* a7, void* a8, void* a9, void* a10, void* a11, void* a12, void* a13, void* a14);
	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_sscanf_16(const char* text, const char* fmt, void* a0, void* a1, void* a2, void* a3, void* a4, void* a5,
																							 void* a6, void* a7, void* a8, void* a9, void* a10, void* a11, void* a12, void* a13, void* a14,
																							 void* a15);
#ifdef __cplusplus	
}
#endif

#endif