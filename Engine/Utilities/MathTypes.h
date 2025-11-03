#pragma once
#include "CommonHeaders.h"

namespace Curry::Math
{
	constexpr float Pi = 3.14159265358979323846f; // 円周率
	constexpr float Epsilon = 1e-5f; // 浮動小数点の微小値
#if defined(_WIN64)
	using Vector2 = DirectX::XMFLOAT2; // 2次元ベクトル
	using Vector2A = DirectX::XMFLOAT2A; // 2次元ベクトル（アライメント付き）
	using Vector3 = DirectX::XMFLOAT3; // 3次元ベクトル
	using Vector3A = DirectX::XMFLOAT3A; // 3次元ベクトル（アライメント付き）
	using Vector4 = DirectX::XMFLOAT4; // 4次元ベクトル
	using Vector4A = DirectX::XMFLOAT4A; // 4次元ベクトル（アライメント付き）
	using UInt2 = DirectX::XMUINT2; // 2次元符号なし整数ベクトル
	using UInt3 = DirectX::XMUINT3; // 3次元符号なし整数ベクトル
	using UInt4 = DirectX::XMUINT4; // 4次元符号なし整数ベクトル
	using Int2 = DirectX::XMINT2; // 2次元整数ベクトル
	using Int3 = DirectX::XMINT3; // 3次元整数ベクトル
	using Int4 = DirectX::XMINT4; // 4次元整数ベクトル
	using Matrix3x3 = DirectX::XMFLOAT3X3; // 3x3行列
	using Matrix4x4 = DirectX::XMFLOAT4X4; // 4x4行列
	using Matrix4x4A = DirectX::XMFLOAT4X4A; // 4x4行列（アライメント付き）
#endif
}