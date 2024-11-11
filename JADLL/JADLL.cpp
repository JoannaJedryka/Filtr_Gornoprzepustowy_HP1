#include "pch.h"
#include <stdint.h>
#include <stdlib.h>

extern "C" __declspec(dllexport) int count_c(uint32_t * image, int width, int height, int yStart, int stripHeight) {
    int filter[3][3] = { { 0, -1, 0 },
                         { -1, 5, -1 },
                         { 0, -1, 0 } };

    uint32_t* result = (uint32_t*)malloc(width * height * sizeof(uint32_t));
    if (result == NULL) {
        return -1;
    }

    for (int y = yStart; y < yStart + stripHeight; y++) {
        if (y <= 0 || y >= height - 1) continue;

        for (int x = 1; x < width - 1; x++) {
            int red = 0, green = 0, blue = 0;

            for (int filterY = -1; filterY <= 1; filterY++) {
                for (int filterX = -1; filterX <= 1; filterX++) {
                    int pixel = image[(y + filterY) * width + (x + filterX)];
                    red += ((pixel >> 16) & 0xFF) * filter[filterY + 1][filterX + 1];
                    green += ((pixel >> 8) & 0xFF) * filter[filterY + 1][filterX + 1];
                    blue += (pixel & 0xFF) * filter[filterY + 1][filterX + 1];
                }
            }

            red = red < 0 ? 0 : (red > 255 ? 255 : red);
            green = green < 0 ? 0 : (green > 255 ? 255 : green);
            blue = blue < 0 ? 0 : (blue > 255 ? 255 : blue);

            result[y * width + x] = (red << 16) | (green << 8) | blue;
        }
    }

    for (int y = yStart; y < yStart + stripHeight; y++) {
        for (int x = 1; x < width - 1; x++) {
            image[y * width + x] = result[y * width + x];
        }
    }

    free(result);
    return 0;
}
