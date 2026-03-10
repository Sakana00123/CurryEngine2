#pragma once

#include "Test.h"
#include "../Engine/Components/Object.h"
#include "../Engine/Components/Transform.h"

#include <iostream>
#include <ctime>

using namespace Curry;

class EngineTest : public Test
{
public:
	bool Initialize() override
	{
		srand((UInt32)time(nullptr)); // シードを現在の時刻で初期化
		return true;
	}
	
	void Run() override
	{
		do
		{
			for (UInt32 i = 0; i < 10000; ++i)
			{
				CreateRandom();
				RemoveRandom();
				_numEntities = static_cast<UInt32>(_entities.size());
			}
			
			PrintResults();

		} while (getchar() != 'q');
	}
	
	void Shutdown() override
	{ }

private:
	void CreateRandom()
	{
		UInt32 count = rand() % 20;
		if (_entities.empty()) count = 1000; // 最初は多めに生成
		Transform::InitInfo transformInfo{};
		GameObject::GameObjectInfo entityInfo
		{
			&transformInfo,
		};

		while (count > 0)
		{
			++_added;
			GameObject::GameObject entity{ GameObject::CreateGameObject(entityInfo) };
			assert(entity.IsValid() && Id::IsValid(entity.GetId())); // 有効なエンティティであることを確認
			_entities.push_back(entity);
			--count;
		}

	}

	void RemoveRandom()
	{
		UInt32 count = rand() % 20;
		if (_entities.size() < 1000) return; // 最低限は残す
		while (count > 0)
		{
			const UInt32 index{ static_cast<UInt32>(rand()) % static_cast<UInt32>(_entities.size()) };
			const GameObject::GameObject entity{ _entities[index] };
			assert(entity.IsValid() && Id::IsValid(entity.GetId())); // 有効なエンティティであることを確認
			if (entity.IsValid())
			{
				GameObject::RemoveGameObject(entity);
				_entities.erase(_entities.begin() + index);
				assert(!GameObject::IsAlive(entity)); // エンティティが無効であることを確認
				++_removed;
			}
			--count;
		}
	}

	void PrintResults() const
	{
		std::cout << "Added Entities: " << _added << std::endl;
		std::cout << "Removed Entities: " << _removed << std::endl;
		std::cout << "Current Entities: " << _numEntities << std::endl;
	}

	Util::Vector<GameObject::GameObject> _entities;

	UInt32 _added{ 0 };
	UInt32 _removed{ 0 };
	UInt32 _numEntities{ 0 };
};