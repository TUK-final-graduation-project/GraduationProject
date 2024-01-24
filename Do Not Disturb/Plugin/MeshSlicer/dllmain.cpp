// dllmain.cpp : DLL 애플리케이션의 진입점을 정의합니다.
#define DllExport __declspec (dllexport)
#include "pch.h"
#include <stdint.h>
#include <stdlib.h>
#include <time.h>

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

extern "C"
{
    DllExport void SeedRandomizer()
    {
        srand((unsigned int)time(0));
    }

    DllExport float Add(float a, float b)
    {
        return a + b;
    }

    DllExport float Random()
    {
        return (double)(rand()) / ((uint64_t)RAND_MAX + 1);
    }

}