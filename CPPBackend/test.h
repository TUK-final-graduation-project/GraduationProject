#ifndef TEST_H
#define TEST_H

#define DLL_EXPORT __declspec(dllexport)

#pragma pack(1)
struct _stMyst {
	int nData;
	int yData;
	bool bTrue;
} typedef MyStruct;

#pragma pack(1)
struct _stNode {
	int x;
	int y;
	bool isWalkable;
} typedef NodeClass;

extern "C" {
	DLL_EXPORT int RecvGridInfo(NodeClass* n, int xSize, int ySize, int index);

	DLL_EXPORT void SendRoute(NodeClass** pStruct, int* nCount);

	DLL_EXPORT int SendPlayerTarget(int playerX, int playerY, int targetX, int targetY);
}

#endif