class Nodeclass {
public:
	bool iswalkable;
	float x;
};

#ifndef TEST_H
#define TEST_H

#define DLL_EXPORT __declspec(dllexport)

extern "C" {
	float DLL_EXPORT SimpleReturnFun(Nodeclass n);
}

#endif