#include "pch.h";
#include "JACpp.h"

extern "C" __declspec(dllexport) int count_cpp(int a, int b) {
    return a + b;
}

