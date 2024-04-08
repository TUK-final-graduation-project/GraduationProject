#pragma once
#include "CNode.h"

class CPathMgr
{
private:
	static CPathMgr* g_pInstance;

	int i_worldSizeX;
	int i_worldSizeY;

public:
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

	void SetGridSize(int x, int y);

	int GetGridSizeX() {
		return i_worldSizeX;
	}
private:
	CPathMgr();
	~CPathMgr();
};

