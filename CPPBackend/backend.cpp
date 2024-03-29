#include <iostream>
#include "backend.h"
#include "string"
#include "CAstar.h"

using namespace std;

CAstar c;

extern "C" {
	const char* DLL_EXPORT SimpleReturnFun(float x, float y, bool isWalkable) {
		string s = to_string(x);
		s += " ";
		s += to_string(y);
		s += " ";
		s += to_string(isWalkable);
		return s.c_str();
	}

	int DLL_EXPORT Re(int* arr, int size) {
		for (int i = 0; i < size; ++i) {
			cout << arr[i] << endl;
		}
		return arr[0];
	}
	int DLL_EXPORT SendWorldSize(int x, int y)
	{
		c.SetGridSize(x, y);
		return c.GetGridSizeX();
	}
	bool DLL_EXPORT SendNodeInfo(int x, int y, bool isWalkable)
	{
		c.SetNodeInfo(x, y, isWalkable);
		return c.GetNodeInfo(x, y);
	}
}