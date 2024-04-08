#include "CPathMgr.h"

CPathMgr* CPathMgr::g_pInstance = nullptr;

void CPathMgr::SetGridSize(int x, int y)
{
	i_worldSizeX = x;
	i_worldSizeY = y;
}

CPathMgr::CPathMgr()
{
	i_worldSizeX = 0;
	i_worldSizeY = 0;
}
CPathMgr::~CPathMgr()
{
}
