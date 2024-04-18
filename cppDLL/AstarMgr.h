#pragma once
#include "Node.h"
#include <vector>

class Node;

class AstarMgr
{
public:
	int gridSizeX;
	int gridSizeY;

	Node** grid;

	Node startNode;
	Node endNode;

	void FindPath();

	std::vector<Node> route;
	std::vector<Node> closeList;
private:
};

