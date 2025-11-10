#pragma once

#include "../Components/ComponentsCommon.h"
#include "TransformComponent.h"

namespace Curry::GameEntity
{
	DEFINE_TYPED_ID(EntityId);

	class Entity
	{
	public:
		constexpr explicit Entity(EntityId id) : _id(id) {}
		constexpr Entity() : _id(Id::InvalidId) {}
		constexpr EntityId GetId() const { return _id; }
		constexpr bool IsValid() const { return Id::IsValid(_id); }

		Transform::Component Transform() const;
	private:
		EntityId _id;
	};
} // namespace Curry::GameEntity