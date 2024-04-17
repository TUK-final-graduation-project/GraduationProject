#include "CPathMgr.h"
#include <set>

CPathMgr* CPathMgr::g_pInstance = nullptr;

void CPathMgr::setPlayerTarget(int playerX, int playerY, int targetX, int targetY)
{
	startNode.x = playerX;
	startNode.y = playerY;

	endNode.x = targetX;
	endNode.y = targetY;
}

CPathMgr::CPathMgr()
{
	i_worldSizeX = 0;
	i_worldSizeY = 0;
}
CPathMgr::~CPathMgr()
{
}
//void CPathMgr::FindPath()
//{
//
//	std::multiset<NodeClass> openList{};
//	closeList.push_back(startNode);
//
//	int X[8] = { 1, 0, -1, 1, 0, -1, 1, -1 };
//	int Y[8] = { 1, 1, 1, -1, -1, -1, 0, 0 };
//	bool running = true;
//	while (running)
//	{
//		NodeClass curNode = closeList.back();
//		// 이웃 노드들의 위치 체크
//		// 영역 밖의 이웃 노드인지 확인
//		for (int i = 0; i < 8; ++i)
//		{
//			if (curNode.x + X[i] < 0 || curNode.y + Y[i] < 0 || curNode.x + X[i] >= i_worldSizeX || curNode.y + Y[i] >= i_worldSizeY)
//			{
//				continue;
//			}
//			// 벽이 아니라면 진행함. >> 접근 가능한 중간 노드이거나 목표 노드인 경우
//			NodeClass nextNode = grid[curNode.y + Y[i]][curNode.x + X[i]];
//			if (nextNode.isWalkable) {
//				//  F, G, H 값, parent node 정의 후 오픈 리스트에 넣기
//				nextNode.g = GetGCost(curNode, nextNode);
//				nextNode.h = GetHCost(nextNode);
//				nextNode.f = nextNode.g + nextNode.h;
//				openList.insert(nextNode);
//
//				// map에서도 반영해주기
//				grid[curNode.y + Y[i]][curNode.x + X[i]] = nextNode;
//
//				// 오픈 리스트에 들어갈 현재 노드가 목표 노드인 경우 >> 닫힌 목록에 바로 넣고 while문 종료
//				if (nextNode == endNode)
//				{
//					closeList.push_back(nextNode);
//					running = false;
//				}
//			}
//		}
//		if (running)
//		{
//			closeList.push_back(*(openList.begin()));
//			openList.clear();
//		}
//	}
//}