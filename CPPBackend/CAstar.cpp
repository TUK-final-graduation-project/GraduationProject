#include "CAstar.h"
#include <vector>
#include <set>
#include <algorithm>
#include <math.h>

CAstar::CAstar() {
	worldSizeX = 0;
	worldSizeY = 0;
}
CAstar::~CAstar()
{
}

void CAstar::SetGridSize(int x, int y)
{
	worldSizeX = x;
	worldSizeY = y;

	grid.resize(worldSizeX, std::vector<CNode>(worldSizeY));
}

void CAstar::SetNodeInfo(int x, int y, bool isWalkable)
{
	grid[x][y].isWalkable = isWalkable;
	grid[x][y].x = x;
	grid[x][y].y = y;
}

void CAstar::SetStartNode(int x, int y)
{
	startNode.x = x;
	startNode.y = y;
}

void CAstar::SetEndNode(int x, int y)
{
	endNode.x = x;
	endNode.y = y;
}

void CAstar::FindPath()
{

	std::multiset<CNode> openList{};
	closeList.push_back(startNode);

	int X[8] = { 1, 0, -1, 1, 0, -1, 1, -1 };
	int Y[8] = { 1, 1, 1, -1, -1, -1, 0, 0 };
	bool running = true;
	while (running)
	{
		CNode curNode = closeList.back();
		// 이웃 노드들의 위치 체크
		// 영역 밖의 이웃 노드인지 확인
		for (int i = 0; i < 8; ++i)
		{
			if (curNode.x + X[i] < 0 || curNode.y + Y[i] < 0 || curNode.x + X[i] >= worldSizeX || curNode.y + Y[i] >= worldSizeY)
			{
				continue;
			}
			// 벽이 아니라면 진행함. >> 접근 가능한 중간 노드이거나 목표 노드인 경우
			CNode nextNode = grid[curNode.y + Y[i]][curNode.x + X[i]];
			if (nextNode.isWalkable) {
				//  F, G, H 값, parent node 정의 후 오픈 리스트에 넣기
				nextNode.g = GetGCost(curNode, nextNode);
				nextNode.h = GetHCost(nextNode);
				nextNode.f = nextNode.g + nextNode.h;
				nextNode.parent = &curNode;
				openList.insert(nextNode);

				// map에서도 반영해주기
				grid[curNode.y + Y[i]][curNode.x + X[i]] = nextNode;

				// 오픈 리스트에 들어갈 현재 노드가 목표 노드인 경우 >> 닫힌 목록에 바로 넣고 while문 종료
				if (nextNode == endNode)
				{
					closeList.push_back(nextNode);
					running = false;

					// path.push_back(closeList.back());
					//std::vector<std::vector<CNode>>::iterator outerIt;
					//std::vector<CNode>::iterator innerIt;
					//for (const auto& node : closeList) {
					//	for (outerIt = grid.begin(); outerIt != grid.end(); ++outerIt) {
					//		innerIt = std::find(outerIt->begin(), outerIt->end(), node);

					//		if (innerIt != outerIt->end()) {
					//			// innerIt->type = 'O';
					//			path.push_back(*innerIt);
					//			break; // 값 찾으면 루프 종료
					//		}
					//	}
					//}
				}
			}
		}
		if (running)
		{
			closeList.push_back(*(openList.begin()));
			openList.clear();
		}
	}
}

double CAstar::GetGCost(CNode a, CNode b)
{
	double cost = sqrt(pow(b.x - a.x, 2) + pow(b.y - a.y, 2));
	return double(a.g + cost);
}

void CAstar::make_route()
{
	//std::vector<std::vector<CNode>>::iterator outerIt;
	//std::vector<CNode>::iterator innerIt;
	//for (const auto& node : closeList) {
	//	for (outerIt = grid.begin(); outerIt != grid.end(); ++outerIt) {
	//		innerIt = std::find(outerIt->begin(), outerIt->end(), node);

	//		if (innerIt != outerIt->end()) {
	//			// innerIt->type = 'O';
	//			path.push_back(*innerIt);
	//			break; // 값 찾으면 루프 종료
	//		}
	//	}
	//}

}

double CAstar::GetHCost(CNode a)
{
	// 목표 노드와 현재 노드까지의 추상 거리
	return sqrt(pow(endNode.x - a.x, 2) + pow(endNode.y - a.y, 2));
}
