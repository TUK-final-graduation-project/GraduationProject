#pragma once

#ifdef BUILD_DLL
	#define DLL_EXPORT __declspec(dllexport)
#else
	#define DLL_EXPORT __declspec(dllimport)
#endif

extern "C" {
	int DLL_EXPORT SimpleTypeReturnFun();
}