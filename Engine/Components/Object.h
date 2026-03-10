#pragma once
#include "ComponentsCommon.h"

namespace Curry
{
	// コンポーネントの初期化情報の前方宣言マクロ
#define INIT_INFO(Component) namespace Component { struct InitInfo; }

	INIT_INFO(Transform);

	
	namespace GameObject
	{
		struct GameObjectInfo
		{
			Transform::InitInfo* transform{ nullptr };
		};

		GameObject CreateGameObject(const GameObjectInfo& info);
		void RemoveGameObject(GameObject e);
		bool IsAlive(GameObject e);
	} // namespace GameEntity

} // namespace Curry