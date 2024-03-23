#pragma once
#ifdef PLUGIN_H
#define PLUGIN_H

#define DLL_EXPORT __declspec(dllexport)

extern "C" {
	int DLL_EXPORT SimpleReturnFun();
}

#endif

