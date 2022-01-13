#include "font_read.h"
#include "myfont.h"
#include "fatfs.h"
#include "qspi/w25q64.h"
#include "lvgl.h"
#include "Show/show.h"
#include "malloc.h"
#include "font_config.h"

my_font_data* get_font_data(lv_font_t* font);
font_load_fp* start_read(my_font_data* font_data);
void read(font_load_fp* fp, void * data, uint16_t size);
void stop_read(font_load_fp* fp);
void error(const char* data);
void set_pos(font_load_fp*,uint32_t);
void add_pos(font_load_fp*,uint32_t);

font_load_api_ font_load_api = {
    .get_font_data = get_font_data,
    .start_read = start_read,
    .read = read,
    .stop_read = stop_read,
    .error = error,
    .set_pos = set_pos,
    .add_pos = add_pos
};

#if USE_TF == 0

void set_pos(font_load_fp* fp,uint32_t pos){
    fp->fp.pos = pos;
}

void add_pos(font_load_fp* fp,uint32_t pos){
    fp->fp.pos += pos;
}

font_load_fp* start_read(my_font_data* font_data){
    font_load_fp* fp = malloc(sizeof(font_load_fp));
    fp->fp.local = font_data->file_local;
    fp->fp.pos = 0;
    return fp;
}

void stop_read(font_load_fp* fp) {
    free(fp);
}

void read(font_load_fp* fp, void * data, uint16_t size) {
    W25QXX_Read_Utils(&fp->fp, data, size);
}

#else

void set_pos(font_load_fp* fp,uint32_t pos) {
    if (f_tell(&fp->fp) == pos)
        return;
    f_lseek(&fp->fp, pos);
}

void add_pos(font_load_fp* fp,uint32_t pos) {
    FIL fil = fp->fp;
    f_lseek(&fil, f_tell(&fil) + pos);
}

font_load_fp* start_read(my_font_data* font_data){
    return &font_data->fp;
}

void stop_read(font_load_fp* fp) {

}

void read(font_load_fp* fp, void * data, uint16_t size) {
    UINT size1;
    f_read(&fp->fp, data, size, &size1);
}

#endif

my_font_data* get_font_data(lv_font_t* font){
    if(font == &font_16.font)
        return &font_16;
    else if(font == &font_24.font)
        return &font_24;
    else if(font == &font_32.font)
        return &font_32;

    return NULL;
}

void error(const char* data)
{
    lv_label_set_text(info, data);
}
