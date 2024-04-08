#ifndef BACKEND_H
#define BACKEND_H

#define DLL_EXPORT __declspec(dllexport)

extern "C" {
	const char* DLL_EXPORT SimpleReturnFun(float x, float y, bool isWalkable);
	int DLL_EXPORT Re(int* arr, int size);
	int DLL_EXPORT SendWorldSize(int x, int y);
	bool DLL_EXPORT SendNodeInfo(int x, int y, bool isWalkable);
	int DLL_EXPORT SendStartNode(int x, int y);
	int DLL_EXPORT SendEndNode(int x, int y);
	bool DLL_EXPORT CompStartNode(int x, int y);
	bool DLL_EXPORT CompEndNode(int x, int y);
	int DLL_EXPORT ReceivePath();
	void DLL_EXPORT DeleteAstar();
}

#endif