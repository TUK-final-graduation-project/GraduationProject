#include "test.h"
#include "CNode.h"
#include <vector>
#include <array>

class CPathMgr
{
private:
	static CPathMgr* g_pInstance;

	NodeClass startNode;
	NodeClass endNode;
	std::vector<NodeClass> closeList{};
public:
	NodeClass* grid;

	int i_worldSizeX;
	int i_worldSizeY;
	
	void setPlayerTarget(int playerX, int playerY, int targetX, int targetY);
	int getPlayerX() {
		return startNode.x;
	}
	static CPathMgr* GetInstance() 
	{
		if (g_pInstance == nullptr) {
			return g_pInstance = new CPathMgr;
		}
		return g_pInstance;
	}

	static void Release() 
	{
		if (g_pInstance != nullptr) {
			delete g_pInstance;
			g_pInstance = nullptr;
		}
	}

	void FindPath();

private:
	CPathMgr();
	~CPathMgr();
};

