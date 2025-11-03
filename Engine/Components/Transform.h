#pragma once
#include "ComponentsCommon.h"

namespace Curry::Transform
{
	struct InitInfo
	{
		Float32 position[3]{};
		Float32 rotation[4]{};
		Float32 scale[3]{ 1.f, 1.f, 1.f };
	};


	Component CreateTransform(const InitInfo& info, GameEntity::Entity entity);
	void RemoveTransform(Component c);
}