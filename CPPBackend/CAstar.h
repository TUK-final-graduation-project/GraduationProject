#include "CNode.h"
#include <vector>

using namespace std;

class CNode;
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
	CNode GetStartNode() {
		return startNode;
	}
	void SetEndNode(int x, int y);
	CNode GetEndNode() {
		return endNode;
	}

	double GetHCost(CNode a);
	double GetGCost(CNode a, CNode b);

	CNode GetPath() {
		CNode c = path.back();
		path.pop_back();
		return c;
	}
	int GetPathSize() {
		// FindPath();
		return path.size();
	}
	void make_route(std::vector<CNode> closeList);

private:
	vector<vector<CNode>> grid;
	// CNode** grid;
	int worldSizeX;
	int worldSizeY;
	CNode startNode;
	CNode endNode;
	vector<CNode> path;
};