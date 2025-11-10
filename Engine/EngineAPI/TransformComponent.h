#pragma once
#include "../Components/ComponentsCommon.h"

namespace Curry::Transform
{
	DEFINE_TYPED_ID(TransformId);
	
	class Component final
	{
	public:
		constexpr explicit Component(TransformId id) : _id(id) {}
		constexpr Component() : _id(Id::InvalidId) {}
		constexpr TransformId GetId() const { return _id; }
		constexpr bool IsValid() const { return Id::IsValid(_id); }

		Math::Vector4 Rotation() const;
		Math::Vector3 Position() const;
		Math::Vector3 Scale() const;
	private:
		TransformId _id;
	};

}