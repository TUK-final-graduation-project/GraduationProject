#pragma once
class CNode
{
public:
	int x, y;
	bool isWalkable;

	int f, g, h;
	CNode* parent;
	// �� ������ �����ε�
	bool operator<(const CNode& other) const
	{
		return f < other.f;
	}
	bool operator==(const CNode& other) const
	{
		return ((x == other.x) && (y == other.y));
	}
};

