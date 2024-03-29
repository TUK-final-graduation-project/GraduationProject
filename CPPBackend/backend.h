#ifndef BACKEND_H
#define BACKEND_H

#define DLL_EXPORT __declspec(dllexport)

extern "C" {
	const char* DLL_EXPORT SimpleReturnFun(float x, float y, bool isWalkable);
	int DLL_EXPORT Re(int* arr, int size);
	int DLL_EXPORT SendWorldSize(int x, int y);
	bool DLL_EXPORT SendNodeInfo(int x, int y, bool isWalkable);
}

#endif