project(vma LANGUAGES CXX)

find_package(Vulkan)
find_package(VulkanMemoryAllocator CONFIG REQUIRED)

add_library(${PROJECT_NAME} SHARED lib.cc)
target_link_libraries(${PROJECT_NAME} PUBLIC Vulkan::Vulkan GPUOpen::VulkanMemoryAllocator)
