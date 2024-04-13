#include "test.h"
float DLL_EXPORT SimpleReturnFun(MYStruct* n, int nCount)
{
	return n[0].nData;
}
