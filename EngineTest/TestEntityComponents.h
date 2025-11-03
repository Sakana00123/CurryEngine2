#pragma once

#include "Test.h"
#include "../Engine/Components/Entity.h"
#include "../Engine/Components/Transform.h"

using namespace Curry;

class EngineTest : public Test
{
public:
	bool Initialize() override { return true; }
	void Run() override {}
	void Shutdown() override {}
};