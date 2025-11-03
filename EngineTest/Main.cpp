#pragma comment(lib, "Engine.lib")

#include "TestEntityComponents.h"

#define TEST_ENTITY_COMPONENTS 1

#if TEST_ENTITY_COMPONENTS

#else
#error One of the tests need to be enabled.
#endif

int main()
{
#if _DEBUG
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif // _DEBUG

	EngineTest test{};

	if (!test.Initialize())
	{
		test.Run();
	}
	test.Shutdown();
}