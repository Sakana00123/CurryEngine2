#pragma once

#include "../Components/ComponentsCommon.h"
#include "TransformComponent.h"

namespace Curry::GameObject
{
	DEFINE_TYPED_ID(GameObjectId);

	class GameObject
	{
	public:
		constexpr explicit GameObject(GameObjectId id) : _id(id) {}
		constexpr GameObject() : _id(Id::InvalidId) {}
		constexpr GameObjectId GetId() const { return _id; }
		constexpr bool IsValid() const { return Id::IsValid(_id); }

		Transform::Component Transform() const;
	private:
		GameObjectId _id;
	};
} // namespace Curry::GameEntity