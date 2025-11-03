#pragma once

#include "../Components/ComponentsCommon.h"
#include "TransformComponent.h"

namespace Curry::GameEntity
{
	DEFINE_TYPED_ID(EntityId);

	class Entity
	{
	public:
		constexpr explicit Entity(EntityId id) : m_Id(id) {}
		constexpr Entity() : m_Id(Id::InvalidId) {}
		constexpr EntityId GetId() const { return m_Id; }
		constexpr bool IsValid() const { return Id::IsValid(m_Id); }

		Transform::Component Transform() const;
	private:
		EntityId m_Id;
	};
} // namespace Curry::GameEntity