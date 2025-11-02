#pragma once
#include "ComponentsCommon.h"

namespace Curry
{
	// コンポーネントの初期化情報の前方宣言マクロ
#define INIT_INFO(Component) namespace Component { struct InitInfo; }

	INIT_INFO(Transform);

	
	namespace GameEntity
	{
		struct EntityInfo
		{
			Transform::InitInfo* transform{ nullptr };
		};

		EntityId CreateGameEntity(const EntityInfo& info);
		void RemoveGameEntity(EntityId id);
		bool IsAlive(EntityId id);
	} // namespace GameEntity

} // namespace Curry