#include "Astar.h"

extern "C" {
	int DLL_EXPORT SimpleTypeArgFun(Node n) {
		return n.x + n.y;
	}
}