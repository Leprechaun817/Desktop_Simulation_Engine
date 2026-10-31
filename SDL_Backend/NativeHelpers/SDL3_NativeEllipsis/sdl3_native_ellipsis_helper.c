#include "sdl3_native_ellipsis_helper.h"

#include <stdbool.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <wchar.h>

#if defined(_WIN32)
#include <windows.h>
#else
#include <dlfcn.h>
#endif

#define SDL_NATIVE_ELLIPSIS_ERROR (-32000)
#define SDL_NATIVE_ELLIPSIS_SIZE_ERROR ((size_t)-1)
#define SDL_NATIVE_ELLIPSIS_MESSAGE_BYTES 256u
#define SDL_NATIVE_ELLIPSIS_MAX_PRINTF_SLOTS 16u

typedef bool (SDL3_NATIVE_ELLIPSIS_CDECL *sdl_native_ellipsis_set_error_fn)(const char* fmt, ...);
typedef void (SDL3_NATIVE_ELLIPSIS_CDECL *sdl_native_ellipsis_log_message_fn)(int category, int priority, const char* fmt, ...);
typedef bool (SDL3_NATIVE_ELLIPSIS_CDECL* sdl_native_ellipsis_render_debug_text_format_fn)(void* renderer, float x, float y, const char* fmt, ...);
typedef int (SDL3_NATIVE_ELLIPSIS_CDECL *sdl_native_ellipsis_sscanf_fn)(const char* text, const char* fmt, ...);
typedef int (SDL3_NATIVE_ELLIPSIS_CDECL *sdl_native_ellipsis_snprintf_fn)(char* text, size_t maxlen, const char* fmt, ...);
typedef int (SDL3_NATIVE_ELLIPSIS_CDECL *sdl_native_ellipsis_asprintf_fn)(char** text, const char* fmt, ...);
typedef int (SDL3_NATIVE_ELLIPSIS_CDECL *sdl_native_ellipsis_swprintf_fn)(wchar_t* text, size_t maxlen, const wchar_t* fmt, ...);
typedef size_t (SDL3_NATIVE_ELLIPSIS_CDECL *sdl_native_ellipsis_ioprintf_fn)(void* context, const char* fmt, ...);

static sdl_native_ellipsis_set_error_fn g_sdl_set_error;
static sdl_native_ellipsis_log_message_fn g_sdl_log_message;
static sdl_native_ellipsis_render_debug_text_format_fn g_sdl_render_debug_text_format;
static sdl_native_ellipsis_sscanf_fn g_sdl_sscanf;
static sdl_native_ellipsis_snprintf_fn g_sdl_snprintf;
static sdl_native_ellipsis_asprintf_fn g_sdl_asprintf;
static sdl_native_ellipsis_swprintf_fn g_sdl_swprintf;
static sdl_native_ellipsis_ioprintf_fn g_sdl_ioprintf;
static char g_last_error[SDL3_NATIVE_ELLIPSIS_MESSAGE_BYTES];

#if defined(_WIN32)
static module g_sdl_module;
#else
static void* g_sdl_module;
#endif

static void sdl_native_ellipsis_set_last_error(const char* message)
{
	size_t length;
	size_t copy_length;

	if (message == NULL) {
		message = "unknown error";
	}

	length = strlen(message);
	if (length < SDL3_NATIVE_ELLIPSIS_MESSAGE_BYTES - 1u) {
		copy_length = length;
	}
	else {
		copy_length = SDL3_NATIVE_ELLIPSIS_MESSAGE_BYTES - 1u;
	}

	memcpy(g_last_error, message, copy_length);
	g_last_error[copy_length] = 0;
}

static void sdl_native_ellipsis_set_error_code(const char* prefix, unsigned long code) 
{
#if defined(_MSC_VER)
	sprintf_s(g_last_error, sizeof(g_last_error), "%s: %lu", prefix, code);
#else
	snprintf(g_last_error, sizeof(g_last_error), "%s: %lu", prefix, code);
#endif
}

static void sdl_native_ellipsis_reset_exports(void)
{
	g_sdl_sscanf = NULL;
	g_sdl_snprintf = NULL;
	g_sdl_asprintf = NULL;
	g_sdl_swprintf = NULL;
	g_sdl_set_error = NULL;
	g_sdl_log_message = NULL;
	g_sdl_ioprintf = NULL;
	g_sdl_render_debug_text_format = NULL;
}

static int sdl_native_ellipsis_ensure_module_loaded(void)
{
	if (g_sdl_render_debug_text_format != NULL) {
		return 0;
	}

#if defined(_WIN32)
	g_sdl_module = LoadLibraryA("SDL3.dll");
	if (g_sdl_module == NULL) {
		sdl_native_ellipsis_set_error_code("LoadLibraryA(SDL3.dll) failed", GetLastError());
		return -1;
	}
#else
	g_sdl_module = dlopen("libSDL3.so", RTLD_NOW | RTLD_LOCAL);
	if (g_sdl_module == NULL) {
		sdl_native_ellipsis_set_last_error(dlerror());
		return -1;
	}
#endif

	return 0;
}

