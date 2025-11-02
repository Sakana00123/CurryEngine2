#pragma once
#include "ComponentsCommon.h"

namespace Curry::Transform
{
	DEFINE_ID_TYPE(TransformId);

	struct InitInfo
	{
		Float32 position[3]{};
		Float32 rotation[4]{};
		Float32 scale[3]{ 1.f, 1.f, 1.f };
	};


	TransformId CreateTransform(const InitInfo& info, GameEntity::EntityId entityId);
	void RemoveTransform(TransformId id);
}