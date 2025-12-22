cmake_minimum_required(VERSION 3.31)

if (WIN32)
    set(CMAKE_C_COMPILER "clang-cl.exe")
    set(CMAKE_CXX_COMPILER "clang-cl.exe")
    set(CMAKE_MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Debug>:Debug>")
endif ()

project(GpuMemoryAllocator LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 23)
set(CMAKE_EXPORT_COMPILE_COMMANDS ON)

add_subdirectory(vma)

if (WIN32)
    find_package(directx-headers CONFIG REQUIRED)
    find_package(D3D12MemoryAllocator CONFIG REQUIRED)
endif ()

add_library(${PROJECT_NAME} SHARED stub.cc)
if (WIN32)
    target_link_libraries(${PROJECT_NAME} PUBLIC Microsoft::DirectX-Headers GPUOpen::D3D12MemoryAllocator)
endif ()

target_link_libraries(${PROJECT_NAME} PUBLIC vma)

