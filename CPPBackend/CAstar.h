#include "CNode.h"
#include <vector>

using namespace std;

class CAstar 
{
public:
	CAstar();
	~CAstar();
	void FindPath();
	void SetGridSize(int x, int y);
	int GetGridSizeX() {
		return worldSizeX;
	}
	void SetNodeInfo(int x, int y, bool isWalkable);
	bool GetNodeInfo(int x, int y) {
		return grid[x][y].isWalkable;
	}
	void SetStartNode(int x, int y);
	void SetEndNode(int x, int y);

	double GetHCost(CNode a);
	double GetGCost(CNode a, CNode b);

	void make_route(std::vector<CNode> closeList);

private:
	vector<vector<CNode>> grid;
	// CNode** grid;
	int worldSizeX;
	int worldSizeY;
	CNode startNode;
	CNode endNode;
};