#ifndef PROJECT12_FONT_CONFIG_H
#define PROJECT12_FONT_CONFIG_H

#define USE_TF 0

#include "main.h"
#include "fatfs.h"
#include "qspi/w25q64.h"

typedef struct {
#if USE_TF == 0
    w25qxx_utils fp;
#else
    FIL fp;
#endif
} font_load_fp;

typedef union {
    uint32_t u32;
    uint8_t u8[4];
} font_len_cov;

#endif //PROJECT12_FONT_CONFIG_H
