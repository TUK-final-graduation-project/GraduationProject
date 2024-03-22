#pragma once
class Node {
public:
	bool isWalkable;
	int PosX;
	int PosY;

private:
	int fCost;
	int hCost;
	int gCost;
};
