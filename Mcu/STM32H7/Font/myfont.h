#ifndef ID3MP3_MYFONT_H
#define ID3MP3_MYFONT_H

#include "font/lv_font.h"
#include "font_load.h"

#ifdef __cplusplus
extern "C" {
#endif

void load_font();

#ifdef __cplusplus
}
#endif

extern my_font_data font_16;
extern my_font_data font_24;
extern my_font_data font_32;

#endif
