#pragma once
#include "CommonHeaders.h"

namespace Curry::Id
{
	// IDの型定義 (ID Type Definition)
	using IdType = UInt64; // 64ビットID

	// IDのビット構成 (ID Bit Structure)
	inline constexpr UInt32 GenerationBits{ 8 };												// 世代番号に使用するビット数
	inline constexpr UInt32 IndexBits{ sizeof(IdType) * 8 - GenerationBits };					// インデックスに使用するビット数
	inline constexpr IdType IndexMask{ (IdType{ 1 } << IndexBits) - IdType{ 1 } };				// インデックスマスク
	inline constexpr IdType GenerationMask{ (IdType{ 1 } << GenerationBits) - IdType{ 1 } };	// 世代番号マスク
	inline constexpr IdType IdMask{ IdType{-1} };												// ID全体のマスク

	// 世代番号の型定義 (Generation Number Type Definition)
	using GenerationType = std::conditional_t<GenerationBits <= 16, std::conditional_t<GenerationBits <= 8, UInt8, UInt16>, UInt32>; // 世代番号の型（ビット数に応じて選択）
	static_assert(sizeof(GenerationType) * 8 >= GenerationBits, "GenerationType is too small to hold GenerationBits.");	// 世代番号の型が世代番号ビット数を保持するのに十分であることを確認するための静的アサーション
	static_assert(sizeof(GenerationType) <= sizeof(IdType), "GenerationType must not be larger than IdType.");			// 世代番号の型がID型より大きくないことを確認するための静的アサーション

	/**
	 * @brief 指定されたIDが有効かどうかを確認します。
	 * @param id 確認するID。
	 * @return 有効なIDであればtrue、無効なIDであればfalseを返します。
	 * @note 無効なIDはIdMaskと等しいIDです。
	 */
	inline bool IsValid(IdType id)
	{
		return (id & IdMask) != IdMask;
	}

	/**
	 * @brief 指定されたIDから世代番号を取得します。
	 * @param id 取得するID。
	 * @return IDに対応する世代番号を返します。
	 */
	inline IdType Generation(IdType id)
	{
		return (id >> IndexBits) & GenerationMask;
	}

	/**
	 * @brief 指定されたIDからインデックスを取得します。
	 * @param id 取得するID。
	 * @return IDに対応するインデックスを返します。
	 */
	inline IdType Index(IdType id)
	{
		return id & IndexMask;
	}

	/**
	 * @brief 指定されたIDの世代番号をインクリメントして新しいIDを生成します。
	 * @param id 元のID。
	 * @return 世代番号がインクリメントされた新しいIDを返します。
	 * @note インデックスは元のIDと同じままです。
	 */
	inline IdType NewGeneration(IdType id)
	{
		const IdType generation{ Id::Generation(id) + 1 }; // 世代番号をインクリメント
		assert(generation < 255);
		return Index(id) | (generation << IndexBits); // インデックスはそのままで世代番号をインクリメントして新しいIDを生成
	}

#if _DEBUG
	namespace Internal
	{
		struct IdBase
		{
			constexpr explicit IdBase(IdType id) : m_Id(id) {}
			constexpr operator IdType() const { return m_Id; }
		private:
			IdType m_Id;
		};
	} // namespace Internal

#define DEFINE_ID_TYPE(Name)								\
		struct Name final : public Id::Internal::IdBase		\
		{													\
			constexpr explicit Name(Id::IdType id)			\
				: Id::Internal::IdBase(id) {}				\
			constexpr Name() : IdBase(Id::IdMask) {}		\
		};
#else
#define DEFINE_ID_TYPE(Name) using Name = Id::IdType;
#endif
} // namespace Curry::Id