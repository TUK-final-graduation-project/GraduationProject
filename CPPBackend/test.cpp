#include "test.h"
#include "CPathMgr.h"

DLL_EXPORT int RecvGridInfo(NodeClass* n, int xSize, int ySize, int index)
{
	CPathMgr::GetInstance()->i_worldSizeX = xSize;
	CPathMgr::GetInstance()->i_worldSizeY = ySize;
	CPathMgr::GetInstance()->grid = new NodeClass[xSize * ySize];
	CPathMgr::GetInstance()->grid = n;
	return CPathMgr::GetInstance()->grid[index].x;
}

DLL_EXPORT void SendRoute(NodeClass** pStruct, int* nCount) {

	*nCount = CPathMgr::GetInstance()->i_worldSizeX * CPathMgr::GetInstance()->i_worldSizeY;
	*pStruct = new NodeClass[*nCount];
	for (int i = 0; i < *nCount; ++i)
	{
		(*pStruct)[i].x = CPathMgr::GetInstance()->grid[i].x;
		(*pStruct)[i].y = CPathMgr::GetInstance()->grid[i].y;
		(*pStruct)[i].isWalkable = CPathMgr::GetInstance()->grid[i].isWalkable;

	}
}

DLL_EXPORT int SendPlayerTarget(int playerX, int playerY, int targetX, int targetY)
{
	CPathMgr::GetInstance()->setPlayerTarget(playerX, playerY, targetX, targetY);
	return CPathMgr::GetInstance()->getPlayerX();
}
