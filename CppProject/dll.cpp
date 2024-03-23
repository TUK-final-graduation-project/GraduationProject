#include "dll.h"
#include <string>

extern "C" {
	int DLL_EXPORT SimpleTypeReturnFun() {
		return 1;
	}
}