static void* sdl_native_ellipsis_resolve_symbol(const char* name)
{	
	void* symbol;

	if (sdl_native_ellipsis_ensure_module_loaded() != 0) {
		return NULL;
	}

#if defined(_WIN32)
	symbol = (void*)GetProcAddress(g_sdl_module, name);
#else
	symbol = dlsym(g_sdl_module, name);
#endif

	if (symbol == NULL) {
		sdl_native_ellipsis_set_last_error(name);
	}

	return symbol;
}

static sdl_native_ellipsis_sscanf_fn sdl_native_ellipsis_get_sscanf(void)
{
	if (g_sdl_sscanf == NULL) {
		g_sdl_sscanf = (sdl_native_ellipsis_sscanf_fn)sdl_native_ellipsis_resolve_symbol("SDL_sscanf");
	}

	return g_sdl_sscanf;
}

static sdl_native_ellipsis_snprintf_fn sdl_native_ellipsis_get_snprintf(void)
{
	if (g_sdl_snprintf == NULL) {
		g_sdl_snprintf = (sdl_native_ellipsis_snprintf_fn)sdl_native_ellipsis_resolve_symbol("SDL_snprintf");
	}

	return g_sdl_snprintf;
}

static sdl_native_ellipsis_asprintf_fn sdl_native_ellipsis_get_asprintf(void)
{
	if (g_sdl_asprintf == NULL) {
		g_sdl_asprintf = (sdl_native_ellipsis_asprintf_fn)sdl_native_ellipsis_resolve_symbol("SDL_asprintf");
	}

	return g_sdl_asprintf;
}

static sdl_native_ellipsis_swprintf_fn sdl_native_ellipsis_get_swprintf(void)
{
	if (g_sdl_swprintf == NULL) {
		g_sdl_swprintf = (sdl_native_ellipsis_swprintf_fn)sdl_native_ellipsis_resolve_symbol("SDL_swprintf");
	}

	return g_sdl_swprintf;
}

static sdl_native_ellipsis_set_error_fn sdl_native_ellipsis_get_set_error(void)
{
	if (g_sdl_set_error == NULL) {
		g_sdl_set_error = (sdl_native_ellipsis_set_error_fn)sdl_native_ellipsis_resolve_symbol("SDL_SetError");
	}

	return g_sdl_set_error;
}

static sdl_native_ellipsis_log_message_fn sdl_native_ellipsis_get_log_message(void)
{
	if (g_sdl_log_message == NULL) {
		g_sdl_log_message = (sdl_native_ellipsis_log_message_fn)sdl_native_ellipsis_resolve_symbol("SDL_LogMessage");
	}

	return g_sdl_log_message;
}

static sdl_native_ellipsis_ioprintf_fn sdl_native_ellipsis_get_ioprintf(void)
{
	if (g_sdl_ioprintf == NULL) {
		g_sdl_ioprintf = (sdl_native_ellipsis_ioprintf_fn)sdl_native_ellipsis_resolve_symbol("SDL_IOprintf");
	}

	return g_sdl_ioprintf;
}

static sdl_native_ellipsis_render_debug_text_format_fn sdl_native_ellipsis_get_render_debug_text_format(void)
{
	if (g_sdl_render_debug_text_format == NULL) {
		g_sdl_render_debug_text_format = (sdl_native_ellipsis_render_debug_text_format_fn)sdl_native_ellipsis_resolve_symbol("SDL_RenderDebugTextFormat");
	}

	return g_sdl_render_debug_text_format;
}

static uint64_t sdl_native_ellipsis_slot_raw64(const sdl_native_ellipsis_printf_slot* slot)
{
	uint64_t value;
	switch (slot->kind) {
		case SDL_NATIVE_ELLIPSIS_PRINTF_SIGNED_INTEGER:
			return (uint64_t)slot->signed_value;
		case SDL_NATIVE_ELLIPSIS_PRINTF_UNSIGNED_INTEGER:
			return slot->unsigned_value;
		case SDL_NATIVE_ELLIPSIS_PRINTF_FLOATING_POINT:
			memcpy(&value, &slot->floating_value, sizeof(value));
			return value;
		case SDL_NATIVE_ELLIPSIS_PRINTF_UTF8_STRING:
		case SDL_NATIVE_ELLIPSIS_PRINTF_WIDE_STRING:
		case SDL_NATIVE_ELLIPSIS_PRINTF_POINTER:
			return (uint64_t)slot->pointer_value;
		default:
			return 0u;
	}
}

