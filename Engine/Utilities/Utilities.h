#pragma once

#define USE_STL_VECTOR 1
#define USE_STL_DEQUE 1

#if USE_STL_VECTOR
#include <vector>
namespace Curry::Util
{
	template<typename T>
	using Vector = std::vector<T>;
}
#endif

#if USE_STL_DEQUE
#include <deque>
namespace Curry::Util
{
	template<typename T>
	using Deque = std::deque<T>;
}
#endif

namespace Curry::Util
{
	// TODO: implement our own containers if needed in the future
}