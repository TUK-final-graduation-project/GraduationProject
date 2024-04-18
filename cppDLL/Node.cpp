#include "Node.h"
#include "math.h"

void Node::SetNode(cppNode _node)
{
	this->node = _node;
}

void Node::SetFCost()
{
	fCost = gCost + hCost;
}

void Node::SetGCost(Node a, Node b)
{
	gCost = sqrt(pow(b.GetNodeX() - a.GetNodeX(), 2) + pow(b.GetNodeY() - a.GetNodeY(), 2));

}

void Node::SetHCost(Node a, Node endNode)
{
	hCost = sqrt(pow(endNode.GetNodeX() - a.GetNodeX(), 2) + pow(endNode.GetNodeY() - a.GetNodeY(), 2));
}

void Node::SetParentNode(Node _parent)
{
	*parentNode = _parent;
}
