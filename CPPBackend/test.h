typedef struct _stMyST {
	int nData;
	bool bTrue;
} MYStruct;

#ifndef TEST_H
#define TEST_H

#define DLL_EXPORT __declspec(dllexport)

extern "C" {
	DLL_EXPORT float SimpleReturnFun(MYStruct* n, int nCount);
}

#endif