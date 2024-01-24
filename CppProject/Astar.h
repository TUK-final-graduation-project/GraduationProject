#ifndef ASTAR_H
#define ASTAR_H

#define DLL_EXPORT __declspec(dllexport)
class Node {
public:
	int x;
	int y;
};
extern "C" {
	int DLL_EXPORT SimpleTypeArgFun(Node n);
}

#endif