#ifndef PROJECT12_FONT_WRITE_H
#define PROJECT12_FONT_WRITE_H

#include "main.h"
#include "font_config.h"

#if USE_TF == 0

#define FONT_ADDR 0x1000

#define MAX 0x1000000

#define SAVE_ADDR 0x100
#define TEMP_L 0x8000

#define SAVE_BIT 0xA8

uint32_t write_font(uint32_t pos, uint32_t addr, const char *path);

#endif

#endif //PROJECT12_FONT_WRITE_H
