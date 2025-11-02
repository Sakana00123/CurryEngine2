#pragma once
#include <stdint.h>

namespace Curry
{
	// 符号なし整数 (Unsigned Integers)
    using UInt64 = uint64_t;    // 64ビット符号なし整数
    using UInt32 = uint32_t;    // 32ビット符号なし整数
    using UInt16 = uint16_t;    // 16ビット符号なし整数
    using UInt8 = uint8_t;      // 8ビット符号なし整数

	// 符号あり整数 (Signed Integers)
    using Int64 = int64_t;      // 64ビット整数
    using Int32 = int32_t;      // 32ビット整数
    using Int16 = int16_t;      // 16ビット整数
    using Int8 = int8_t;        // 8ビット整数

	// 浮動小数点 (Floating Point)
    using Float32 = float;      // 32ビット浮動小数点
    using Float64 = double;     // 64ビット浮動小数点

	// その他 (Others)
    using Byte = unsigned char; // 1バイトデータ

	// 無効なID定数 (Invalid ID Constants)
	inline constexpr UInt64 UInt64InvalidId = 0xffff'ffff'ffff'ffffull; // 64ビット無効ID
	inline constexpr UInt32 UInt32InvalidId = 0xffff'ffffu;             // 32ビット無効ID
	inline constexpr UInt16 UInt16InvalidId = 0xffffu; 			        // 16ビット無効ID
	inline constexpr UInt8  UInt8InvalidId = 0xffu; 				    // 8ビット無効ID

} // namespace Curry