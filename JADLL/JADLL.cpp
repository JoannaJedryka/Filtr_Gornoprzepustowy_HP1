#include "pch.h";
#include "JADLL.h"

extern "C" __declspec(dllexport) int count_cpp(int a, int b) {
    return a + b;
}