static int sdl_native_ellipsis_validate_printf_slots(const sdl_native_ellipsis_printf_slot* slots, size_t slot_count)
{ 
	if (slot_count > SDL_NATIVE_ELLIPSIS_MAX_PRINTF_SLOTS) {
		sdl_native_ellipsis_set_last_error("too many printf slots");
		return -1;
	}

	if (slot_count != 0u && slots == NULL) {
		sdl_native_ellipsis_set_last_error("printf slots pointer is null");
		return -2;
	}

	for (size_t i = 0u; i < slot_count; i++) {
		switch (slots[i].kind) {
			case SDL_NATIVE_ELLIPSIS_PRINTF_SIGNED_INTEGER:
			case SDL_NATIVE_ELLIPSIS_PRINTF_UNSIGNED_INTEGER:
			case SDL_NATIVE_ELLIPSIS_PRINTF_UTF8_STRING:
			case SDL_NATIVE_ELLIPSIS_PRINTF_WIDE_STRING:
			case SDL_NATIVE_ELLIPSIS_PRINTF_POINTER:
				if (slots[i].native_byte_count != 4 && slots[i].native_byte_count != 8) {
					sdl_native_ellipsis_set_last_error("unsupported printf slot byte count");
					return -3;
				}
				break;
			case SDL_NATIVE_ELLIPSIS_PRINTF_FLOATING_POINT:
				if (slots[i].native_byte_count != 8) {
					sdl_native_ellipsis_set_last_error("floating point slots must be 8 bytes");
					return -4;
				}
				break;
			default:
				sdl_native_ellipsis_set_last_error("unknown printf slot kind");
				return -5;
		}
	}

	return 0;
}

#define SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_1 sdl_native_ellipsis_slot_raw64(&slots[0])
#define SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_2, SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_1, sdl_native_ellipsis_slot_raw64(&slots[1])
#define SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_3, SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_2, sdl_native_ellipsis_slot_raw64(&slots[2])
#define SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_4, SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_3, sdl_native_ellipsis_slot_raw64(&slots[3])
#define SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_5, SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_4, sdl_native_ellipsis_slot_raw64(&slots[4])
#define SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_6, SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_5, sdl_native_ellipsis_slot_raw64(&slots[5])
#define SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_7, SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_6, sdl_native_ellipsis_slot_raw64(&slots[6])
#define SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_8, SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_7, sdl_native_ellipsis_slot_raw64(&slots[7])
#define SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_9, SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_8, sdl_native_ellipsis_slot_raw64(&slots[8])
#define SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_10, SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_9, sdl_native_ellipsis_slot_raw64(&slots[9])
#define SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_11, SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_10, sdl_native_ellipsis_slot_raw64(&slots[10])
#define SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_12, SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_11, sdl_native_ellipsis_slot_raw64(&slots[11])
#define SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_13, SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_12, sdl_native_ellipsis_slot_raw64(&slots[12])
#define SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_14, SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_13, sdl_native_ellipsis_slot_raw64(&slots[13])
#define SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_15, SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_14, sdl_native_ellipsis_slot_raw64(&slots[14])
#define SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_16, SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_15, sdl_native_ellipsis_slot_raw64(&slots[15])

