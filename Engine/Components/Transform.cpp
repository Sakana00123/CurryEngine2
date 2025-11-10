#include "Transform.h"
#include "Entity.h"

namespace Curry::Transform
{
	namespace
	{
		Util::Vector<Math::Vector4> s_Rotations;		// 各Transformコンポーネントの回転データを格納するベクター
		Util::Vector<Math::Vector3> s_Positions;		// 各Transformコンポーネントの位置データを格納するベクター
		Util::Vector<Math::Vector3> s_Scales;			// 各Transformコンポーネントのスケールデータを格納するベクター

	} // 無名名前空間

	Component CreateTransform(const InitInfo& info, GameEntity::Entity entity)
	{
		assert(entity.IsValid()); // 有効なエンティティであることを確認
		const Id::IdType entityIndex{ Id::Index(entity.GetId()) }; // エンティティのインデックスを取得
		
		if (s_Positions.size() > entityIndex)
		{
			// 既存のエンティティの場合、データを更新
			s_Rotations[entityIndex] = Math::Vector4{ info.rotation };
			s_Positions[entityIndex] = Math::Vector3{ info.position };
			s_Scales[entityIndex] = Math::Vector3{ info.scale };
		}
		else
		{
			// 新しいエンティティの場合、データを追加
			assert(s_Positions.size() == entityIndex);
			s_Rotations.emplace_back(Math::Vector4{ info.rotation });
			s_Positions.emplace_back(Math::Vector3{ info.position });
			s_Scales.emplace_back(Math::Vector3{ info.scale });
		}

		return Component(TransformId{ static_cast<Id::IdType>(s_Positions.size() - 1) });
	}

	void RemoveTransform(Component c)
	{
		// Transformコンポーネントの削除は、ここでは特に何もしない
		// 実際のリソース管理はエンティティ管理システムで行う
		assert(c.IsValid());
	}

	Math::Vector4 Component::Rotation() const
	{
		assert(IsValid());
		return s_Rotations[Id::Index(_id)];
	}
	Math::Vector3 Component::Position() const
	{
		assert(IsValid());
		return s_Positions[Id::Index(_id)];
	}
	Math::Vector3 Component::Scale() const
	{
		assert(IsValid());
		return s_Scales[Id::Index(_id)];
	}

}