#include "Entity.h"
#include "Transform.h"

namespace Curry::GameEntity
{
	namespace
	{
		Util::Vector<Transform::Component> s_Transforms;		// 各エンティティのTransformコンポーネントを格納するベクター
		Util::Vector<Id::GenerationType> s_Generations;			// 各エンティティの世代番号を格納するベクター
		Util::Deque<EntityId> s_FreeIds; 						// 再利用可能なエンティティIDのデック
	} // 無名名前空間

	Entity CreateGameEntity(const EntityInfo& info)
	{
		assert(info.transform); // 少なくともTransformコンポーネントの初期化情報が必要
		if (!info.transform) {
			return Entity{}; // 無効なIDを返す
		}

		EntityId id;

		if (s_FreeIds.size() > Id::MinDeletedElements)
		{
			id = s_FreeIds.front();
			assert(!IsAlive(Entity{ id }));
			s_FreeIds.pop_front();
			id = EntityId{ Id::NewGeneration(id) };
			++s_Generations[Id::Index(id)];
		}
		else
		{
			id = EntityId{ static_cast<Id::IdType>(s_Generations.size()) };
			s_Generations.push_back(0);  // 新しいエンティティの世代番号を初期化

			// コンポーネントスペースのリサイズ
			// NOTE: resize() を呼ばない理由は、メモリの再割り当てを避けるため。
			s_Transforms.emplace_back(); // Transformコンポーネント用のスペースを確保
		}

		const Entity newEntity{ id };
		const Id::IdType index{ Id::Index(id) };

		// Transformコンポーネントの作成
		assert(!s_Transforms[index].IsValid());
		s_Transforms[index] = (Transform::CreateTransform(*info.transform, newEntity));
		if (!s_Transforms[index].IsValid())
		{
			// コンポーネントの作成に失敗した場合、無効なエンティティを返す
			return Entity{};
		}

		return newEntity;
	}

	void RemoveGameEntity(Entity e)
	{
		const EntityId id{ e.GetId() };
		const Id::IdType index{ Id::Index(id) };
		assert(IsAlive(e)); // エンティティが有効であることを確認
		if (IsAlive(e))
		{
			// Transformコンポーネントの削除
			Transform::RemoveTransform(s_Transforms[index]);
			s_Transforms[index] = Transform::Component{}; // コンポーネントを無効化
			s_FreeIds.push_back(id); // エンティティIDを再利用リストに追加
		}
	}

	bool IsAlive(Entity e)
	{
		assert(e.IsValid()); // エンティティが有効なIDを持っていることを確認
		const EntityId id{ e.GetId() };
		const Id::IdType index{ Id::Index(id) };
		assert(index < s_Generations.size()); // インデックスが範囲内であることを確認
		assert(s_Generations[index] == Id::Generation(id)); // 世代番号が一致することを確認
		return (s_Generations[index] == Id::Generation(id) && s_Transforms[index].IsValid());
	}

	Transform::Component Entity::Transform() const
	{
		assert(IsAlive(*this)); // エンティティが有効であることを確認
		const Id::IdType index{ Id::Index(m_Id) }; // エンティティのインデックスを取得
		return s_Transforms[index];
	}
}