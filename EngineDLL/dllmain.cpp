
#pragma comment(lib, "Engine.lib")

// DllMain.cpp : DLL アプリケーションのエントリ ポイントを定義します。
#define WIN32_LEAN_AND_MEAN             // Windows ヘッダーからほとんど使用されない内容を除外します。
#include <Windows.h>
#include <crtdbg.h>

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
#if _DEBUG
        _CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif // _DEBUG
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

