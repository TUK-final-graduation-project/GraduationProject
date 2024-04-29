#pragma once
#include "cppDLL.h"


class Node
{
public:
	bool operator==(const Node& other) const {
		return (this->node.x == other.node.x) && (this->node.y == other.node.y);
	}
	bool operator<(const Node& other) const
	{
		return fCost < other.fCost;
	}
private:
	cppNode node;

	int fCost;
	int gCost;
	int hCost;

	Node* parentNode;
public:
	void SetNode(cppNode node);
	void SetFCost();
	void SetGCost(Node a, Node b);
	void SetHCost(Node a, Node endNode);
	void SetParentNode(Node _parent);

	int GetFCost() {
		return fCost;
	}
	int GetHCost() {
		return hCost;
	}
	int GetGCost() {
		return gCost;
	}
	Node* GetParentNode() {
		return parentNode;
	}
	int GetNodeX() {
		return node.x;
	}
	int GetNodeY() {
		return node.y;
	}
	bool GetWalkable() {
		return node.walkable;
	}
	cppNode GetNode() {
		return node;
	}
};

