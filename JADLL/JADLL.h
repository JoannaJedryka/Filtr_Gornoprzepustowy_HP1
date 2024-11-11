#pragma once
#include <stdint.h>  // Include this for uint32_t
#include <stdlib.h>  // Include this for malloc and free
extern "C" __declspec(dllexport) int count_c(uint32_t * image, int width, int height, int yStart, int stripHeight);
