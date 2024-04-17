#include "AstarMgr.h"
#include <set>
#include <algorithm>

void AstarMgr::FindPath()
{
	// std::multiset<Node> openList;

	int neighborNodeX[8] = { -1, -1, -1, 0, 0, 1, 1, 1 };
	int neighborNodeY[8] = { -1, 0, 1, -1, 1, -1, 0, 1 };

	//bool running = true;
	closeList.push_back(startNode);
	Node curNode = closeList.back();
	for (int i = 0; i < 8; ++i) {
		if ((curNode.GetNodeX() + neighborNodeX[i] >= 0 && curNode.GetNodeY() + neighborNodeY[i] >= 0)) {
			Node nextNode = grid[curNode.GetNodeX() + neighborNodeX[i]][curNode.GetNodeY() + neighborNodeY[i]];
			if (nextNode.GetWalkable()) {
		/*		nextNode.SetGCost(curNode, nextNode);
				nextNode.SetHCost(nextNode, endNode);
				nextNode.SetFCost();
				nextNode.SetParentNode(curNode);*/
				closeList.push_back(nextNode);
			}
		}
	}
	//openList.insert(closeList.front());
	//while (running) {

	//	Node curNode = closeList.back();

	//	for (int i = 0; i < 8; ++i) {
	//		if ((curNode.GetNodeX() + neighborNodeX[i] < 0 && curNode.GetNodeY() + neighborNodeY[i] < 0) ||
	//			(curNode.GetNodeX() + neighborNodeX[i] >= gridSizeX || curNode.GetNodeY() + neighborNodeY[i] >= gridSizeY)) {
	//			continue;
	//		}
	//		Node nextNode = grid[curNode.GetNodeX() + neighborNodeX[i]][curNode.GetNodeY() + neighborNodeY[i]];
	//		if (nextNode.GetWalkable()) {
	//			nextNode.SetGCost(curNode, nextNode);
	//			nextNode.SetHCost(nextNode, endNode);
	//			nextNode.SetFCost();
	//			nextNode.SetParentNode(curNode);
	//			openList.insert(nextNode);

	//			// map에서도 반영해주기
	//			// grid[curNode.GetNodeX() + neigberNodeX[i]][curNode.GetNodeY() + neigberNodeY[i]] = nextNode;
	//			if (nextNode == endNode)
	//			{
	//				closeList.push_back(nextNode);
	//				running = false;
	//				openList.clear();
	//			}
	//		}
	//	}
	//	if (running)
	//	{
	//		closeList.push_back(*(openList.begin()));
	//		openList.clear();
	//	}
	//	running = false;
	//}
}
