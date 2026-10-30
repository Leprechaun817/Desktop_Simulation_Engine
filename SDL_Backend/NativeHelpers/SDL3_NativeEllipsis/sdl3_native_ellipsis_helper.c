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

#define SDL3_NATIVE_ELLIPSIS_ERROR (-32000)
#define SDL3_NATIVE_ELLIPSIS_SIZE_RROR ((size_t)-1)
#define SDL3_NATIVE_ELLIPSIS_MESSAGE_BYTES 256u
#define SDL3_NATIVE_ELLIPSIS_MAX_PRINTF_SLOTS 16u

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

	}
}