#define SDL_NATIVE_ELLIPSIS_SNPRINTF_CASE(count) \
	case count: \
		return direct_fn(text, maxlen, fmt, SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_##count)

#define SDL_NATIVE_ELLIPSIS_ASPRINTF_CASE(count) \
	case count: \
		return direct_fn(text, fmt, SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_##count)

#define SDL_NATIVE_ELLIPSIS_SWPRINTF_CASE(count) \
	case count: \
		return direct_fn((wchar_t*)text, maxlen, (const wchar_t*)fmt, SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_##count)

#define SDL_NATIVE_ELLIPSIS_SET_ERROR_CASE(count) \
	case count: \
		return direct_fn(fmt, SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_##count) ? 1 : 0

#define SDL_NATIVE_ELLIPSIS_LOG_MESSAGE_CASE(count) \
	case count: \
		direct_fn(category, priority, fmt, SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_##count); \
		return 0

#define SDL_NATIVE_ELLIPSIS_IOPRINTF_CASE(count) \
	case count: \
		return direct_fn(context, fmt, SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_##count)

#define SDL_NATIVE_ELLIPSIS_RENDER_DEBUG_TEXT_FORMAT_CASE(count) \
	case count: \
		return direct_fn(renderer, x, y, fmt, SDL_NATIVE_ELLIPSIS_PRINTF_VALUE_##count) ? 1 : 0

SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_set_sdl_path(const char* path)
{
	sdl_native_ellipsis_reset_exports();
	
	if (g_sdl_module != NULL) {
#if defined(_WIN32)
		FreeLibrary(g_sdl_module);
#else
		dlclose(g_sdl_module);
#endif
		g_sdl_module = NULL;
	}

	const char* resolved_path;
#if defined(_WIN32)
	if (path != NULL && path[0] != 0) {
		resolved_path = path;
	}
	else {
		resolved_path = "SDL3.dll";
	}

	g_sdl_module = LoadLibraryA(resolved_path);
	if (g_sdl_module == NULL) {
		sdl_native_ellipsis_set_error_code("LoadLibraryA(SDL3.dll path) failed", GetLastError());
		return -1;
	}
#else
	if (path != NULL && path[0] != 0) {
		resolved_path = path;
	}
	else {
		resolved_path = "libSDL3.so";
	}

	g_sdl_module = dlopen(resolved_path, RTLD_NOW | RTLD_LOCAL);
	if (g_sdl_module == NULL) {
		sdl_native_ellipsis_set_last_error(dlerror());
		return -1;
	}
#endif

	if (sdl_native_ellipsis_get_sscanf() != NULL) {
		return 0;
	}

	return -2;
}

SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_get_last_error(char* buffer, size_t buffer_size)
{
	if (buffer == NULL || buffer_size == 0u) {
		return -1;
	}

	size_t length = strlen(g_last_error);
	size_t copy_length;
	if (length < buffer_size - 1u) {
		copy_length = length;
	}
	else {
		copy_length = buffer_size - 1u;
	}

	memcpy(buffer, g_last_error, copy_length);
	buffer[copy_length] = 0;

	return 0;
}

SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_snprintf(char* text, size_t maxlen, const char* fmt, const sdl_native_ellipsis_printf_slot* slots, size_t
																						slot_count)
{ 
	if (fmt == NULL || (text == NULL && maxlen != 0u)) {
		sdl_native_ellipsis_set_last_error("SDL_snprintf format pointer is null or text is null with nonzero max length");
		return SDL_NATIVE_ELLIPSIS_ERROR;
	}

	sdl_native_ellipsis_snprintf_fn direct_fn = sdl_native_ellipsis_get_snprintf();
	if (direct_fn == NULL) {
		return SDL_NATIVE_ELLIPSIS_ERROR;
	}

	if (slot_count == 0u) {
		return direct_fn(text, maxlen, fmt);
	}

	if (sdl_native_ellipsis_validate_printf_slots(slots, slot_count) != 0) {
		return SDL_NATIVE_ELLIPSIS_ERROR;
	}

	switch (slot_count) {
		SDL_NATIVE_ELLIPSIS_SNPRINTF_CASE(1);
		SDL_NATIVE_ELLIPSIS_SNPRINTF_CASE(2);
		SDL_NATIVE_ELLIPSIS_SNPRINTF_CASE(3);
		SDL_NATIVE_ELLIPSIS_SNPRINTF_CASE(4);
		SDL_NATIVE_ELLIPSIS_SNPRINTF_CASE(5);
		SDL_NATIVE_ELLIPSIS_SNPRINTF_CASE(6);
		SDL_NATIVE_ELLIPSIS_SNPRINTF_CASE(7);
		SDL_NATIVE_ELLIPSIS_SNPRINTF_CASE(8);
		SDL_NATIVE_ELLIPSIS_SNPRINTF_CASE(9);
		SDL_NATIVE_ELLIPSIS_SNPRINTF_CASE(10);
		SDL_NATIVE_ELLIPSIS_SNPRINTF_CASE(11);
		SDL_NATIVE_ELLIPSIS_SNPRINTF_CASE(12);
		SDL_NATIVE_ELLIPSIS_SNPRINTF_CASE(13);
		SDL_NATIVE_ELLIPSIS_SNPRINTF_CASE(14);
		SDL_NATIVE_ELLIPSIS_SNPRINTF_CASE(15);
		SDL_NATIVE_ELLIPSIS_SNPRINTF_CASE(16);
		default:
			sdl_native_ellipsis_set_last_error("unsupported printf slot count");

			return SDL_NATIVE_ELLIPSIS_SIZE_ERROR;
	}
}

SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_asprintf(char** text, const char* fmt, const sdl_native_ellipsis_printf_slot* slots, size_t slot_count)
{
	if (text == NULL || fmt == NULL) {
		sdl_native_ellipsis_set_last_error("SDL_asprintf text or format pointer is null");

		return SDL_NATIVE_ELLIPSIS_ERROR;
	}

	sdl_native_ellipsis_asprintf_fn direct_fn = sdl_native_ellipsis_get_asprintf();
	if (direct_fn == NULL) {
		return SDL_NATIVE_ELLIPSIS_ERROR;
	}

	if (slot_count == 0u) {
		return direct_fn(text, fmt);
	}

	if (sdl_native_ellipsis_validate_printf_slots(slots, slot_count) != 0) {
		return SDL_NATIVE_ELLIPSIS_ERROR;
	}

	switch (slot_count) {
		SDL_NATIVE_ELLIPSIS_ASPRINTF_CASE(1);
		SDL_NATIVE_ELLIPSIS_ASPRINTF_CASE(2);
		SDL_NATIVE_ELLIPSIS_ASPRINTF_CASE(3);
		SDL_NATIVE_ELLIPSIS_ASPRINTF_CASE(4);
		SDL_NATIVE_ELLIPSIS_ASPRINTF_CASE(5);
		SDL_NATIVE_ELLIPSIS_ASPRINTF_CASE(6);
		SDL_NATIVE_ELLIPSIS_ASPRINTF_CASE(7);
		SDL_NATIVE_ELLIPSIS_ASPRINTF_CASE(8);
		SDL_NATIVE_ELLIPSIS_ASPRINTF_CASE(9);
		SDL_NATIVE_ELLIPSIS_ASPRINTF_CASE(10);
		SDL_NATIVE_ELLIPSIS_ASPRINTF_CASE(11);
		SDL_NATIVE_ELLIPSIS_ASPRINTF_CASE(12);
		SDL_NATIVE_ELLIPSIS_ASPRINTF_CASE(13);
		SDL_NATIVE_ELLIPSIS_ASPRINTF_CASE(14);
		SDL_NATIVE_ELLIPSIS_ASPRINTF_CASE(15);
		SDL_NATIVE_ELLIPSIS_ASPRINTF_CASE(16);
		default:
			sdl_native_ellipsis_set_last_error("unsupported printf slot count");

			return SDL_NATIVE_ELLIPSIS_SIZE_ERROR;
	}
}

SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_swprintf(void* text, size_t maxlen, const void* fmt, const sdl_native_ellipsis_printf_slot* slots,
																						size_t slot_count, int wchar_size)
{
	if (fmt == NULL || (text == NULL && maxlen != 0u)) {
		sdl_native_ellipsis_set_last_error("SDL_swprintf format pointer is null or text is null with nonzero max length");

		return SDL_NATIVE_ELLIPSIS_ERROR;
	}

	if (wchar_size != (int)sizeof(wchar_t)) {
		sdl_native_ellipsis_set_last_error("managed wchar_t width does not match native helper");

		return SDL_NATIVE_ELLIPSIS_ERROR;
	}

	sdl_native_ellipsis_swprintf_fn direct_fn = sdl_native_ellipsis_get_swprintf();
	if (direct_fn == NULL) {
		return SDL_NATIVE_ELLIPSIS_ERROR;
	}

	if (slot_count == 0u) {
		return direct_fn((wchar_t*)text, maxlen, (const wchar_t*)fmt);
	}

	if (sdl_native_ellipsis_validate_printf_slots(slots, slot_count) != 0) {
		return SDL_NATIVE_ELLIPSIS_ERROR;
	}

	switch (slot_count) {
		SDL_NATIVE_ELLIPSIS_SWPRINTF_CASE(1);
		SDL_NATIVE_ELLIPSIS_SWPRINTF_CASE(2);
		SDL_NATIVE_ELLIPSIS_SWPRINTF_CASE(3);
		SDL_NATIVE_ELLIPSIS_SWPRINTF_CASE(4);
		SDL_NATIVE_ELLIPSIS_SWPRINTF_CASE(5);
		SDL_NATIVE_ELLIPSIS_SWPRINTF_CASE(6);
		SDL_NATIVE_ELLIPSIS_SWPRINTF_CASE(7);
		SDL_NATIVE_ELLIPSIS_SWPRINTF_CASE(8);
		SDL_NATIVE_ELLIPSIS_SWPRINTF_CASE(9);
		SDL_NATIVE_ELLIPSIS_SWPRINTF_CASE(10);
		SDL_NATIVE_ELLIPSIS_SWPRINTF_CASE(11);
		SDL_NATIVE_ELLIPSIS_SWPRINTF_CASE(12);
		SDL_NATIVE_ELLIPSIS_SWPRINTF_CASE(13);
		SDL_NATIVE_ELLIPSIS_SWPRINTF_CASE(14);
		SDL_NATIVE_ELLIPSIS_SWPRINTF_CASE(15);
		SDL_NATIVE_ELLIPSIS_SWPRINTF_CASE(16);
		default:
			sdl_native_ellipsis_set_last_error("unsupported printf slot count");

			return SDL_NATIVE_ELLIPSIS_SIZE_ERROR;
	}
}

SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_set_error(const char* fmt, const sdl_native_ellipsis_printf_slot* slots, size_t slot_count)
{
	if (fmt == NULL) {
		sdl_native_ellipsis_set_last_error("SDL_SetError format pointer is null");

		return SDL_NATIVE_ELLIPSIS_ERROR;
	}

	sdl_native_ellipsis_set_error_fn direct_fn = sdl_native_ellipsis_get_set_error();
	if (direct_fn == NULL) {
		return SDL_NATIVE_ELLIPSIS_ERROR;
	}

	if (slot_count == 0u) {
		if (direct_fn(fmt)) {
			return 1;
		}
		else {
			return 0;
		}
	}

	if(sdl_native_ellipsis_validate_printf_slots(slots, slot_count) != 0) {
		return SDL_NATIVE_ELLIPSIS_ERROR;
	}

	switch (slot_count) {
		SDL_NATIVE_ELLIPSIS_SET_ERROR_CASE(1);
		SDL_NATIVE_ELLIPSIS_SET_ERROR_CASE(2);
		SDL_NATIVE_ELLIPSIS_SET_ERROR_CASE(3);
		SDL_NATIVE_ELLIPSIS_SET_ERROR_CASE(4);
		SDL_NATIVE_ELLIPSIS_SET_ERROR_CASE(5);
		SDL_NATIVE_ELLIPSIS_SET_ERROR_CASE(6);
		SDL_NATIVE_ELLIPSIS_SET_ERROR_CASE(7);
		SDL_NATIVE_ELLIPSIS_SET_ERROR_CASE(8);
		SDL_NATIVE_ELLIPSIS_SET_ERROR_CASE(9);
		SDL_NATIVE_ELLIPSIS_SET_ERROR_CASE(10);
		SDL_NATIVE_ELLIPSIS_SET_ERROR_CASE(11);
		SDL_NATIVE_ELLIPSIS_SET_ERROR_CASE(12);
		SDL_NATIVE_ELLIPSIS_SET_ERROR_CASE(13);
		SDL_NATIVE_ELLIPSIS_SET_ERROR_CASE(14);
		SDL_NATIVE_ELLIPSIS_SET_ERROR_CASE(15);
		SDL_NATIVE_ELLIPSIS_SET_ERROR_CASE(16);
		default:
			sdl_native_ellipsis_set_last_error("unsupported printf slot count");

			return SDL_NATIVE_ELLIPSIS_SIZE_ERROR;
	}
}

SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_log_message(int category, int priority, const char* fmt, const sdl_native_ellipsis_printf_slot* slots,
																						   size_t slot_count)
{
	if (fmt == NULL) {
		sdl_native_ellipsis_set_last_error("SDL_LogMessage format pointer is null");

		return SDL_NATIVE_ELLIPSIS_ERROR;
	}

	sdl_native_ellipsis_log_message_fn direct_fn = sdl_native_ellipsis_get_log_message();
	if (direct_fn == NULL) {
		return SDL_NATIVE_ELLIPSIS_ERROR;
	}

	if (slot_count == 0u) {
		direct_fn(category, priority, fmt);

		return 0;
	}

	if (sdl_native_ellipsis_validate_printf_slots(slots, slot_count) != 0) {
		return SDL_NATIVE_ELLIPSIS_ERROR;
	}

	switch (slot_count) {
		SDL_NATIVE_ELLIPSIS_LOG_MESSAGE_CASE(1);
		SDL_NATIVE_ELLIPSIS_LOG_MESSAGE_CASE(2);
		SDL_NATIVE_ELLIPSIS_LOG_MESSAGE_CASE(3);
		SDL_NATIVE_ELLIPSIS_LOG_MESSAGE_CASE(4);
		SDL_NATIVE_ELLIPSIS_LOG_MESSAGE_CASE(5);
		SDL_NATIVE_ELLIPSIS_LOG_MESSAGE_CASE(6);
		SDL_NATIVE_ELLIPSIS_LOG_MESSAGE_CASE(7);
		SDL_NATIVE_ELLIPSIS_LOG_MESSAGE_CASE(8);
		SDL_NATIVE_ELLIPSIS_LOG_MESSAGE_CASE(9);
		SDL_NATIVE_ELLIPSIS_LOG_MESSAGE_CASE(10);
		SDL_NATIVE_ELLIPSIS_LOG_MESSAGE_CASE(11);
		SDL_NATIVE_ELLIPSIS_LOG_MESSAGE_CASE(12);
		SDL_NATIVE_ELLIPSIS_LOG_MESSAGE_CASE(13);
		SDL_NATIVE_ELLIPSIS_LOG_MESSAGE_CASE(14);
		SDL_NATIVE_ELLIPSIS_LOG_MESSAGE_CASE(15);
		SDL_NATIVE_ELLIPSIS_LOG_MESSAGE_CASE(16);
		default:
			sdl_native_ellipsis_set_last_error("unsupported printf slot count");

			return SDL_NATIVE_ELLIPSIS_SIZE_ERROR;
	}
}

SDL3_NATIVE_ELLIPSIS_EXPORT size_t SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_ioprintf(void* context, const char* fmt, const sdl_native_ellipsis_printf_slot* slots, size_t slot_count)
{
	if (context == NULL || fmt == NULL) {
		sdl_native_ellipsis_set_last_error("SDL_IOprintf context or format pointer is null");

		return SDL_NATIVE_ELLIPSIS_ERROR;
	}

	sdl_native_ellipsis_ioprintf_fn direct_fn = sdl_native_ellipsis_get_ioprintf();
	if (direct_fn == NULL) {
		return SDL_NATIVE_ELLIPSIS_ERROR;
	}

	if (slot_count == 0u) {
		return direct_fn(context, fmt);
	}

	if (sdl_native_ellipsis_validate_printf_slots(slots, slot_count) != 0) {
		return SDL_NATIVE_ELLIPSIS_ERROR;
	}

	switch (slot_count) {
		SDL_NATIVE_ELLIPSIS_IOPRINTF_CASE(1);
		SDL_NATIVE_ELLIPSIS_IOPRINTF_CASE(2);
		SDL_NATIVE_ELLIPSIS_IOPRINTF_CASE(3);
		SDL_NATIVE_ELLIPSIS_IOPRINTF_CASE(4);
		SDL_NATIVE_ELLIPSIS_IOPRINTF_CASE(5);
		SDL_NATIVE_ELLIPSIS_IOPRINTF_CASE(6);
		SDL_NATIVE_ELLIPSIS_IOPRINTF_CASE(7);
		SDL_NATIVE_ELLIPSIS_IOPRINTF_CASE(8);
		SDL_NATIVE_ELLIPSIS_IOPRINTF_CASE(9);
		SDL_NATIVE_ELLIPSIS_IOPRINTF_CASE(10);
		SDL_NATIVE_ELLIPSIS_IOPRINTF_CASE(11);
		SDL_NATIVE_ELLIPSIS_IOPRINTF_CASE(12);
		SDL_NATIVE_ELLIPSIS_IOPRINTF_CASE(13);
		SDL_NATIVE_ELLIPSIS_IOPRINTF_CASE(14);
		SDL_NATIVE_ELLIPSIS_IOPRINTF_CASE(15);
		SDL_NATIVE_ELLIPSIS_IOPRINTF_CASE(16);
		default:
			sdl_native_ellipsis_set_last_error("unsupported printf slot count");

			return SDL_NATIVE_ELLIPSIS_SIZE_ERROR;
	}
}

SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_render_debug_text_format(void* renderer, float x, float y, const char* fmt,
																										const sdl_native_ellipsis_printf_slot* slots, size_t slot_count)
{
	if (renderer == NULL || fmt == NULL) {
		sdl_native_ellipsis_set_last_error("SDL_RenderDebugTextFormat renderer or format pointer is null");

		return SDL_NATIVE_ELLIPSIS_ERROR;
	}

	sdl_native_ellipsis_render_debug_text_format_fn direct_fn = sdl_native_ellipsis_get_render_debug_text_format();
	if (direct_fn == NULL) {
		return SDL_NATIVE_ELLIPSIS_ERROR;
	}

	if (slot_count == 0u) {
		if (direct_fn(renderer, x, y, fmt)) {
			return 1;
		}
		else {
			return 0;
		}
	}

	if (sdl_native_ellipsis_validate_printf_slots(slots, slot_count) != 0) {
		return SDL_NATIVE_ELLIPSIS_ERROR;
	}

	switch (slot_count) {
		SDL_NATIVE_ELLIPSIS_RENDER_DEBUG_TEXT_FORMAT_CASE(1);
		SDL_NATIVE_ELLIPSIS_RENDER_DEBUG_TEXT_FORMAT_CASE(2);
		SDL_NATIVE_ELLIPSIS_RENDER_DEBUG_TEXT_FORMAT_CASE(3);
		SDL_NATIVE_ELLIPSIS_RENDER_DEBUG_TEXT_FORMAT_CASE(4);
		SDL_NATIVE_ELLIPSIS_RENDER_DEBUG_TEXT_FORMAT_CASE(5);
		SDL_NATIVE_ELLIPSIS_RENDER_DEBUG_TEXT_FORMAT_CASE(6);
		SDL_NATIVE_ELLIPSIS_RENDER_DEBUG_TEXT_FORMAT_CASE(7);
		SDL_NATIVE_ELLIPSIS_RENDER_DEBUG_TEXT_FORMAT_CASE(8);
		SDL_NATIVE_ELLIPSIS_RENDER_DEBUG_TEXT_FORMAT_CASE(9);
		SDL_NATIVE_ELLIPSIS_RENDER_DEBUG_TEXT_FORMAT_CASE(10);
		SDL_NATIVE_ELLIPSIS_RENDER_DEBUG_TEXT_FORMAT_CASE(11);
		SDL_NATIVE_ELLIPSIS_RENDER_DEBUG_TEXT_FORMAT_CASE(12);
		SDL_NATIVE_ELLIPSIS_RENDER_DEBUG_TEXT_FORMAT_CASE(13);
		SDL_NATIVE_ELLIPSIS_RENDER_DEBUG_TEXT_FORMAT_CASE(14);
		SDL_NATIVE_ELLIPSIS_RENDER_DEBUG_TEXT_FORMAT_CASE(15);
		SDL_NATIVE_ELLIPSIS_RENDER_DEBUG_TEXT_FORMAT_CASE(16);
		default:
			sdl_native_ellipsis_set_last_error("unsupported printf slot count");

			return SDL_NATIVE_ELLIPSIS_ERROR;
	}
}

SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_sscanf_0(const char* text, const char* fmt)
{
	sdl_native_ellipsis_sscanf_fn fn = sdl_native_ellipsis_get_sscanf();

	if (fn != NULL) {
		return fn(text, fmt);
	}

	return SDL_NATIVE_ELLIPSIS_ERROR;
}

#define SDL_NATIVE_ELLIPSIS_PARAMETERS_1 void* a0
#define SDL_NATIVE_ELLIPSIS_PARAMETERS_2 SDL_NATIVE_ELLIPSIS_PARAMETERS_1, void* a1
#define SDL_NATIVE_ELLIPSIS_PARAMETERS_3 SDL_NATIVE_ELLIPSIS_PARAMETERS_2, void* a2
#define SDL_NATIVE_ELLIPSIS_PARAMETERS_4 SDL_NATIVE_ELLIPSIS_PARAMETERS_3, void* a3
#define SDL_NATIVE_ELLIPSIS_PARAMETERS_5 SDL_NATIVE_ELLIPSIS_PARAMETERS_4, void* a4
#define SDL_NATIVE_ELLIPSIS_PARAMETERS_6 SDL_NATIVE_ELLIPSIS_PARAMETERS_5, void* a5
#define SDL_NATIVE_ELLIPSIS_PARAMETERS_7 SDL_NATIVE_ELLIPSIS_PARAMETERS_6, void* a6
#define SDL_NATIVE_ELLIPSIS_PARAMETERS_8 SDL_NATIVE_ELLIPSIS_PARAMETERS_7, void* a7
#define SDL_NATIVE_ELLIPSIS_PARAMETERS_9 SDL_NATIVE_ELLIPSIS_PARAMETERS_8, void* a8
#define SDL_NATIVE_ELLIPSIS_PARAMETERS_10 SDL_NATIVE_ELLIPSIS_PARAMETERS_9, void* a9
#define SDL_NATIVE_ELLIPSIS_PARAMETERS_11 SDL_NATIVE_ELLIPSIS_PARAMETERS_10, void* a10
#define SDL_NATIVE_ELLIPSIS_PARAMETERS_12 SDL_NATIVE_ELLIPSIS_PARAMETERS_11, void* a11
#define SDL_NATIVE_ELLIPSIS_PARAMETERS_13 SDL_NATIVE_ELLIPSIS_PARAMETERS_12, void* a12
#define SDL_NATIVE_ELLIPSIS_PARAMETERS_14 SDL_NATIVE_ELLIPSIS_PARAMETERS_13, void* a13
#define SDL_NATIVE_ELLIPSIS_PARAMETERS_15 SDL_NATIVE_ELLIPSIS_PARAMETERS_14, void* a14
#define SDL_NATIVE_ELLIPSIS_PARAMETERS_16 SDL_NATIVE_ELLIPSIS_PARAMETERS_15, void* a15

#define SDL_NATIVE_ELLIPSIS_ARGUMENTS_1 a0
#define SDL_NATIVE_ELLIPSIS_ARGUMENTS_2 SDL_NATIVE_ELLIPSIS_ARGUMENTS_1, a1
#define SDL_NATIVE_ELLIPSIS_ARGUMENTS_3 SDL_NATIVE_ELLIPSIS_ARGUMENTS_2, a2
#define SDL_NATIVE_ELLIPSIS_ARGUMENTS_4 SDL_NATIVE_ELLIPSIS_ARGUMENTS_3, a3
#define SDL_NATIVE_ELLIPSIS_ARGUMENTS_5 SDL_NATIVE_ELLIPSIS_ARGUMENTS_4, a4
#define SDL_NATIVE_ELLIPSIS_ARGUMENTS_6 SDL_NATIVE_ELLIPSIS_ARGUMENTS_5, a5
#define SDL_NATIVE_ELLIPSIS_ARGUMENTS_7 SDL_NATIVE_ELLIPSIS_ARGUMENTS_6, a6
#define SDL_NATIVE_ELLIPSIS_ARGUMENTS_8 SDL_NATIVE_ELLIPSIS_ARGUMENTS_7, a7
#define SDL_NATIVE_ELLIPSIS_ARGUMENTS_9 SDL_NATIVE_ELLIPSIS_ARGUMENTS_8, a8
#define SDL_NATIVE_ELLIPSIS_ARGUMENTS_10 SDL_NATIVE_ELLIPSIS_ARGUMENTS_9, a9
#define SDL_NATIVE_ELLIPSIS_ARGUMENTS_11 SDL_NATIVE_ELLIPSIS_ARGUMENTS_10, a10
#define SDL_NATIVE_ELLIPSIS_ARGUMENTS_12 SDL_NATIVE_ELLIPSIS_ARGUMENTS_11, a11
#define SDL_NATIVE_ELLIPSIS_ARGUMENTS_13 SDL_NATIVE_ELLIPSIS_ARGUMENTS_12, a12
#define SDL_NATIVE_ELLIPSIS_ARGUMENTS_14 SDL_NATIVE_ELLIPSIS_ARGUMENTS_13, a13
#define SDL_NATIVE_ELLIPSIS_ARGUMENTS_15 SDL_NATIVE_ELLIPSIS_ARGUMENTS_14, a14
#define SDL_NATIVE_ELLIPSIS_ARGUMENTS_16 SDL_NATIVE_ELLIPSIS_ARGUMENTS_15, a15

#define SDL_NATIVE_ELLIPSIS_DEFINE(count) \
	SDL3_NATIVE_ELLIPSIS_EXPORT int SDL3_NATIVE_ELLIPSIS_CDECL sdl_native_ellipsis_sscanf_##count(const char* text, const char* fmt, SDL_NATIVE_ELLIPSIS_PARAMETERS_##count) \
	{ \ 
		sdl_native_ellipsis_sscanf_fn fn = sdl_native_ellipsis_get_sscanf(); \
		if(fn != NULL) { \
			return fn(text, fmt, SDL_NATIVE_ELLIPSIS_ARGUMENTS_##count); \
		} \
		else { \
			return SDL_NATIVE_ELLIPSIS_ERROR; \
		} \
	}

SDL_NATIVE_ELLIPSIS_DEFINE(1)
SDL_NATIVE_ELLIPSIS_DEFINE(2)
SDL_NATIVE_ELLIPSIS_DEFINE(3)
SDL_NATIVE_ELLIPSIS_DEFINE(4)
SDL_NATIVE_ELLIPSIS_DEFINE(5)
SDL_NATIVE_ELLIPSIS_DEFINE(6)
SDL_NATIVE_ELLIPSIS_DEFINE(7)
SDL_NATIVE_ELLIPSIS_DEFINE(8)
SDL_NATIVE_ELLIPSIS_DEFINE(9)
SDL_NATIVE_ELLIPSIS_DEFINE(10)
SDL_NATIVE_ELLIPSIS_DEFINE(11)
SDL_NATIVE_ELLIPSIS_DEFINE(12)
SDL_NATIVE_ELLIPSIS_DEFINE(13)
SDL_NATIVE_ELLIPSIS_DEFINE(14)
SDL_NATIVE_ELLIPSIS_DEFINE(15)
SDL_NATIVE_ELLIPSIS_DEFINE(16)