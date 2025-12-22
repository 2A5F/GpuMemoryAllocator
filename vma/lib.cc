#include <vulkan/vulkan.h>

#define VMA_IMPLEMENTATION

#if defined(_WIN32) || defined(_WIN64)
  #define VMA_CALL_POST __declspec(dllexport)
#elif defined(__GNUC__)
  #define VMA_CALL_POST __attribute__((visibility("default")))
#else
  #define VMA_CALL_POST
#endif

#include "vma/vk_mem_alloc.h"
