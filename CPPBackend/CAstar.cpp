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

}

void CAstar::SetEndNode(int x, int y)
{

}

void CAstar::FindPath()
{
	std::vector<CNode> closeList{};
	std::multiset<CNode> openList{};
	closeList.push_back(startNode);

	int X[8] = { 1, 0, -1, 1, 0, -1, 1, -1 };
	int Y[8] = { 1, 1, 1, -1, -1, -1, 0, 0 };
	bool running = true;
	while (running)
	{
		CNode curNode = closeList.back();
		// �̿� ������ ��ġ üũ
		// ���� ���� �̿� ������� Ȯ��
		for (int i = 0; i < 8; ++i)
		{
			if (curNode.x + X[i] < 0 || curNode.y + Y[i] < 0 || curNode.x + X[i] >= worldSizeX || curNode.y + Y[i] >= worldSizeY)
			{
				continue;
			}
			// ���� �ƴ϶�� ������. >> ���� ������ �߰� ����̰ų� ��ǥ ����� ���
			CNode nextNode = grid[curNode.y + Y[i]][curNode.x + X[i]];
			if (nextNode.isWalkable) {
				//  F, G, H ��, parent node ���� �� ���� ����Ʈ�� �ֱ�
				nextNode.g = GetGCost(curNode, nextNode);
				nextNode.h = GetHCost(nextNode);
				nextNode.f = nextNode.g + nextNode.h;
				nextNode.parent = &curNode;
				openList.insert(nextNode);

				// map������ �ݿ����ֱ�
				grid[curNode.y + Y[i]][curNode.x + X[i]] = nextNode;

				// ���� ����Ʈ�� �� ���� ��尡 ��ǥ ����� ��� >> ���� ��Ͽ� �ٷ� �ְ� while�� ����
				if (nextNode == endNode)
				{
					closeList.push_back(nextNode);
					running = false;

					// ���� ��� ���
					//for (const auto& node : closeList) {
					//	std::cout << "closeList, x, y, type: " << node.x << ", " << node.y << ", " << node.type << std::endl;
					//}
					make_route(closeList);
				}
			}
		}
		if (running)
		{
			// ���� ����� ���� ó�� ��� ���� ��Ͽ� �ְ� ���� ��� �ʱ�ȭ

			// ���� ��� ���
			//for (const auto& node : openList) {
			//	std::cout << "openList, x, y, type: " << node.x << ", " << node.y << ", " << node.type << std::endl;
			//}
			closeList.push_back(*(openList.begin()));
			openList.clear();
			// ���� ��� ���
			//for (const auto& node : closeList) {
			//	std::cout << "closeList, x, y, type: " << node.x << ", " << node.y << ", " << node.type << std::endl;
			//}
		}
	}
}

double CAstar::GetGCost(CNode a, CNode b)
{
	double cost = sqrt(pow(b.x - a.x, 2) + pow(b.y - a.y, 2));
	return double(a.g + cost);
}

void CAstar::make_route(std::vector<CNode> closeList)
{
	std::vector<std::vector<CNode>>::iterator outerIt;
	std::vector<CNode>::iterator innerIt;
	for (const auto& node : closeList) {
		for (outerIt = grid.begin(); outerIt != grid.end(); ++outerIt) {
			innerIt = std::find(outerIt->begin(), outerIt->end(), node);

			if (innerIt != outerIt->end()) {
				// innerIt->type = 'O';
				break; // �� ã���� ���� ����
			}
		}
	}
}

double CAstar::GetHCost(CNode a)
{
	// ��ǥ ���� ���� �������� �߻� �Ÿ�
	return sqrt(pow(endNode.x - a.x, 2) + pow(endNode.y - a.y, 2));
}