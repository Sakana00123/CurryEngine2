#pragma once

//#pragma warning(disable: 4530) // disable C++ exception warning (Œx‚ªŸT“©‚µ‚­‚È‚Á‚½‚ç‚±‚±‚ğ—LŒø‰»‚·‚é)

// C/C++ Standard Library Headers
#include <stdint.h>
#include <assert.h>
#include <typeinfo>

#if defined(_WIN64)
#include <DirectXMath.h>
#endif

// Engine Common Headers
#include "../Utilities/Utilities.h"
#include "../Utilities/MathTypes.h"
#include "PrimitiveTypes.h"