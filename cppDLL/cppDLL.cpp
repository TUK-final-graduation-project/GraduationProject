#include "cppDLL.h"
#include "AstarMgr.h"
#include <vector>

AstarMgr mgr;

int GridInfo(cppNode* nodes, int xSize, int ySize)
{
	mgr.gridSizeX = xSize;
	mgr.gridSizeY = ySize;

	mgr.grid = new Node* [xSize];

	int index = 0;
	for (int i = 0; i < xSize; ++i) {
		mgr.grid[i] = new Node[ySize];
		for (int j = 0; j < ySize; ++j) {
			mgr.grid[i][j].SetNode(nodes[index]);
			index++;
		}
	}
	return mgr.grid[0][0].GetNodeX();
}

DLLAPI void MakeRoute(int sX, int sY, int eX, int eY, cppNode** route, int* nCount)
{
	cppNode tmp;
	tmp.x = sX;
	tmp.y = sY;
	mgr.startNode.SetNode(tmp);
	tmp.x = eX;
	tmp.y = eY;
	mgr.endNode.SetNode(tmp);


	mgr.FindPath();

	*nCount = mgr.closeList.size();
	*route = new cppNode[*nCount];

	for (int i = 0; i < *nCount; ++i) {
		(*route)[i].x = mgr.closeList[i].GetNodeX();
		(*route)[i].y = mgr.closeList[i].GetNodeY();
		(*route)[i].walkable = true;
	}
	mgr.closeList.clear();
}
