#include <vulkan/vulkan.h>

#define VMA_IMPLEMENTATION

#if defined(WIN32)
  #define VMA_CALL_POST __declspec(dllexport)
#elif defined(__GNUC__)
  #define VMA_CALL_POST __attribute__((visibility("default")))
#else
  #define VMA_CALL_POST
#endif

#if defined(WIN32)
#include "vma/vk_mem_alloc.h"
#else
#include "vk_mem_alloc.h"
#endif
