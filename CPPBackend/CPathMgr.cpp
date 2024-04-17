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
//		// �̿� ������ ��ġ üũ
//		// ���� ���� �̿� ������� Ȯ��
//		for (int i = 0; i < 8; ++i)
//		{
//			if (curNode.x + X[i] < 0 || curNode.y + Y[i] < 0 || curNode.x + X[i] >= i_worldSizeX || curNode.y + Y[i] >= i_worldSizeY)
//			{
//				continue;
//			}
//			// ���� �ƴ϶�� ������. >> ���� ������ �߰� ����̰ų� ��ǥ ����� ���
//			NodeClass nextNode = grid[curNode.y + Y[i]][curNode.x + X[i]];
//			if (nextNode.isWalkable) {
//				//  F, G, H ��, parent node ���� �� ���� ����Ʈ�� �ֱ�
//				nextNode.g = GetGCost(curNode, nextNode);
//				nextNode.h = GetHCost(nextNode);
//				nextNode.f = nextNode.g + nextNode.h;
//				openList.insert(nextNode);
//
//				// map������ �ݿ����ֱ�
//				grid[curNode.y + Y[i]][curNode.x + X[i]] = nextNode;
//
//				// ���� ����Ʈ�� �� ���� ��尡 ��ǥ ����� ��� >> ���� ��Ͽ� �ٷ� �ְ� while�� ����
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