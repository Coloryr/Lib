#ifndef PROJECT12_FONT_READ_H
#define PROJECT12_FONT_READ_H

#include "lvgl.h"
#include "font_load.h"
#include "fatfs.h"
#include "font_config.h"
#include "qspi/w25q64.h"

typedef struct {
#if USE_TF == 0
    w25qxx_utils fp;
#else
    FIL fp;
#endif
} font_load_fp;

typedef struct {
    my_font_data* (*get_font_data)(lv_font_t*);
    font_load_fp* (*start_read)(my_font_data*);
    void (*read)(font_load_fp*, void *, uint16_t);
    void (*stop_read)(font_load_fp*);
    void (*error)(const char*);
    void (*set_pos)(font_load_fp*, uint32_t);
    void (*add_pos)(font_load_fp*, uint32_t);
} font_load_api_;

extern font_load_api_ font_load_api;

#endif //PROJECT12_FONT_READ_H
