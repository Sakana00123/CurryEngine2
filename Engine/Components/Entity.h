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

		Entity CreateGameEntity(const EntityInfo& info);
		void RemoveGameEntity(Entity e);
		bool IsAlive(Entity e);
	} // namespace GameEntity

} // namespace Curry