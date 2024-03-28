#include "backend.h"
#include "string"

using namespace std;

extern "C" {
	const char* DLL_EXPORT SimpleReturnFun(float x, float y, bool isWalkable) {
		string s = to_string(x);
		s += " ";
		s += to_string(y);
		s += " ";
		s += to_string(isWalkable);
		
		return s.c_str();
	}
}