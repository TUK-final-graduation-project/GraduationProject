#ifndef CPPDLL_H
#define CPPDLL_H

#define DLLAPI extern "C" __declspec(dllexport)

#pragma pack(1)
struct _cppNode {
	int x;
	int y;
	bool walkable;
}typedef cppNode;


DLLAPI int GridInfo(cppNode* nodes, int xSize, int ySize);
DLLAPI void MakeRoute(int sX, int sY, int eX, int eY, cppNode** route, int* nCount);
#endif