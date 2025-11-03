#pragma once
#include "../Components/ComponentsCommon.h"

namespace Curry::Transform
{
	DEFINE_TYPED_ID(TransformId);
	
	class Component final
	{
	public:
		constexpr explicit Component(TransformId id) : m_Id(id) {}
		constexpr Component() : m_Id(Id::InvalidId) {}
		constexpr TransformId GetId() const { return m_Id; }
		constexpr bool IsValid() const { return Id::IsValid(m_Id); }

		Math::Vector4 Rotation() const;
		Math::Vector3 Position() const;
		Math::Vector3 Scale() const;
	private:
		TransformId m_Id;
	};

}