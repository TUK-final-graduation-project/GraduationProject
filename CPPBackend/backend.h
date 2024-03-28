#ifndef BACKEND_H
#define BACKEND_H

#define DLL_EXPORT __declspec(dllexport)

extern "C" {
	const char* DLL_EXPORT SimpleReturnFun(float x, float y, bool isWalkable);
}

#endif