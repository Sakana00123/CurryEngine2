
#ifndef EDITOR_INTERFACE
#define EDITOR_INTERFACE extern "C" __declspec(dllexport)
#endif // !EDITOR_INTERFACE

#include "CommonHeaders.h"
#include "Id.h"
#include "../Engine/Components/Entity.h"
#include "../Engine/Components/Transform.h"

using namespace Curry;

namespace
{
	struct TransformComponent
	{
		Float32 position[3];
		Float32 rotation[3];
		Float32 scale[3];

		Transform::InitInfo ToInitInfo() const
		{
			using namespace DirectX;
			Transform::InitInfo info{};
			memcpy(&info.position[0], &position[0], sizeof(Float32) * _countof(position));
			memcpy(&info.scale[0], &scale[0], sizeof(Float32) * _countof(scale));
			// 回転をオイラー角からクォータニオンに変換
			XMFLOAT3A rot{ &rotation[0] };
			XMVECTOR quat{ XMQuaternionRotationRollPitchYawFromVector(XMLoadFloat3A(&rot)) };
			XMFLOAT4A rotQuat{};
			XMStoreFloat4A(&rotQuat, quat);
			memcpy(&info.rotation[0], &rotQuat.x, sizeof(Float32) * _countof(info.rotation));
			return info;
		}
	};

	struct GameEntityDescriptor
	{
		TransformComponent transform;
	};

	GameEntity::Entity EntityFromId(Id::IdType id)
	{
		return GameEntity::Entity{ GameEntity::EntityId{ id } };
	}

} // 無名名前空間

EDITOR_INTERFACE Id::IdType CreateGameEntity(GameEntityDescriptor* e)
{
	assert(e);
	GameEntityDescriptor& desc{ *e };
	Transform::InitInfo transformInfo{ desc.transform.ToInitInfo() };
	GameEntity::EntityInfo entityInfo
	{
		&transformInfo,
	};
	return GameEntity::CreateGameEntity(entityInfo).GetId();
}

EDITOR_INTERFACE void RemoveGameEntity(Id::IdType id)
{
	assert(Id::IsValid(id));
	GameEntity::RemoveGameEntity(EntityFromId(id));
}